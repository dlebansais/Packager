# Packager

A tool to package a project before deployment.

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

