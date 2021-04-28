namespace Packager
{
    using System;
    using McMaster.Extensions.CommandLineUtils;

    /// <summary>
    /// Generates a .nuspec file based on project .csproj content.
    /// </summary>
    [Command(ExtendedHelpText = @"
Generate the .nuspec file from .csproj project(s) to package a solution before deployment.
Use the first solution file in the current directory as input.
Use either the first project from the solution, or merge all projects into one .nuspec file.
")]
    public partial class Program
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="arguments">Command-line arguments.</param>
        /// <returns>-1 in case of error; otherwise 0.</returns>
        public static int Main(string[] arguments) => RunAndSetResult(CommandLineApplication.Execute<Program>(arguments));

        [Option(Description = "Create a file intended for Debug configurations.", ShortName = "d", LongName = "debug")]
        private bool IsDebug { get; set; }

        [Option(CommandOptionType.SingleOrNoValue, Description = "Merge into one file. If no name is specified, use the solution file name.", ShortName = "m", LongName = "merge", ValueName = "Merged file name")]
        private (bool HasValue, string Name) Merge { get; set; }

        [Option(CommandOptionType.SingleValue, Description = "The description to insert in the output.", ShortName = "n", LongName = "description", ValueName = "Description Text")]
        private string NuspecDescription { get; set; } = string.Empty;

        [Option(CommandOptionType.SingleValue, Description = "Relative path to to icon to insert in the output.", ShortName = "i", LongName = "icon", ValueName = "Icon Path")]
        private string NuspecIcon { get; set; } = string.Empty;

        private static int RunAndSetResult(int ignored)
        {
            return ExecuteResult;
        }

        private static int ExecuteResult = -1;

        private void OnExecute()
        {
            try
            {
                ShowCommandLineArguments();
                ExecuteProgram(IsDebug, Merge.HasValue, Merge.Name, NuspecDescription, NuspecIcon, out bool HasErrors);

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
                ConsoleDebug.Write("Debug output selected");

            if (Merge.HasValue)
                if (Merge.Name != null && Merge.Name.Length > 0)
                    ConsoleDebug.Write($"Merged output selected: '{Merge.Name}'");
                else
                    ConsoleDebug.Write("Merged output selected (no name)");

            if (NuspecDescription.Length > 0)
                ConsoleDebug.Write($"Output description: '{NuspecDescription}'");
        }

        private static void PrintException(Exception e)
        {
            Exception? CurrentException = e;

            do
            {
                ConsoleDebug.Write("***************");
                ConsoleDebug.Write(CurrentException.Message);

                string? StackTrace = CurrentException.StackTrace;
                if (StackTrace != null)
                    ConsoleDebug.Write(StackTrace);

                CurrentException = CurrentException.InnerException;
            }
            while (CurrentException != null);
        }
    }
}
