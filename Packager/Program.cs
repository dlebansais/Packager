﻿namespace Packager;

using System;
using System.Collections.Generic;
using System.IO;
using Contracts;
using SlnExplorer;

/// <summary>
/// Generates a .nuspec file based on project .csproj content.
/// </summary>
internal partial class Program
{
    private static void ExecuteProgram(bool isDebug, bool isAnalyzer, bool isMerge, string? mergeName, string nuspecDescription, string nuspecIcon, string nuspecPrefix, out bool hasErrors)
    {
        ConsoleDebug.Write($"Current Directory: {Environment.CurrentDirectory}");

        CheckOutputDirectory(isDebug, out bool IsDirectoryExisting, out string NugetDirectory);
        if (!IsDirectoryExisting)
        {
            ConsoleDebug.Write($"WARNING: output folder {NugetDirectory} does not exist");
            hasErrors = false;
            return;
        }

        LoadSolutionAndProjectList(out string SolutionName, out List<Project> ProjectList);
        FilterProcessedProjects(ProjectList, out List<Project> ProcessedProjectList, out hasErrors);

        List<Nuspec> NuspecList = [];

        if (isMerge)
        {
            MergeProjects(isDebug, SolutionName, ProcessedProjectList, mergeName, nuspecDescription, out Nuspec mergedNuspec, ref hasErrors);

            if (!hasErrors)
            {
                NuspecList.Add(mergedNuspec);
                ConsoleDebug.Write("All projects have been merged");
            }
        }
        else
        {
            NuspecList = [];
            foreach (Project Project in ProcessedProjectList)
                NuspecList.Add(Nuspec.FromProject(isDebug, Project));
        }

        foreach (Nuspec Nuspec in NuspecList)
            WriteNuspec(Nuspec, isDebug, isAnalyzer, nuspecIcon, nuspecPrefix);
    }

    private static void CheckOutputDirectory(bool isDebug, out bool isDirectoryExisting, out string nugetDirectory)
    {
        nugetDirectory = isDebug ? "nuget-debug" : "nuget";
        isDirectoryExisting = Directory.Exists(nugetDirectory);
    }

    private static void LoadSolutionAndProjectList(out string solutionName, out List<Project> projectList)
    {
        solutionName = string.Empty;
        projectList = [];

        string[] Files = Directory.GetFiles(Environment.CurrentDirectory, "*.sln");
        ConsoleDebug.Write($"Found {Files.Length} solution file(s)");

        foreach (string SolutionFileName in Files)
        {
            ConsoleDebug.Write($"  Solution file: {SolutionFileName}");
            Solution NewSolution = new(SolutionFileName);

            solutionName = NewSolution.Name;

            foreach (Project Item in NewSolution.ProjectList)
            {
                bool IsIgnored = Item.ProjectType > ProjectType.KnownToBeMSBuildFormat;
                string Operation = IsIgnored ? "Ignored" : "Parsed";

                ConsoleDebug.Write($"    Project: {Item.ProjectName} ({Operation})");

                if (!IsIgnored)
                    projectList.Add(Item);
            }
        }

        ConsoleDebug.Write($"Found {projectList.Count} project file(s)");
    }

    private static void FilterProcessedProjects(List<Project> projectList, out List<Project> processedProjectList, out bool hasErrors)
    {
        processedProjectList = [];
        hasErrors = false;

        foreach (Project Item in projectList)
            FilterProcessedProject(Item, processedProjectList, ref hasErrors);

        ConsoleDebug.Write($"Processing {processedProjectList.Count} project file(s)");
    }

    private static void FilterProcessedProject(Project project, List<Project> processedProjectList, ref bool hasErrors)
    {
        ConsoleDebug.Write($"  Project file: {project.RelativePath}");

        project.LoadDetails(project.RelativePath);
        bool ProjectHasErrors = project.CheckVersionConsistency(out string WarningOrErrorText);
        hasErrors |= ProjectHasErrors;

        if (WarningOrErrorText.Length > 0)
            if (ProjectHasErrors)
                ConsoleDebug.Write($"    ERROR: {WarningOrErrorText}", true);
            else
                ConsoleDebug.Write($"    {WarningOrErrorText}", false);
        else
            FilterProcessedProjectNoWarning(project, processedProjectList);
    }

    private static void FilterProcessedProjectNoWarning(Project project, List<Project> processedProjectList)
    {
        Contract.Assert(project.HasVersion);
        Contract.Assert(project.IsAssemblyVersionValid);
        Contract.Assert(project.IsFileVersionValid);

        Contract.Assert(project.Version.Length > 0);
        ConsoleDebug.Write($"    Version: {project.Version}");

        Contract.Assert(project.AssemblyVersion.Length > 0);
        ConsoleDebug.Write($"    Assembly Version: {project.AssemblyVersion}");

        Contract.Assert(project.FileVersion.Length > 0);
        ConsoleDebug.Write($"    File Version: {project.FileVersion}");

        if (project.HasRepositoryUrl)
            ConsoleDebug.Write($"    Repository Url: {project.RepositoryUrl}");
        if (project.FrameworkList.Count > 0)
        {
            string TargetFrameworks = string.Empty;

            foreach (Framework Item in project.FrameworkList)
            {
                if (TargetFrameworks.Length > 0)
                    TargetFrameworks += ";";
                TargetFrameworks += Item.Name;
            }

            ConsoleDebug.Write($"    Target Framework(s): {TargetFrameworks}");
        }

        if (project.IsTestProject)
            ConsoleDebug.Write($"    Ignored by packager (test)");
        else if (project.IsNotPackable)
            ConsoleDebug.Write($"    Ignored by packager (not packable)");
        else if (!project.HasTargetFrameworks)
            ConsoleDebug.Write($"    Ignored by packager (no target framework)");
        else if (!project.HasRepositoryUrl)
            ConsoleDebug.Write($"    Ignored by packager (no repository URL)");
        else
            processedProjectList.Add(project);
    }

    private static void MergeProjects(bool isDebug, string solutionName, List<Project> projectList, string? mergeName, string nuspecDescription, out Nuspec mergedNuspec, ref bool hasErrors)
    {
        if (!FindSelectedProject(solutionName, projectList, mergeName, out Project SelectedProject) ||
            !MergePackageDependencies(isDebug, projectList, out List<PackageReference> MergedPackageDependencies))
        {
            mergedNuspec = Nuspec.Empty;
            hasErrors = true;
            return;
        }

        string Description = nuspecDescription.Length > 0 ? nuspecDescription : SelectedProject.Description;
        Uri RepositoryUrl = Contract.AssertNotNull(SelectedProject.RepositoryUrl);

        mergedNuspec = new Nuspec(solutionName, string.Empty, SelectedProject.Version, SelectedProject.Author, Description, SelectedProject.Copyright, RepositoryUrl, SelectedProject.ApplicationIcon, SelectedProject.FrameworkList, MergedPackageDependencies);

        foreach (Project Project in projectList)
        {
            if (Project.Version != mergedNuspec.Version ||
                Project.Author != mergedNuspec.Author ||
                Project.Copyright != mergedNuspec.Copyright ||
                Project.RepositoryUrl != mergedNuspec.RepositoryUrl ||
                !IsFrameworkListEqual(Project.FrameworkList, mergedNuspec.FrameworkList))
            {
                hasErrors = true;
                return;
            }
        }
    }

    private static bool FindSelectedProject(string solutionName, List<Project> projectList, string? mergeName, out Project selectedProject)
    {
        foreach (Project Project in projectList)
            if (Project.ProjectName == mergeName)
            {
                selectedProject = Project;
                return true;
            }

        if (mergeName is null && solutionName.Length > 0 && projectList.Count > 0)
        {
            selectedProject = projectList[0];
            return true;
        }

        Contract.Unused(out selectedProject);
        return false;
    }

    private static bool MergePackageDependencies(bool isDebug, List<Project> projectList, out List<PackageReference> mergedPackageDependencies)
    {
        mergedPackageDependencies = [];

        Dictionary<string, List<PackageReference>> ConflictTable = [];
        foreach (Project Project in projectList)
        {
            List<PackageReference> ProjectPackageDependencies = Nuspec.GetPackageDependencies(isDebug, Project);
            MergePackageDependencies(mergedPackageDependencies, ProjectPackageDependencies, ConflictTable);
        }

        if (ConflictTable.Count == 0)
            return true;

        WriteConflicts(ConflictTable);
        return false;
    }

    private static void WriteConflicts(Dictionary<string, List<PackageReference>> conflictTable)
    {
        ConsoleDebug.Write($"ERROR: {conflictTable.Count} {(conflictTable.Count == 1 ? "dependency" : "dependencies")} with conflicting versions.", isError: true);

        foreach (KeyValuePair<string, List<PackageReference>> Entry in conflictTable)
        {
            ConsoleDebug.Write($"    {Entry.Key}");

            foreach (PackageReference Package in Entry.Value)
                ConsoleDebug.Write($"      {Package.Version}");
        }
    }

    private static bool IsFrameworkListEqual(IReadOnlyList<Framework> list1, IReadOnlyList<Framework> list2)
    {
        if (list1.Count != list2.Count)
            return false;

        for (int i = 0; i < list1.Count; i++)
            if (list1[i].Type != list2[i].Type || list1[i].Major != list2[i].Major || list1[i].Minor != list2[i].Minor || list1[i].Moniker != list2[i].Moniker)
                return false;

        return true;
    }

    private static void MergePackageDependencies(List<PackageReference> mergedList, List<PackageReference> list, Dictionary<string, List<PackageReference>> conflictTable)
    {
        foreach (PackageReference Package in list)
            if (mergedList.Find(p => Package.Name == p.Name) is PackageReference MatchingPackage)
            {
                if (!IsPackageReferenceEqual(Package, MatchingPackage))
                    if (conflictTable.TryGetValue(Package.Name, out List<PackageReference>? ConflictTableValue))
                    {
                        List<PackageReference> ExistingConflicts = Contract.AssertNotNull(ConflictTableValue);

                        if (ExistingConflicts.Find(p => IsPackageReferenceEqual(Package, p)) is null)
                            ExistingConflicts.Add(Package);
                    }
                    else
                    {
                        conflictTable.Add(Package.Name, [MatchingPackage, Package]);
                    }
            }
            else
            {
                mergedList.Add(Package);
            }
    }

    private static bool IsPackageReferenceEqual(PackageReference package1, PackageReference package2)
    {
        Contract.Assert(package1.Name == package2.Name);

        return package1.Version == package2.Version;
    }
}
