﻿namespace Packager;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    /// Gets the framework that represents any framework.
    /// </summary>
    public static Framework AnyFramework { get; } = new(string.Empty, FrameworkType.None, 0, 0, FrameworkMoniker.none, 0, 0);

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
        FrameworkList = [];
        PackageDependencies = [];
        PackageIcon = string.Empty;
        PackageLicenseExpression = string.Empty;
        PackageReadmeFile = string.Empty;
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
    /// <param name="packageIcon">The nuspec package icon.</param>
    /// <param name="packageLicense">The nuspec package license.</param>
    /// <param name="packageReadmeFile">The nuspec package readme file.</param>
    public Nuspec(string name,
                  string relativePath,
                  string version,
                  string author,
                  string description,
                  string copyright,
                  Uri repositoryUrl,
                  string applicationIcon,
                  IReadOnlyList<Framework> frameworkList,
                  Dictionary<Framework, PackageReferenceList> packageDependencies,
                  string packageIcon,
                  string packageLicense,
                  string packageReadmeFile)
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
        PackageIcon = packageIcon;
        PackageLicenseExpression = packageLicense;
        PackageReadmeFile = packageReadmeFile;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Nuspec"/> class from a project.
    /// </summary>
    /// <param name="isDebug">If true, gets debug dependencies; otherwise, get release dependencies.</param>
    /// <param name="project">The project.</param>
    /// <returns>The created instance.</returns>
    public static Nuspec FromProject(bool isDebug, Project project)
    {
        Uri ParsedUrl = Contract.AssertNotNull(project.RepositoryUrl);
        Dictionary<Framework, PackageReferenceList> PackageDependencies = GetPackageDependencies(isDebug, project);

        return new Nuspec(project.ProjectName,
                          project.RelativePath,
                          project.Version,
                          project.Author,
                          project.Description,
                          project.Copyright,
                          ParsedUrl,
                          project.ApplicationIcon,
                          project.FrameworkList,
                          PackageDependencies,
                          project.PackageIcon,
                          project.PackageLicenseExpression,
                          project.PackageReadmeFile);
    }

    /// <summary>
    /// Gets package dependencies in a project.
    /// </summary>
    /// <param name="isDebug">If true, gets debug dependencies; otherwise, get release dependencies.</param>
    /// <param name="project">The project.</param>
    public static Dictionary<Framework, PackageReferenceList> GetPackageDependencies(bool isDebug, Project project)
    {
        Dictionary<Framework, PackageReferenceList> Result = [];

        foreach (PackageReference Item in project.PackageReferenceList)
            if (!Item.IsAllPrivateAssets)
            {
                if ((isDebug && (Item.Condition == "'$(Configuration)|$(Platform)'=='Debug|x64'" || Item.Condition == "'$(Configuration)'=='Debug'")) ||
                    (!isDebug && (Item.Condition == "'$(Configuration)|$(Platform)'!='Debug|x64'" || Item.Condition == "'$(Configuration)'!='Debug'")))
                {
                    AddPackageReference(AnyFramework, Item, Result);
                }
                else if (TryParseFrameworkCondition(Item, project, out Framework? specificFramework))
                {
                    AddPackageReference(specificFramework, Item, Result);
                }
                else if (Item.Condition == string.Empty)
                {
                    AddPackageReference(AnyFramework, Item, Result);
                }
            }

        return Result;
    }

    private static bool TryParseFrameworkCondition(PackageReference packageReference, Project project, [MaybeNullWhen(false)] out Framework framework)
    {
        foreach (Framework Item in project.FrameworkList)
        {
            string ExpectedCondition = $"'$(TargetFramework)'=='{Item.Name}'";

            if (packageReference.Condition == ExpectedCondition)
            {
                framework = Item;
                return true;
            }
        }

        framework = null;
        return false;
    }

    private static void AddPackageReference(Framework framework, PackageReference packageReference, Dictionary<Framework, PackageReferenceList> packageDependencies)
    {
        if (packageDependencies.TryGetValue(framework, out PackageReferenceList? ExistingList))
        {
            ExistingList.Add(packageReference);
        }
        else
        {
            packageDependencies[framework] = [packageReference];
        }
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
    public Dictionary<Framework, PackageReferenceList> PackageDependencies { get; init; }

    /// <summary>
    /// Gets the nuspec package icon.
    /// </summary>
    public string PackageIcon { get; init; }

    /// <summary>
    /// Gets the nuspec package license.
    /// </summary>
    public string PackageLicenseExpression { get; init; }

    /// <summary>
    /// Gets the nuspec package readme file.
    /// </summary>
    public string PackageReadmeFile { get; init; }
    #endregion
}
