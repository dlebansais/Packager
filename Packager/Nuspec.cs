namespace Packager
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the content of a nuspec file.
    /// </summary>
    internal class Nuspec
    {
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
            FrameworkList = new List<Framework>();
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
        /// <param name="frameworkList">The list of nuspec frameworks.</param>
        public Nuspec(string name, string relativePath, string version, string author, string description, string copyright, Uri repositoryUrl, IReadOnlyList<Framework> frameworkList)
        {
            Name = name;
            RelativePath = relativePath;
            Version = version;
            Author = author;
            Description = description;
            Copyright = copyright;
            RepositoryUrl = repositoryUrl;
            FrameworkList = frameworkList;
        }

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
        /// Gets the list of nuspec frameworks.
        /// </summary>
        public IReadOnlyList<Framework> FrameworkList { get; init; }
    }
}
