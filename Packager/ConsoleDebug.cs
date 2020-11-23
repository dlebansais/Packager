namespace Packager
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Contracts;

    /// <summary>
    /// Writes text to the console.
    /// </summary>
    public static class ConsoleDebug
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int pid);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern bool WriteConsole(IntPtr hConsoleOutput, string lpBuffer, uint nNumberOfCharsToWrite, out uint lpNumberOfCharsWritten, IntPtr lpReserved);

        /// <summary>
        /// Writes text to the console.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <param name="isError">True if the text to write is an error message.</param>
        public static void Write(string text, bool isError = false)
        {
            Contract.RequireNotNull(text, out string Text);

            if (!IsInitialized)
            {
                IsInitialized = true;

                StdOut = GetStdHandle(-11);
                AttachConsole(-1);
            }

            WriteConsole(StdOut, Text, (uint)Text.Length, out _, IntPtr.Zero);

            ConsoleColor OldColor = Console.ForegroundColor;

            if (isError)
                Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(text);
            Debug.WriteLine(text);

            if (isError)
                Console.ForegroundColor = OldColor;
        }

        /// <summary>
        /// Closes the console.
        /// </summary>
        public static void Close()
        {
            // Get command prompt back
            System.Windows.Forms.SendKeys.SendWait("{ENTER}");

            IsInitialized = false;
            StdOut = IntPtr.Zero;
        }

        private static bool IsInitialized;
        private static IntPtr StdOut;
    }
}
