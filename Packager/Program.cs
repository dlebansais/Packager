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
        public static void Main(string[] arguments)
        {
            bool IsDebug = arguments != null && arguments.Length > 0 && arguments[0] == "--debug";

            string CurrentDirectory = Environment.CurrentDirectory;
            Console.WriteLine($"Current Directory: {CurrentDirectory}");

            string[] Files = Directory.GetFiles(CurrentDirectory, "*.sln");
            Console.WriteLine($"Found {Files.Length} solution file(s)");

            List<Solution> SolutionList = new List<Solution>();
            List<Project> ProjectList = new List<Project>();
            List<Project> ProcessedProjectList = new List<Project>();

            foreach (string SolutionFileName in Files)
            {
                Console.WriteLine($"  Solution file: {SolutionFileName}");
                Solution NewSolution = new Solution(SolutionFileName);

                SolutionList.Add(NewSolution);

                foreach (Project Item in NewSolution.ProjectList)
                {
                    bool IsIgnored = Item.ProjectType != "Unknown";
                    string Operation = IsIgnored ? "Ignored" : "Parsed";

                    Console.WriteLine($"    Project: {Item.ProjectName} ({Operation})");

                    if (!IsIgnored)
                        ProjectList.Add(Item);
                }
            }

            Console.WriteLine($"Found {ProjectList.Count} project file(s)");

            foreach (Project Item in ProjectList)
            {
                Console.WriteLine($"  Project file: {Item.RelativePath}");

                Item.Parse();
                if (Item.HasVersion && Item.IsAssemblyVersionValid && Item.IsFileVersionValid && Item.HasRepositoryUrl && Item.HasTargetFrameworks)
                    ProcessedProjectList.Add(Item);
            }

            Console.WriteLine($"Processing {ProcessedProjectList.Count} project file(s)");

            foreach (Project Item in ProcessedProjectList)
            {
                Console.WriteLine($"  Processing: {Item.RelativePath}");

                string NugetDirectory = "nuget";
                if (!Directory.Exists(NugetDirectory))
                    Directory.CreateDirectory(NugetDirectory);

                string NuspecFileName = $"{Item.ProjectName}-Debug.nuspec";
                string NuspecPath = Path.Combine(NugetDirectory, NuspecFileName);

                using (FileStream FirstStream = new FileStream(NuspecPath, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter FirstWriter = new StreamWriter(FirstStream, Encoding.ASCII))
                    {
                        FirstWriter.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    }
                }

                using FileStream Stream = new FileStream(NuspecPath, FileMode.Append, FileAccess.Write);
                using StreamWriter Writer = new StreamWriter(Stream, Encoding.UTF8);

                Writer.WriteLine("<package>");
                Writer.WriteLine("  <metadata>");

                string DebugPrefix = IsDebug ? "-Debug" : string.Empty;

                Writer.WriteLine($"    <id>{Item.ProjectName}{DebugPrefix}</id>");
                Writer.WriteLine($"    <version>{Item.Version}</version>");

                if (Item.Author.Length > 0)
                    Writer.WriteLine($"    <authors>{Item.Author}</authors>");

                if (Item.Description.Length > 0)
                    Writer.WriteLine($"    <description>{Item.Description}</description>");

                if (Item.Copyright.Length > 0)
                    Writer.WriteLine($"    <copyright>{Item.Copyright}</copyright>");

                Writer.WriteLine($"    <repository type=\"git\" url=\"{Item.RepositoryUrl}\"/>");

                Writer.WriteLine("    <dependencies>");

                foreach (Framework Framework in Item.FrameworkList)
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

                    Writer.WriteLine($"      <group targetFramework=\"{FrameworkName}{Framework.Major}.{Framework.Minor}\"/>");
                }

                Writer.WriteLine("    </dependencies>");
                Writer.WriteLine("  </metadata>");
                Writer.Write("</package>");
            }
        }
    }
}
