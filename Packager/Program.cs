namespace Packager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows.Media.Imaging;
    using Contracts;
    using SlnExplorer;

    /// <summary>
    /// Generates a .nuspec file based on project .csproj content.
    /// </summary>
    public partial class Program
    {
        private void ExecuteProgram(bool isDebug, bool isMerge, string mergeName, string nuspecDescription, string nuspecIcon, out bool hasErrors)
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

            List<Nuspec> NuspecList = new List<Nuspec>();

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
                NuspecList = new List<Nuspec>();
                foreach (Project Project in ProcessedProjectList)
                    NuspecList.Add(Nuspec.FromProject(isDebug, Project));
            }

            foreach (Nuspec Nuspec in NuspecList)
                WriteNuspec(Nuspec, isDebug, nuspecIcon);
        }

        private void CheckOutputDirectory(bool isDebug, out bool isDirectoryExisting, out string nugetDirectory)
        {
            nugetDirectory = isDebug ? "nuget-debug" : "nuget";
            isDirectoryExisting = Directory.Exists(nugetDirectory);
        }

        private static void LoadSolutionAndProjectList(out string solutionName, out List<Project> projectList)
        {
            solutionName = string.Empty;
            projectList = new List<Project>();

            string[] Files = Directory.GetFiles(Environment.CurrentDirectory, "*.sln");
            ConsoleDebug.Write($"Found {Files.Length} solution file(s)");

            foreach (string SolutionFileName in Files)
            {
                ConsoleDebug.Write($"  Solution file: {SolutionFileName}");
                Solution NewSolution = new Solution(SolutionFileName);

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
            processedProjectList = new List<Project>();
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
            if (project.Version.Length > 0)
                ConsoleDebug.Write($"    Version: {project.Version}");
            if (project.AssemblyVersion.Length > 0)
                ConsoleDebug.Write($"    Assembly Version: {project.AssemblyVersion}");
            if (project.FileVersion.Length > 0)
                ConsoleDebug.Write($"    File Version: {project.FileVersion}");
            if (project.RepositoryUrl != null)
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

            if (project.HasVersion && project.IsAssemblyVersionValid && project.IsFileVersionValid && project.HasRepositoryUrl && project.HasTargetFrameworks)
                processedProjectList.Add(project);
        }

        private static void MergeProjects(bool isDebug, string solutionName, List<Project> projectList, string mergeName, string nuspecDescription, out Nuspec mergedNuspec, ref bool hasErrors)
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

            List<PackageReference> MergedPackageDependencies = new();

            foreach (Project Project in projectList)
            {
                List<PackageReference> ProjectPackageDependencies = Nuspec.GetPackageDependencies(isDebug, Project);
                MergePackageDependencies(MergedPackageDependencies, ProjectPackageDependencies);
            }

            mergedNuspec = new Nuspec(solutionName, string.Empty, SelectedProject.Version, SelectedProject.Author, Description, SelectedProject.Copyright, RepositoryUrl, SelectedProject.ApplicationIcon, SelectedProject.FrameworkList, MergedPackageDependencies);

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

        private static void MergePackageDependencies(List<PackageReference> mergedList, List<PackageReference> list)
        {
            foreach (PackageReference Package1 in list)
            {
                bool IsFound = false;

                foreach (PackageReference Package2 in mergedList)
                    if (IsPackageReferenceEqual(Package1, Package2))
                    {
                        IsFound = true;
                        break;
                    }

                if (!IsFound)
                    mergedList.Add(Package1);
            }
        }

        private static bool IsPackageReferenceEqual(PackageReference package1, PackageReference package2)
        {
            return package1.Name == package2.Name && package1.Version == package2.Version;
        }

        private static void WriteNuspec(Nuspec nuspec, bool isDebug, string nuspecIcon)
        {
            if (nuspec.RelativePath.Length > 0)
                ConsoleDebug.Write($"  Processing: {nuspec.RelativePath}");
            else
                ConsoleDebug.Write($"  Processing...");

            InitializeFile(nuspec, isDebug, out string NuspecPath);

            using FileStream Stream = new FileStream(NuspecPath, FileMode.Append, FileAccess.Write);
            using StreamWriter Writer = new StreamWriter(Stream, Encoding.UTF8);

            Writer.WriteLine("<package xmlns=\"http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd\">");
            Writer.WriteLine("  <metadata>");

            string ApplicationIcon = nuspecIcon.Length > 0 ? nuspecIcon : nuspec.ApplicationIcon;

            WriteMiscellaneousInfo(Writer, nuspec, isDebug, NuspecPath, ApplicationIcon);
            WriteDependencies(Writer, nuspec);
            WriteExtraContentFiles(Writer, isDebug);

            Writer.WriteLine("  </metadata>");
            Writer.Write("</package>");
        }

        private static void InitializeFile(Nuspec nuspec, bool isDebug, out string nuspecPath)
        {
            string DebugSuffix = GetDebugSuffix(isDebug);
            string NugetDirectory = isDebug ? "nuget-debug" : "nuget";
            string NuspecFileName = $"{nuspec.Name}{DebugSuffix}.nuspec";
            nuspecPath = Path.Combine(NugetDirectory, NuspecFileName);

            using FileStream FirstStream = new FileStream(nuspecPath, FileMode.Create, FileAccess.Write);
            using StreamWriter FirstWriter = new StreamWriter(FirstStream, Encoding.ASCII);
            FirstWriter.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");

            ConsoleDebug.Write($"     Created: {nuspecPath}");
        }

        private static void WriteMiscellaneousInfo(StreamWriter writer, Nuspec nuspec, bool isDebug, string nuspecPath, string nuspecIcon)
        {
            string DebugSuffix = GetDebugSuffix(isDebug);
            string DebugTitle = GetDebugTitle(isDebug);

            writer.WriteLine($"    <id>{nuspec.Name}{DebugSuffix}</id>");
            writer.WriteLine($"    <version>{nuspec.Version}</version>");
            writer.WriteLine($"    <title>{nuspec.Name}{DebugTitle}</title>");

            if (nuspec.Author.Length > 0)
                writer.WriteLine($"    <authors>{HtmlEncoded(nuspec.Author)}</authors>");

            if (nuspec.Description.Length > 0)
                writer.WriteLine($"    <description>{HtmlEncoded(nuspec.Description)}</description>");

            if (nuspec.Copyright.Length > 0)
                writer.WriteLine($"    <copyright>{HtmlEncoded(nuspec.Copyright)}</copyright>");

            if (nuspecIcon.Length > 0)
            {
                ConvertIcoToPng(nuspecPath, ref nuspecIcon);
                writer.WriteLine($"    <icon>{nuspecIcon}</icon>");
            }

            writer.WriteLine($"    <projectUrl>{nuspec.RepositoryUrl}</projectUrl>");
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
                        if (Framework.Major < 5)
                            FrameworkName = ".NETFramework";
                        else
                            FrameworkName = "net";
                        break;
                }

                if (nuspec.FrameworkList.Count > 0 && nuspec.PackageDependencies.Count > 0)
                {
                    writer.WriteLine($"      <group targetFramework=\"{FrameworkName}{Framework.Major}.{Framework.Minor}\">");

                    foreach (PackageReference Item in nuspec.PackageDependencies)
                        writer.WriteLine($"        <dependency id=\"{Item.Name}\" version=\"{Item.Version}\"/>");

                    writer.WriteLine($"      </group>");
                }
                else
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

        private static string GetDebugTitle(bool isDebug)
        {
            return isDebug ? " (Debug)" : string.Empty;
        }

        private static void ConvertIcoToPng(string nuspecPath, ref string fileName)
        {
            if (Path.GetExtension(fileName) != ".ico")
                return;

            string NuspecFolder = Path.GetDirectoryName(nuspecPath)!;
            string IconFileName = Path.Combine(NuspecFolder, Path.GetFileName(fileName));
            string PngFileName = Path.ChangeExtension(IconFileName, ".png");

            if (!File.Exists(IconFileName))
            {
                if (File.Exists(PngFileName))
                    fileName = Path.ChangeExtension(fileName, ".png");

                return;
            }

            ConvertIcoToPng(IconFileName, PngFileName);

            File.Delete(IconFileName);

            fileName = Path.ChangeExtension(fileName, ".png");
        }

        private static void ConvertIcoToPng(string iconFileName, string pngFileName)
        {
            using FileStream IconStream = new FileStream(iconFileName, FileMode.Open, FileAccess.Read);
            using FileStream PngStream = new FileStream(pngFileName, FileMode.Create, FileAccess.Write);

            IconBitmapDecoder Decoder = new IconBitmapDecoder(IconStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
            PngBitmapEncoder Encoder = new PngBitmapEncoder();
            Encoder.Frames.Add(Decoder.Frames[0]);
            Encoder.Save(PngStream);
        }
    }
}
