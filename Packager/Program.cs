namespace Packager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Contracts;

    /// <summary>
    /// Generates a .nuspec file based on project .csproj content.
    /// </summary>
    public partial class Program
    {
        private void ExecuteProgram(bool isDebug, bool isMerge, string mergeName, string nuspecDescription, out bool hasErrors)
        {
            LoadSolutionAndProjectList(out string SolutionName, out List<Project> ProjectList);
            FilterProcessedProjects(ProjectList, out List<Project> ProcessedProjectList, out hasErrors);

            List<Nuspec> NuspecList = new List<Nuspec>();

            if (isMerge)
            {
                MergeProjects(SolutionName, ProcessedProjectList, mergeName, nuspecDescription, out Nuspec mergedNuspec, ref hasErrors);

                if (!hasErrors)
                {
                    NuspecList.Add(mergedNuspec);
                    ConsoleDebug.Write("All projects have been merged");
                }
            }
            else
            {
                NuspecList = new List<Nuspec>();
                foreach (Project Project in ProcessedProjectList)
                    NuspecList.Add(Project.ToNuspec());
            }

            foreach (Nuspec Nuspec in NuspecList)
                WriteNuspec(Nuspec, isDebug);
        }

        private static void LoadSolutionAndProjectList(out string solutionName, out List<Project> projectList)
        {
            solutionName = string.Empty;
            projectList = new List<Project>();

            string CurrentDirectory = Environment.CurrentDirectory;
            ConsoleDebug.Write($"Current Directory: {CurrentDirectory}");

            string[] Files = Directory.GetFiles(CurrentDirectory, "*.sln");
            ConsoleDebug.Write($"Found {Files.Length} solution file(s)");

            foreach (string SolutionFileName in Files)
            {
                ConsoleDebug.Write($"  Solution file: {SolutionFileName}");
                Solution NewSolution = new Solution(SolutionFileName);

                solutionName = NewSolution.Name;

                foreach (Project Item in NewSolution.ProjectList)
                {
                    bool IsIgnored = Item.ProjectType != "Unknown" && Item.ProjectType != "KnownToBeMSBuildFormat";
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
            processedProjectList = new List<Project>();
            hasErrors = false;

            foreach (Project Item in projectList)
            {
                ConsoleDebug.Write($"  Project file: {Item.RelativePath}");

                Item.Parse(ref hasErrors);
                if (Item.HasVersion && Item.IsAssemblyVersionValid && Item.IsFileVersionValid && Item.HasRepositoryUrl && Item.HasTargetFrameworks)
                    processedProjectList.Add(Item);
            }

            ConsoleDebug.Write($"Processing {processedProjectList.Count} project file(s)");
        }

        private static void MergeProjects(string solutionName, List<Project> projectList, string mergeName, string nuspecDescription, out Nuspec mergedNuspec, ref bool hasErrors)
        {
            mergedNuspec = Nuspec.Empty;

            Project? SelectedProject = null;

            foreach (Project Project in projectList)
                if (Project.ProjectName == mergeName)
                    SelectedProject = Project;

            if (SelectedProject == null)
            {
                if (solutionName.Length == 0 || projectList.Count == 0)
                {
                    hasErrors = true;
                    return;
                }

                SelectedProject = projectList[0];
            }

            string Description = nuspecDescription.Length > 0 ? nuspecDescription : SelectedProject.Description;
            Contract.RequireNotNull(SelectedProject.RepositoryUrl, out Uri RepositoryUrl);

            mergedNuspec = new Nuspec(solutionName, string.Empty, SelectedProject.Version, SelectedProject.Author, Description, SelectedProject.Copyright, RepositoryUrl, SelectedProject.FrameworkList);

            foreach (Project Project in projectList)
                if (Project.Version != mergedNuspec.Version || Project.Author != mergedNuspec.Author || Project.Copyright != mergedNuspec.Copyright || Project.RepositoryUrl != mergedNuspec.RepositoryUrl || !IsFrameworkListEqual(Project.FrameworkList, mergedNuspec.FrameworkList))
                {
                    hasErrors = true;
                    return;
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

        private static void WriteNuspec(Nuspec nuspec, bool isDebug)
        {
            if (nuspec.RelativePath.Length > 0)
                ConsoleDebug.Write($"  Processing: {nuspec.RelativePath}");
            else
                ConsoleDebug.Write($"  Processing...");

            InitializeFile(nuspec, isDebug, out string NuspecPath);

            using FileStream Stream = new FileStream(NuspecPath, FileMode.Append, FileAccess.Write);
            using StreamWriter Writer = new StreamWriter(Stream, Encoding.UTF8);

            Writer.WriteLine("<package>");
            Writer.WriteLine("  <metadata>");

            WriteMiscellaneousInfo(Writer, nuspec, isDebug);
            WriteDependencies(Writer, nuspec);
            WriteExtraContentFiles(Writer, isDebug);

            Writer.WriteLine("  </metadata>");
            Writer.Write("</package>");
        }

        private static void InitializeFile(Nuspec nuspec, bool isDebug, out string nuspecPath)
        {
            string DebugSuffix = GetDebugSuffix(isDebug);
            string NugetDirectory = isDebug ? "nuget-debug" : "nuget";

            if (!Directory.Exists(NugetDirectory))
                Directory.CreateDirectory(NugetDirectory);

            string NuspecFileName = $"{nuspec.Name}{DebugSuffix}.nuspec";
            nuspecPath = Path.Combine(NugetDirectory, NuspecFileName);

            using FileStream FirstStream = new FileStream(nuspecPath, FileMode.Create, FileAccess.Write);
            using StreamWriter FirstWriter = new StreamWriter(FirstStream, Encoding.ASCII);
            FirstWriter.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");

            ConsoleDebug.Write($"     Created: {nuspecPath}");
        }

        private static void WriteMiscellaneousInfo(StreamWriter writer, Nuspec nuspec, bool isDebug)
        {
            string DebugSuffix = GetDebugSuffix(isDebug);
            writer.WriteLine($"    <id>{nuspec.Name}{DebugSuffix}</id>");
            writer.WriteLine($"    <version>{nuspec.Version}</version>");

            if (nuspec.Author.Length > 0)
                writer.WriteLine($"    <authors>{HtmlEncoded(nuspec.Author)}</authors>");

            if (nuspec.Description.Length > 0)
                writer.WriteLine($"    <description>{HtmlEncoded(nuspec.Description)}</description>");

            if (nuspec.Copyright.Length > 0)
                writer.WriteLine($"    <copyright>{HtmlEncoded(nuspec.Copyright)}</copyright>");

            writer.WriteLine($"    <repository type=\"git\" url=\"{nuspec.RepositoryUrl}\"/>");
        }

        private static void WriteDependencies(StreamWriter writer, Nuspec nuspec)
        {
            writer.WriteLine("    <dependencies>");

            foreach (Framework Framework in nuspec.FrameworkList)
            {
                string FrameworkName = string.Empty;

                switch (Framework.Type)
                {
                    case FrameworkType.NetStandard:
                        FrameworkName = ".NETStandard";
                        break;
                    case FrameworkType.NetCore:
                        FrameworkName = ".NETCoreApp";
                        break;
                    case FrameworkType.NetFramework:
                        FrameworkName = ".NETFramework";
                        break;
                }

                writer.WriteLine($"      <group targetFramework=\"{FrameworkName}{Framework.Major}.{Framework.Minor}\"/>");
            }

            writer.WriteLine("    </dependencies>");
        }

        private static void WriteExtraContentFiles(StreamWriter writer, bool isDebug)
        {
            if (isDebug)
            {
                writer.WriteLine("    <contentFiles>");
                writer.WriteLine("      <files include=\"lib/**/*.pdb\"/>");
                writer.WriteLine("    </contentFiles>");
            }
        }

        private static string HtmlEncoded(string s)
        {
            s = s.Replace("\"", "&quot;");
            s = s.Replace("?", "&amp;");
            s = s.Replace("<", "&lt;");
            s = s.Replace(">", "&gt;");

            return s;
        }

        private static string GetDebugSuffix(bool isDebug)
        {
            return isDebug ? "-Debug" : string.Empty;
        }
    }
}
