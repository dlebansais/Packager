﻿namespace Packager;

using System;
using System.Globalization;
using Contracts;
using McMaster.Extensions.CommandLineUtils;

/// <summary>
/// Generates a .nuspec file based on project .csproj content.
/// </summary>
[Command(ExtendedHelpText = @"
Generate the .nuspec file from .csproj project(s) to package a solution before deployment.
Use the first solution file in the current directory as input.
Use either the first project from the solution, or merge all projects into one .nuspec file.
")]
internal partial class Program
{
    /// <summary>
    /// Program entry point.
    /// </summary>
    /// <param name="arguments">Command-line arguments.</param>
    /// <returns>-1 in case of error; otherwise 0.</returns>
    public static int Main(string[] arguments) => RunAndSetResult(CommandLineApplication.Execute<Program>(arguments));

    [Option(Description = "Create a file intended for Debug configurations.", ShortName = "d", LongName = "debug")]
    private bool IsDebug { get; set; }

    [Option(Description = "The package is for an analyzer.", ShortName = "a", LongName = "analyzer")]
    private bool IsAnalyzer { get; set; }

    [Option(CommandOptionType.SingleOrNoValue, Description = "Merge into one file. If no name is specified, use the solution file name.", ShortName = "m", LongName = "merge", ValueName = "Merged file name")]
    private (bool HasValue, string Name) Merge { get; set; }

    [Option(CommandOptionType.SingleValue, Description = "The description to insert in the output.", ShortName = "n", LongName = "description", ValueName = "Description Text")]
    private string NuspecDescription { get; set; } = string.Empty;

    [Option(CommandOptionType.SingleValue, Description = "Relative path to to icon to insert in the output.", ShortName = "i", LongName = "icon", ValueName = "Icon Path")]
    private string NuspecIcon { get; set; } = string.Empty;

    [Option(CommandOptionType.SingleValue, Description = "A prefix in front of the output package file name.", ShortName = "p", LongName = "prefix", ValueName = "Package prefix")]
    private string NuspecPrefix { get; set; } = string.Empty;

#pragma warning disable IDE0060 // Remove unused parameter
    private static int RunAndSetResult(int ignored) => ExecuteResult;
#pragma warning restore IDE0060 // Remove unused parameter

    private static int ExecuteResult = -1;

#pragma warning disable IDE0051 // Remove unused private members: this member is called through reflection from the McMaster.Extensions.CommandLineUtils tool.
    private void OnExecute()
#pragma warning restore IDE0051 // Remove unused private members
    {
        try
        {
            ShowCommandLineArguments();
            ExecuteProgram(IsDebug, IsAnalyzer, Merge.HasValue, Merge.Name, NuspecDescription, NuspecIcon, NuspecPrefix, out bool HasErrors);

            ExecuteResult = HasErrors ? -1 : 0;
        }
        catch (Exception e)
        {
            PrintException(e);
        }
    }

    private void ShowCommandLineArguments()
    {
        if (IsDebug)
            ConsoleDebug.Write("Debug output selected.");

        if (IsAnalyzer)
            ConsoleDebug.Write("The package is for an analyzer.");

        if (Merge.HasValue)
            if (Merge.Name is not null && Merge.Name.Length > 0)
                ConsoleDebug.Write($"Merged output selected: '{Merge.Name}'.");
            else
                ConsoleDebug.Write("Merged output selected (no name).");

        if (NuspecDescription.Length > 0)
            ConsoleDebug.Write($"Output description: '{NuspecDescription}'.");

        if (NuspecPrefix.Length > 0)
            ConsoleDebug.Write($"Prefix: '{NuspecPrefix}'.");
    }

    private static void PrintException(Exception e)
    {
        Exception? CurrentException = e;

        do
        {
            ConsoleDebug.Write("***************");
            ConsoleDebug.Write(CurrentException.Message);

            // If CurrentException.StackTrace is null, StackTrace is set to string.Empty.
            string StackTrace = Contract.AssertNotNull(Convert.ToString((object?)CurrentException.StackTrace, CultureInfo.InvariantCulture));
            ConsoleDebug.Write(StackTrace);

            CurrentException = CurrentException.InnerException;
        }
        while (CurrentException is not null);
    }
}
