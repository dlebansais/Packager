# Packager

A tool to package a project before deployment.

[![Build status](https://ci.appveyor.com/api/projects/status/bo5s4x480tu8dr5x?svg=true)](https://ci.appveyor.com/project/dlebansais/packager) [![CodeFactor](https://www.codefactor.io/repository/github/dlebansais/packager/badge)](https://www.codefactor.io/repository/github/dlebansais/packager)

## Usage

This tool is intended to be used in the context of Continuous Integration (CI). In your CI script:

+ Download the packager. Ex: `nuget install Packager -DependencyVersion Highest -OutputDirectory packages`
+ Run it in the root directory, this will generate a `.nuspec` file in the `nuget` subdirectory.
+ If desired, repeat with `--debug` to generate a `.nuspec` file that include symbols in the `nuget-debug` directory.
+ Run `nuget pack` command(s) to generate package(s).

## Getting the tool version

If you choose to download the latest version of the tool, the following PowerShell script will find the version number and run it:

````
- ps: $folder = Get-ChildItem -Path packages/Packager.* -Name | Out-String    # Get the installation folder
- ps: $firstline = ($folder -split '\r\n')[0]                                 # Get rid of carriage-return
- ps: $fullpath = ".\packages\$firstline\lib\net48\Packager.exe"              # Build full path
- ps: '& $fullpath --debug'                                                   # Execute the packager with --debug
````

## Suuported tags

The packager supports the following tags in the project file:

| .csproj tag                | Description                                          | .nuspec tag   |
| -------------------------- | ---------------------------------------------------- | ------------- |
| `Authors`                  | The package authors.                                 | `authors`     |
| `Description`              | The package description.                             | `description` |
| `Copyright`                | The package copyright.                               | `copyright`   |
| `RepositoryUrl`            | The project repository.                              | `projectUrl`  |
| `PackageIcon`              | The package icon (can be overwritten with `--icon`). | `icon`        |
| `PackageLicenseExpression` | The package license.                                 | `license`     |
| `PackageReadmeFile`        | The package readme.                                  | `readme`      |

## Dependencies

The packager will declare as many dependencies as there are frameworks in the `<Framework>` or `<Frameworks>` tags.

There is support for framework-specific depedencies, debug/release dependencies, and for excluding some dependencies.

To declare a dependency, specify a `PackageReference` in the project file, and use one the options listed below, from highest to lowest priority.

### Private assets

If `PrivateAssets` is `all` the reference will not be included. This is typically the case for analyzers.

If you still want to include the reference, you can use `compile;runtime;contentFiles;build;buildMultitargeting;buildTransitive;analyzers;native` which is the same as `all` for all purpose but this comparison.

### Conditional, for a release build

With a condition that is either `"'$(Configuration)'!='Debug'"` or `"'$(Configuration)|$(Platform)'!='Debug|x64'"` depending if you're generating Any CPU or x64 binaries.

### Conditional, for a debug build

With a condition that is either `"'$(Configuration)'=='Debug'"` or `"'$(Configuration)|$(Platform)'=='Debug|x64'"` depending if you're generating Any CPU or x64 binaries.

### Conditional, for a specific framework

With a condition of the type `"'$(TargetFramework)'=='<Framework>'"` where `<Framework>` is one of the target frameworks in the project, the dependency will be added only for that frameork in the package.

To specify a dependency for the .NET Framework 4.7.2 for instance, use `"'$(TargetFramework)'=='net472'"`.

If a dependency is the same for several frameworks you must repeat the `PackageReference` tag for each framework, with the appropriate condition. For example, the following doesn't work:

```csproj
<ItemGroup>
    <PackageReference Include="Microsoft.Build" Version="17.11.4" Condition="'$(TargetFramework)'=='net7.0-windows7.0' Or '$(TargetFramework)'=='net8.0-windows7.0'" />
    <PackageReference Include="Microsoft.Build" Version="17.12.6" Condition="'$(TargetFramework)'=='net9.0-windows7.0'" />
</ItemGroup>
```
But this does:
```csproj
<ItemGroup>
    <PackageReference Include="Microsoft.Build" Version="17.11.4" Condition="'$(TargetFramework)'=='net7.0-windows7.0'" />
    <PackageReference Include="Microsoft.Build" Version="17.11.4" Condition="'$(TargetFramework)'=='net8.0-windows7.0'" />
    <PackageReference Include="Microsoft.Build" Version="17.12.6" Condition="'$(TargetFramework)'=='net9.0-windows7.0'" />
</ItemGroup>
```

### Excluding a dependency

If you want to exclude a dependency to a `PackageReference` that is included in the project, just add a condition that will always be true when you build, such as `"'$(Configuration)'!='None'"`.

The packager use conditions inside the `PackageReference` tag. You can play with conditions in the parent `ItemGroup` to include or exclude stuff differently than the packager.
