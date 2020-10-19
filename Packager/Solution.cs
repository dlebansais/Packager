namespace Packager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Reads and parses a solution file.
    /// </summary>
    internal class Solution
    {
        /*
         * Internal class SolutionParser
         * Name: Microsoft.Build.Construction.SolutionParser
         * Assembly: Microsoft.Build, Version=4.0.0.0
         */

        private static readonly Type SolutionParserType = Type.GetType("Microsoft.Build.Construction.SolutionParser, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
        private static readonly PropertyInfo SolutionParserReader = SolutionParserType.GetProperty("SolutionReader", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly PropertyInfo SolutionParserProjects = SolutionParserType.GetProperty("Projects", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo SolutionParserParseSolution = SolutionParserType.GetMethod("ParseSolution", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Initializes a new instance of the <see cref="Solution"/> class.
        /// </summary>
        /// <param name="solutionFileName">The solution file name.</param>
        public Solution(string solutionFileName)
        {
            SolutionFileName = solutionFileName;

            ConstructorInfo[] Constructors = SolutionParserType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            var solutionParser = Constructors[0].Invoke(null);

            using StreamReader streamReader = new StreamReader(solutionFileName);
            SolutionParserReader.SetValue(solutionParser, streamReader, null);
            SolutionParserParseSolution.Invoke(solutionParser, null);

            var array = (Array)SolutionParserProjects.GetValue(solutionParser, null);
            for (int i = 0; i < array.Length; i++)
            {
                Project NewProject = new Project(array.GetValue(i));
                ProjectList.Add(NewProject);
            }
        }

        /// <summary>
        /// Gets the solution file name.
        /// </summary>
        public string SolutionFileName { get; }

        /// <summary>
        /// Gets the list of projects in the solution.
        /// </summary>
        public List<Project> ProjectList { get; private set; } = new List<Project>();
    }
}
