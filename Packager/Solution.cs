namespace Packager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Contracts;

    /// <summary>
    /// Reads and parses a solution file.
    /// </summary>
    internal class Solution
    {
        #region Init
#pragma warning disable CA1810 // Initialize reference type static fields inline
        static Solution()
#pragma warning restore CA1810 // Initialize reference type static fields inline
        {
            ConsoleDebug.Write("Loading SolutionParser assembly...");

            SolutionParserType = ReflectionTools.GetProjectInSolutionType("SolutionParser");

            SolutionParserReader = ReflectionTools.GetTypeProperty(SolutionParserType, "SolutionReader");
            SolutionParserProjects = ReflectionTools.GetTypeProperty(SolutionParserType, "Projects");
            SolutionParserParseSolution = ReflectionTools.GetTypeMethod(SolutionParserType, "ParseSolution");
        }

        private static readonly Type SolutionParserType;
        private static readonly PropertyInfo SolutionParserReader;
        private static readonly PropertyInfo SolutionParserProjects;
        private static readonly MethodInfo SolutionParserParseSolution;

        /// <summary>
        /// Initializes a new instance of the <see cref="Solution"/> class.
        /// </summary>
        /// <param name="solutionFileName">The solution file name.</param>
        public Solution(string solutionFileName)
        {
            SolutionFileName = solutionFileName;
            Name = Path.GetFileNameWithoutExtension(solutionFileName);

            ConstructorInfo Constructor = ReflectionTools.GetFirstTypeConstructor(SolutionParserType);
            var solutionParser = Constructor.Invoke(null);

            using StreamReader streamReader = new StreamReader(solutionFileName);
            SolutionParserReader.SetValue(solutionParser, streamReader, null);
            SolutionParserParseSolution.Invoke(solutionParser, null);

            ProjectList = new List<Project>();
            Array array = (Array)ReflectionTools.GetPropertyValue(SolutionParserProjects, solutionParser);
            for (int i = 0; i < array.Length; i++)
            {
                Contract.RequireNotNull(array.GetValue(i), out object SolutionProject);
                Project NewProject = new Project(SolutionProject);

                ProjectList.Add(NewProject);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the solution file name.
        /// </summary>
        public string SolutionFileName { get; init; }

        /// <summary>
        /// Gets the solution name.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Gets the list of projects in the solution.
        /// </summary>
        public List<Project> ProjectList { get; init; }
        #endregion
    }
}
