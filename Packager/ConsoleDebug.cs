namespace Packager
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
        public static void Write(string text)
        {
            Console.WriteLine(text);
        }
    }
}
