namespace Packager;

using System;
using System.Diagnostics;
using Contracts;

/// <summary>
/// Writes text to the console.
/// </summary>
public static partial class ConsoleDebug
{
    /// <summary>
    /// Writes text to the console.
    /// </summary>
    /// <param name="text">The text to write.</param>
    /// <param name="isError">True if the text to write is an error message.</param>
    [RequireNotNull(nameof(text))]
    private static void WriteVerified(string text, bool isError = false)
    {
        ConsoleColor OldColor = Console.ForegroundColor;

        if (isError)
            Console.ForegroundColor = ConsoleColor.Red;

        Console.WriteLine(text);
        Debug.WriteLine(text);

        if (isError)
            Console.ForegroundColor = OldColor;
    }
}
