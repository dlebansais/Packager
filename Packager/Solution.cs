namespace Packager
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public class Solution
    {
        //internal class SolutionParser
        //Name: Microsoft.Build.Construction.SolutionParser
        //Assembly: Microsoft.Build, Version=4.0.0.0

        static readonly Type s_SolutionParser;
        static readonly PropertyInfo s_SolutionParser_solutionReader;
        static readonly MethodInfo s_SolutionParser_parseSolution;
        static readonly PropertyInfo s_SolutionParser_projects;

        static Solution()
        {
            s_SolutionParser = Type.GetType("Microsoft.Build.Construction.SolutionParser, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            s_SolutionParser_solutionReader = s_SolutionParser.GetProperty("SolutionReader", BindingFlags.NonPublic | BindingFlags.Instance);
            s_SolutionParser_projects = s_SolutionParser.GetProperty("Projects", BindingFlags.NonPublic | BindingFlags.Instance);
            s_SolutionParser_parseSolution = s_SolutionParser.GetMethod("ParseSolution", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public Solution(string solutionFileName)
        {
            SolutionFileName = solutionFileName;

            ConstructorInfo[] Constructors = s_SolutionParser.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            var solutionParser = Constructors[0].Invoke(null);

            using StreamReader streamReader = new StreamReader(solutionFileName);
            s_SolutionParser_solutionReader.SetValue(solutionParser, streamReader, null);
            s_SolutionParser_parseSolution.Invoke(solutionParser, null);

            var array = (Array)s_SolutionParser_projects.GetValue(solutionParser, null);
            for (int i = 0; i < array.Length; i++)
            {
                Project NewProject = new Project(array.GetValue(i));
                ProjectList.Add(NewProject);
            }
        }

        public string SolutionFileName { get; }
        public List<Project> ProjectList { get; private set; } = new List<Project>();
    }
}
