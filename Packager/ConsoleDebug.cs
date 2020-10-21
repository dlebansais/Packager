﻿namespace Packager
{
    using System;

    /// <summary>
    /// Writes text to the console.
    /// </summary>
    public static class ConsoleDebug
    {
        /// <summary>
        /// Writes text to the console.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <param name="isError">True if the text to write is an error message.</param>
        public static void Write(string text, bool isError = false)
        {
            ConsoleColor OldColor = Console.ForegroundColor;

            if (isError)
                Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(text);

            if (isError)
                Console.ForegroundColor = OldColor;
        }
    }
}
