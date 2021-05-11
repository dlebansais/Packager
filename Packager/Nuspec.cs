namespace Packager
{
    using System;
    using System.Collections.Generic;
    using Contracts;
    using SlnExplorer;

    /// <summary>
    /// Represents the content of a nuspec file.
    /// </summary>
    internal class Nuspec
    {
        #region Init
        /// <summary>
        /// Gets the empty nuspec.
        /// </summary>
        public static Nuspec Empty { get; } = new Nuspec();

        /// <summary>
        /// Initializes a new instance of the <see cref="Nuspec"/> class.
        /// </summary>
        private Nuspec()
        {
            Name = string.Empty;
            RelativePath = string.Empty;
            Version = string.Empty;
            Author = string.Empty;
            Description = string.Empty;
            Copyright = string.Empty;
            RepositoryUrl = null;
            ApplicationIcon = string.Empty;
            FrameworkList = new List<Framework>();
            PackageDependencies = new List<PackageReference>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Nuspec"/> class.
        /// </summary>
        /// <param name="name">The nuspec name.</param>
        /// <param name="relativePath">The nuspec relative path.</param>
        /// <param name="version">The nuspec version.</param>
        /// <param name="author">The nuspec author.</param>
        /// <param name="description">The nuspec description.</param>
        /// <param name="copyright">The nuspec copyright text.</param>
        /// <param name="repositoryUrl">The nuspec repository URL.</param>
        /// <param name="applicationIcon">The nuspec application icon.</param>
        /// <param name="frameworkList">The list of nuspec frameworks.</param>
        /// <param name="packageDependencies">The list of nuspec package dependencies.</param>
        public Nuspec(string name, string relativePath, string version, string author, string description, string copyright, Uri repositoryUrl, string applicationIcon, IReadOnlyList<Framework> frameworkList, List<PackageReference> packageDependencies)
        {
            Name = name;
            RelativePath = relativePath;
            Version = version;
            Author = author;
            Description = description;
            Copyright = copyright;
            RepositoryUrl = repositoryUrl;
            ApplicationIcon = applicationIcon;
            FrameworkList = frameworkList;
            PackageDependencies = packageDependencies;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Nuspec"/> class from a project.
        /// </summary>
        /// <param name="isDebug">If true, gets debug dependencies; otherwise, get release dependencies.</param>
        /// <param name="project">The project.</param>
        /// <returns>The created instance.</returns>
        public static Nuspec FromProject(bool isDebug, Project project)
        {
            Contract.RequireNotNull(project.RepositoryUrl, out Uri ParsedUrl);

            List<PackageReference> PackageDependencies = GetPackageDependencies(isDebug, project);

            return new Nuspec(project.ProjectName, project.RelativePath, project.Version, project.Author, project.Description, project.Copyright, ParsedUrl, project.ApplicationIcon, project.FrameworkList, PackageDependencies);
        }

        /// <summary>
        /// Gets package dependencies in a project.
        /// </summary>
        /// <param name="isDebug">If true, gets debug dependencies; otherwise, get release dependencies.</param>
        /// <param name="project">The project.</param>
        public static List<PackageReference> GetPackageDependencies(bool isDebug, Project project)
        {
            List<PackageReference> Result = new();

            foreach (PackageReference Item in project.PackageReferenceList)
            {
                if (isDebug && Item.Condition == "'$(Configuration)|$(Platform)'=='Debug|x64'")
                    Result.Add(Item);
                else if (!isDebug && Item.Condition == "'$(Configuration)|$(Platform)'!='Debug|x64'")
                    Result.Add(Item);
            }

            return Result;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the nuspec name.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Gets the nuspec relative path.
        /// </summary>
        public string RelativePath { get; init; }

        /// <summary>
        /// Gets the nuspec version.
        /// </summary>
        public string Version { get; init; }

        /// <summary>
        /// Gets the nuspec author.
        /// </summary>
        public string Author { get; init; }

        /// <summary>
        /// Gets the nuspec description.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// Gets the nuspec copyright text.
        /// </summary>
        public string Copyright { get; init; }

        /// <summary>
        /// Gets the nuspec repository URL.
        /// </summary>
        public Uri? RepositoryUrl { get; init; }

        /// <summary>
        /// Gets the nuspec application icon.
        /// </summary>
        public string ApplicationIcon { get; init; }

        /// <summary>
        /// Gets the list of nuspec frameworks.
        /// </summary>
        public IReadOnlyList<Framework> FrameworkList { get; init; }

        /// <summary>
        /// Gets the list of nuspec package dependencies.
        /// </summary>
        public List<PackageReference> PackageDependencies { get; init; }
        #endregion
    }
}
