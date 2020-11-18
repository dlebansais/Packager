# Packager

A tool to package a project before deployment.

## Usage

This tool is intended to be used in the context of Continuous Integration (CI). In your CI script:

+ Download the packager
+ Run it in the root folder, this will generate a .nuspec file
+ If desired, repeat with --debug to generate a .nuspec file that include symbols
+ Run dotnet pack command(s) to generate package(s)
