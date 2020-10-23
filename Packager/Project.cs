namespace Packager
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Xml.Linq;

    /// <summary>
    /// Reads and parses a project file.
    /// </summary>
    [DebuggerDisplay("{ProjectName}, {RelativePath}, {ProjectGuid}")]
    internal class Project
    {
#pragma warning disable CA1810 // Initialize reference type static fields inline
        static Project()
#pragma warning restore CA1810 // Initialize reference type static fields inline
        {
            ConsoleDebug.Write("Loading ProjectInSolution assembly...");

            ProjectInSolutionType = Type.GetType("Microsoft.Build.Construction.ProjectInSolution, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            ProjectInSolutionProjectName = ProjectInSolutionType.GetProperty("ProjectName", BindingFlags.NonPublic | BindingFlags.Instance);
            ProjectInSolutionRelativePath = ProjectInSolutionType.GetProperty("RelativePath", BindingFlags.NonPublic | BindingFlags.Instance);
            ProjectInSolutionProjectGuid = ProjectInSolutionType.GetProperty("ProjectGuid", BindingFlags.NonPublic | BindingFlags.Instance);
            ProjectInSolutionProjectType = ProjectInSolutionType.GetProperty("ProjectType", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private static readonly Type ProjectInSolutionType;
        private static readonly PropertyInfo ProjectInSolutionProjectName;
        private static readonly PropertyInfo ProjectInSolutionRelativePath;
        private static readonly PropertyInfo ProjectInSolutionProjectGuid;
        private static readonly PropertyInfo ProjectInSolutionProjectType;

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <param name="solutionProject">The project as loaded from a solution.</param>
        public Project(object solutionProject)
        {
            ProjectName = (string)ProjectInSolutionProjectName.GetValue(solutionProject, null);
            RelativePath = (string)ProjectInSolutionRelativePath.GetValue(solutionProject, null);
            ProjectGuid = (string)ProjectInSolutionProjectGuid.GetValue(solutionProject, null);
            ProjectType = ProjectInSolutionProjectType.GetValue(solutionProject, null).ToString();
        }

        /// <summary>
        /// Gets the project name.
        /// </summary>
        public string ProjectName { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the project relative path.
        /// </summary>
        public string RelativePath { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the project GUID.
        /// </summary>
        public string ProjectGuid { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the project type.
        /// </summary>
        public string ProjectType { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the project version.
        /// </summary>
        public string Version { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the project has a version.
        /// </summary>
        public bool HasVersion { get { return Version.Length > 0; } }

        /// <summary>
        /// Gets a value indicating whether the project has a valid assembly version.
        /// </summary>
        public bool IsAssemblyVersionValid { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the project has a valid file version.
        /// </summary>
        public bool IsFileVersionValid { get; private set; }

        /// <summary>
        /// Gets the project author.
        /// </summary>
        public string Author { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the project description.
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the project copyright text.
        /// </summary>
        public string Copyright { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the project repository URL.
        /// </summary>
        public string RepositoryUrl { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the project has a repository URL.
        /// </summary>
        public bool HasRepositoryUrl { get { return RepositoryUrl.Length > 0; } }

        /// <summary>
        /// Gets the list of parsed project frameworks.
        /// </summary>
        public List<Framework> FrameworkList { get; } = new List<Framework>();

        /// <summary>
        /// Gets a value indicating whether the project has target frameworks.
        /// </summary>
        public bool HasTargetFrameworks { get { return FrameworkList.Count > 0; } }

        /// <summary>
        /// Parses a loaded project.
        /// </summary>
        /// <param name="hasErrors">Set to true upon return if an error was found.</param>
        public void Parse(ref bool hasErrors)
        {
            XElement Root = XElement.Load(RelativePath);
            Version = string.Empty;
            string AssemblyVersion = string.Empty;
            string FileVersion = string.Empty;

            foreach (XElement ProjectElement in Root.Descendants("PropertyGroup"))
            {
                XElement? VersionElement = ProjectElement.Element("Version");
                if (VersionElement != null)
                {
                    Version = VersionElement.Value;
                    ConsoleDebug.Write($"    Version: {Version}");
                }

                XElement? AssemblyVersionElement = ProjectElement.Element("AssemblyVersion");
                if (AssemblyVersionElement != null)
                {
                    AssemblyVersion = AssemblyVersionElement.Value;
                    ConsoleDebug.Write($"    AssemblyVersion: {AssemblyVersion}");
                }

                XElement? FileVersionElement = ProjectElement.Element("FileVersion");
                if (FileVersionElement != null)
                {
                    FileVersion = FileVersionElement.Value;
                    ConsoleDebug.Write($"    FileVersion: {FileVersion}");
                }

                XElement? AuthorElement = ProjectElement.Element("Authors");
                if (AuthorElement != null)
                    Author = AuthorElement.Value;

                XElement? DescriptionElement = ProjectElement.Element("Description");
                if (DescriptionElement != null)
                    Description = DescriptionElement.Value;

                XElement? CopyrightElement = ProjectElement.Element("Copyright");
                if (CopyrightElement != null)
                    Copyright = CopyrightElement.Value;

                XElement? RepositoryUrlElement = ProjectElement.Element("RepositoryUrl");
                if (RepositoryUrlElement != null)
                {
                    RepositoryUrl = RepositoryUrlElement.Value;
                    ConsoleDebug.Write($"    RepositoryUrl: {RepositoryUrl}");
                }

                XElement? TargetFrameworkElement = ProjectElement.Element("TargetFramework");
                if (TargetFrameworkElement != null)
                {
                    TargetFrameworks = TargetFrameworkElement.Value;
                    ConsoleDebug.Write($"    TargetFramework: {TargetFrameworks}");
                }
                else
                {
                    XElement? TargetFrameworksElement = ProjectElement.Element("TargetFrameworks");
                    if (TargetFrameworksElement != null)
                    {
                        TargetFrameworks = TargetFrameworksElement.Value;
                        ConsoleDebug.Write($"    TargetFrameworks: {TargetFrameworks}");
                    }
                }
            }

            if (HasVersion)
            {
                IsAssemblyVersionValid = AssemblyVersion.StartsWith(Version, StringComparison.InvariantCulture);
                if (!IsAssemblyVersionValid)
                {
                    hasErrors = true;
                    ConsoleDebug.Write($"    ERROR: {AssemblyVersion} not compatible with {Version}", true);
                }

                IsFileVersionValid = FileVersion.StartsWith(Version, StringComparison.InvariantCulture);
                if (!IsFileVersionValid)
                {
                    hasErrors = true;
                    ConsoleDebug.Write($"    ERROR: {FileVersion} not compatible with {Version}", true);
                }
            }
            else
                ConsoleDebug.Write("    Ignored because no version");

            if (TargetFrameworks.Length > 0)
            {
                string[] Frameworks = TargetFrameworks.Split(';');
                foreach (string Framework in Frameworks)
                {
                    string NetStandardPattern = "netstandard";
                    string NetCorePattern = "netcoreapp";
                    string NetFrameworkPattern = "net";

                    Framework? NewFramework = null;
                    int Major;
                    int Minor;

                    if (Framework.StartsWith(NetStandardPattern, StringComparison.InvariantCulture) && ParseNetVersion(Framework.Substring(NetStandardPattern.Length), out Major, out Minor))
                        NewFramework = new Framework(FrameworkType.NetStandard, Major, Minor);
                    else if (Framework.StartsWith(NetCorePattern, StringComparison.InvariantCulture) && ParseNetVersion(Framework.Substring(NetCorePattern.Length), out Major, out Minor))
                        NewFramework = new Framework(FrameworkType.NetCore, Major, Minor);
                    else if (Framework.StartsWith(NetFrameworkPattern, StringComparison.InvariantCulture) && ParseNetVersion(Framework.Substring(NetFrameworkPattern.Length), out Major, out Minor))
                        NewFramework = new Framework(FrameworkType.NetFramework, Major, Minor);

                    if (NewFramework != null)
                        FrameworkList.Add(NewFramework);
                }
            }
        }

        private static bool ParseNetVersion(string text, out int major, out int minor)
        {
            major = -1;
            minor = -1;

            string[] Versions = text.Split('.');
            if (Versions.Length == 2)
            {
                if (int.TryParse(Versions[0], out major) && int.TryParse(Versions[1], out minor))
                    return true;
            }
            else if (Versions.Length == 1)
            {
                string Version = Versions[0];
                if (Version.Length > 1 && int.TryParse(Version.Substring(0, 1), out major) && int.TryParse(Version.Substring(1), out minor))
                    return true;
            }

            return false;
        }

        private string TargetFrameworks = string.Empty;
    }
}
