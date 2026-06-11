namespace Packager;

using System;
using System.Diagnostics;
using Contracts;

/// <summary>
/// Writes text to the console.
/// </summary>
internal static partial class ConsoleDebug
{
    /// <summary>
    /// Writes text to the console.
    /// </summary>
    /// <param name="text">The text to write.</param>
    /// <param name="isError">True if the text to write is an error message.</param>
    [RequireNotNull(nameof(text))]
    private static void WriteVerified(string text, bool isError = false)
    {
        if (isError)
        {
            ConsoleColor OldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.Error.WriteLine(text);
            Debug.WriteLine(text);

            Console.ForegroundColor = OldColor;
        }
        else
        {
            Console.WriteLine(text);
            Debug.WriteLine(text);
        }
    }
}
