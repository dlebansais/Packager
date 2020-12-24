namespace Packager
{
    using System;

    /// <summary>
    /// Represent a class that can parse command line arguments for this program.
    /// </summary>
    internal static class CommandLineOptions
    {
        /// <summary>
        /// Parses the --debug command line argument.
        /// </summary>
        /// <param name="arguments">Command line arguments.</param>
        /// <param name="isDebug">True upon return if --debug was parsed.</param>
        public static void ParseIsDebug(string[] arguments, out bool isDebug)
        {
            isDebug = false;

            foreach (string Argument in arguments)
                if (Argument == "--debug")
                {
                    isDebug = true;
                    ConsoleDebug.Write("Debug output selected");
                    break;
                }
        }

        /// <summary>
        /// Parses the --merge command line argument.
        /// </summary>
        /// <param name="arguments">Command line arguments.</param>
        /// <param name="isMerge">True upon return if --merge was parsed.</param>
        /// <param name="mergeName">The merge name to use.</param>
        public static void ParseIsMerge(string[] arguments, out bool isMerge, out string mergeName)
        {
            isMerge = false;
            mergeName = string.Empty;

            string Pattern = "--merge";

            foreach (string Argument in arguments)
                if (Argument.StartsWith(Pattern, StringComparison.InvariantCulture))
                {
                    isMerge = true;

                    if (Argument.Length > Pattern.Length && Argument[Pattern.Length] == ':')
                    {
                        mergeName = Argument.Substring(Pattern.Length + 1);
                        ConsoleDebug.Write($"Merged output selected: '{mergeName}'");
                    }
                    else
                        ConsoleDebug.Write("Merged output selected (no name)");

                    break;
                }
        }

        /// <summary>
        /// Parses the --description command line argument.
        /// </summary>
        /// <param name="arguments">Command line arguments.</param>
        /// <param name="nugetDescription">The description to use upon return if parsed.</param>
        public static void ParseDescription(string[] arguments, out string nugetDescription)
        {
            nugetDescription = string.Empty;

            string Pattern = "--description:";

            foreach (string Argument in arguments)
                if (Argument.StartsWith(Pattern, StringComparison.InvariantCulture))
                {
                    nugetDescription = Argument.Substring(Pattern.Length);
                    ConsoleDebug.Write($"Output description: '{nugetDescription}'");
                    break;
                }
        }
    }
}
