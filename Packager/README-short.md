# Packager

A tool to package a project before deployment.

## Usage

This tool is intended to be used in the context of Continuous Integration (CI). In your CI script:

+ Download the packager. Ex: `nuget install Packager -DependencyVersion Highest -OutputDirectory packages`
+ Run it in the root directory, this will generate a `.nuspec` file in the `nuget` subdirectory.
+ If desired, repeat with `--debug` to generate a `.nuspec` file that include symbols in the `nuget-debug` directory.
+ Run `nuget pack` command(s) to generate package(s).
