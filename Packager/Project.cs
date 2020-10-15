namespace Packager
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Xml.Linq;

    [DebuggerDisplay("{ProjectName}, {RelativePath}, {ProjectGuid}")]
    public class Project
    {
        static readonly Type s_ProjectInSolution;
        static readonly PropertyInfo s_ProjectInSolution_ProjectName;
        static readonly PropertyInfo s_ProjectInSolution_RelativePath;
        static readonly PropertyInfo s_ProjectInSolution_ProjectGuid;
        static readonly PropertyInfo s_ProjectInSolution_ProjectType;

        static Project()
        {
            s_ProjectInSolution = Type.GetType("Microsoft.Build.Construction.ProjectInSolution, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            s_ProjectInSolution_ProjectName = s_ProjectInSolution.GetProperty("ProjectName", BindingFlags.NonPublic | BindingFlags.Instance);
            s_ProjectInSolution_RelativePath = s_ProjectInSolution.GetProperty("RelativePath", BindingFlags.NonPublic | BindingFlags.Instance);
            s_ProjectInSolution_ProjectGuid = s_ProjectInSolution.GetProperty("ProjectGuid", BindingFlags.NonPublic | BindingFlags.Instance);
            s_ProjectInSolution_ProjectType = s_ProjectInSolution.GetProperty("ProjectType", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public Project(object solutionProject)
        {
            ProjectName = (string)s_ProjectInSolution_ProjectName.GetValue(solutionProject, null);
            RelativePath = (string)s_ProjectInSolution_RelativePath.GetValue(solutionProject, null);
            ProjectGuid = (string)s_ProjectInSolution_ProjectGuid.GetValue(solutionProject, null);
            ProjectType = s_ProjectInSolution_ProjectType.GetValue(solutionProject, null).ToString();
        }

        public string ProjectName { get; private set; } = string.Empty;
        public string RelativePath { get; private set; } = string.Empty;
        public string ProjectGuid { get; private set; } = string.Empty;
        public string ProjectType { get; private set; } = string.Empty;
        public string Version { get; private set; } = string.Empty;
        public bool HasVersion { get { return Version.Length > 0; } }
        public bool IsAssemblyVersionValid { get; private set; }
        public bool IsFileVersionValid { get; private set; }
        public string Author { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string Copyright { get; private set; } = string.Empty;
        public string RepositoryUrl { get; private set; } = string.Empty;
        public bool HasRepositoryUrl { get { return RepositoryUrl.Length > 0; } }
        public string TargetFrameworks { get; private set; } = string.Empty;
        public List<Framework> FrameworkList { get; } = new List<Framework>();
        public bool HasTargetFrameworks { get { return FrameworkList.Count > 0; } }

        public void Parse()
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
                    Console.WriteLine($"    Version: {Version}");
                }

                XElement? AssemblyVersionElement = ProjectElement.Element("AssemblyVersion");
                if (AssemblyVersionElement != null)
                {
                    AssemblyVersion = AssemblyVersionElement.Value;
                    Console.WriteLine($"    AssemblyVersion: {AssemblyVersion}");
                }

                XElement? FileVersionElement = ProjectElement.Element("FileVersion");
                if (FileVersionElement != null)
                {
                    FileVersion = FileVersionElement.Value;
                    Console.WriteLine($"    FileVersion: {FileVersion}");
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
                    Console.WriteLine($"    RepositoryUrl: {RepositoryUrl}");
                }

                XElement? TargetFrameworkElement = ProjectElement.Element("TargetFramework");
                if (TargetFrameworkElement != null)
                {
                    TargetFrameworks = TargetFrameworkElement.Value;
                    Console.WriteLine($"    TargetFramework: {TargetFrameworks}");
                }
                else
                {
                    XElement? TargetFrameworksElement = ProjectElement.Element("TargetFrameworks");
                    if (TargetFrameworksElement != null)
                    {
                        TargetFrameworks = TargetFrameworksElement.Value;
                        Console.WriteLine($"    TargetFrameworks: {TargetFrameworks}");
                    }
                }
            }

            if (HasVersion)
            {
                IsAssemblyVersionValid = AssemblyVersion.StartsWith(Version, StringComparison.InvariantCulture);
                if (!IsAssemblyVersionValid)
                    Console.WriteLine($"    ERROR: {AssemblyVersion} not compatible with {Version}");

                IsFileVersionValid = FileVersion.StartsWith(Version, StringComparison.InvariantCulture);
                if (!IsAssemblyVersionValid)
                    Console.WriteLine($"    ERROR: {FileVersion} not compatible with {Version}");
            }
            else
                Console.WriteLine($"    Ignored because no version");

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

                    if (Framework.StartsWith(NetStandardPattern) && ParseNetVersion(Framework.Substring(NetStandardPattern.Length), out Major, out Minor))
                        NewFramework = new Framework(FrameworkType.NetStandard, Major, Minor);
                    else if (Framework.StartsWith(NetCorePattern) && ParseNetVersion(Framework.Substring(NetCorePattern.Length), out Major, out Minor))
                        NewFramework = new Framework(FrameworkType.NetCore, Major, Minor);
                    else if (Framework.StartsWith(NetFrameworkPattern) && ParseNetVersion(Framework.Substring(NetFrameworkPattern.Length), out Major, out Minor))
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
    }
}
