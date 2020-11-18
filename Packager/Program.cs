namespace Packager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Generates a .nuspec file based on project .csproj content.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="arguments">Command-line arguments.</param>
        /// <returns>-1 in case of error; otherwise 0.</returns>
        public static int Main(string[] arguments)
        {
            bool IsDebug = arguments != null && arguments.Length > 0 && arguments[0] == "--debug";

            LoadSolutionAndProjectList(out List<Project> ProjectList);
            FilterProcessedProjects(ProjectList, out List<Project> ProcessedProjectList, out bool HasErrors);

            foreach (Project Project in ProcessedProjectList)
                AnalyseProject(Project, IsDebug);

            return HasErrors ? -1 : 0;
        }

        private static void LoadSolutionAndProjectList(out List<Project> projectList)
        {
            projectList = new List<Project>();

            string CurrentDirectory = Environment.CurrentDirectory;
            ConsoleDebug.Write($"Current Directory: {CurrentDirectory}");

            string[] Files = Directory.GetFiles(CurrentDirectory, "*.sln");
            ConsoleDebug.Write($"Found {Files.Length} solution file(s)");

            foreach (string SolutionFileName in Files)
            {
                ConsoleDebug.Write($"  Solution file: {SolutionFileName}");
                Solution NewSolution = new Solution(SolutionFileName);

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

        private static void AnalyseProject(Project project, bool isDebug)
        {
            ConsoleDebug.Write($"  Processing: {project.RelativePath}");

            InitializeFile(project, isDebug, out string NuspecPath);

            using FileStream Stream = new FileStream(NuspecPath, FileMode.Append, FileAccess.Write);
            using StreamWriter Writer = new StreamWriter(Stream, Encoding.UTF8);

            Writer.WriteLine("<package>");
            Writer.WriteLine("  <metadata>");

            WriteMiscellaneousInfo(Writer, project, isDebug);
            WriteDependencies(Writer, project);
            WriteExtraContentFiles(Writer, isDebug);

            Writer.WriteLine("  </metadata>");
            Writer.Write("</package>");
        }

        private static void InitializeFile(Project project, bool isDebug, out string nuspecPath)
        {
            string DebugSuffix = GetDebugSuffix(isDebug);
            string NugetDirectory = isDebug ? "nuget-debug" : "nuget";

            if (!Directory.Exists(NugetDirectory))
                Directory.CreateDirectory(NugetDirectory);

            string NuspecFileName = $"{project.ProjectName}{DebugSuffix}.nuspec";
            nuspecPath = Path.Combine(NugetDirectory, NuspecFileName);

            using FileStream FirstStream = new FileStream(nuspecPath, FileMode.Create, FileAccess.Write);
            using StreamWriter FirstWriter = new StreamWriter(FirstStream, Encoding.ASCII);
            FirstWriter.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        }

        private static void WriteMiscellaneousInfo(StreamWriter writer, Project project, bool isDebug)
        {
            string DebugSuffix = GetDebugSuffix(isDebug);
            writer.WriteLine($"    <id>{project.ProjectName}{DebugSuffix}</id>");
            writer.WriteLine($"    <version>{project.Version}</version>");

            if (project.Author.Length > 0)
                writer.WriteLine($"    <authors>{project.Author}</authors>");

            if (project.Description.Length > 0)
                writer.WriteLine($"    <description>{project.Description}</description>");

            if (project.Copyright.Length > 0)
                writer.WriteLine($"    <copyright>{project.Copyright}</copyright>");

            writer.WriteLine($"    <repository type=\"git\" url=\"{project.RepositoryUrl}\"/>");
        }

        private static void WriteDependencies(StreamWriter writer, Project project)
        {
            writer.WriteLine("    <dependencies>");

            foreach (Framework Framework in project.FrameworkList)
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

        private static string GetDebugSuffix(bool isDebug)
        {
            return isDebug ? "-Debug" : string.Empty;
        }
    }
}
