using System;
using System.IO;
using System.Reflection;

namespace EarlyBoundSettingsGenerator.SettingsUpdater
{
    public abstract class FileUpdateBase
    {
        public const string GeneratorSettingsRelativePath = @"DLaB.EarlyBoundGeneratorV2\Settings";
        public const string LogicSettingsRelativePath = @"DLaB.EarlyBoundGeneratorV2.Logic\Settings";
        public const string LogicRelativePath = @"DLaB.EarlyBoundGeneratorV2.Logic";
        public const string ModelBuilderExtensionsPath = @"DLaB.ModelBuilderExtensions";
        public const string TestResourcesPath = @"DLaB.ModelBuilderExtensions.Tests\Resources";

        public PropertyInfo Property { get; set; }
        public DirectoryInfo SolutionDirectory => SolutionDirectoryLazy.Value;

        private static readonly Lazy<DirectoryInfo> SolutionDirectoryLazy = new Lazy<DirectoryInfo>(GetSolutionDirectory);

        protected FileUpdateBase(PropertyInfo property)
        {
            Property = property;
        }

        public abstract void UpdateFile();

        protected string GetGeneratorSettingsFilePath(string fileName)
        {
            var path = Path.Combine(SolutionDirectory.FullName, GeneratorSettingsRelativePath, fileName);
            AssertFileExists(path);
            return path;
        }

        protected string GetLogicSettingsFilePath(string fileName)
        {
            var path =  Path.Combine(SolutionDirectory.FullName, LogicSettingsRelativePath, fileName);
            AssertFileExists(path);
            return path;
        }

        protected string GetLogicFilePath(string fileName)
        {
            var path = Path.Combine(SolutionDirectory.FullName, LogicRelativePath, fileName);
            AssertFileExists(path);
            return path;
        }

        protected string GetModelBuilderExtPath(string fileName)
        {
            var path = Path.Combine(SolutionDirectory.FullName, ModelBuilderExtensionsPath, fileName);
            AssertFileExists(path);
            return path;
        }

        protected string GetTestResourcesPath(string fileName)
        {
            var path = Path.Combine(SolutionDirectory.FullName, TestResourcesPath, fileName);
            AssertFileExists(path);
            return path;
        }

        protected void AssertFileExists(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception("Unable to find file at path " + path + "!");
            }
        }

        protected int GetInsertIndexOfAlphabeticallySortedProperty(string[] file, string startString, string tooFarString, string valueToInsert, string lineStartMatch, int indexOfWordInLine = 2, int initialInsertOffset = 2)
        {
            var possibleInsertIndex = -1;

            for (var i = 0; i < file.Length; i++)
            {
                var line = file[i].TrimStart();
                if (possibleInsertIndex == -1)
                {
                    if (line.StartsWith(startString))
                    {
                        possibleInsertIndex = i + initialInsertOffset;
                    }
                    continue;
                }
                if (tooFarString != null 
                    && line.StartsWith(tooFarString))
                {
                    break;
                }

                var originalLine = file[i];
                while (line.TrimEnd().EndsWith("=") || line.TrimEnd().EndsWith("+"))
                {
                    line += file[++i];
                }

                if (originalLine.Replace("\t", "    ").StartsWith(lineStartMatch))
                {
                    var existingPropName = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[indexOfWordInLine];
                    if (string.Compare(existingPropName, valueToInsert, StringComparison.Ordinal) > 0)
                    {
                        continue;
                    }
                    possibleInsertIndex = i + 1;
                }
            }

            if (possibleInsertIndex == -1)
            {
                throw new Exception("Unable to determine property index insertion!");
            }

            return possibleInsertIndex;
        }

        private static DirectoryInfo GetSolutionDirectory()
        {
            // Check for Debug/Release ==> EarlyBoundSettingsGenerator.SettingsUpdater\bin\Debug
            var directory = Directory.GetParent(Environment.CurrentDirectory)?.Parent;
            if (directory?.Name == Assembly.GetCallingAssembly().GetName().Name)
            {
                return directory.Parent;
            }

            // Assume in folder of project
            if (directory == null)
            {
                throw new Exception("Expected to be ran from VS or to be in the folder of the EarlyBound Generator Project");
            }

            return directory;
        }
    }
}
