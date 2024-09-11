using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class GitIgnoreEditor
    {
        public const string UserSpecificSection = @"# User-specific files
*.user.config
*.suo
*.user
*.userosscache
*.sln.docstates
";
        public const string IgnoreXrmUnitTestUserConfig = "*.user.config";

        public static void AddXrmUnitTestConfig(InitializeSolutionInfo info)
        {
            if (!info.ConfigureXrmUnitTest)
            {
                return;
            }

            var gitIgnorePath = Path.Combine(Path.GetDirectoryName(info.SolutionPath) ?? string.Empty, @".gitignore");
            if (!File.Exists(gitIgnorePath))
            {
                File.WriteAllText(gitIgnorePath, UserSpecificSection);
                return;
            }

            var lines = File.ReadAllLines(gitIgnorePath);
            if (lines.Any(l => l.Trim() == IgnoreXrmUnitTestUserConfig))
            {
                return;
            }

            var updatedLines = new List<string>(lines);
            var userSpecificFilesIndex = Array.FindIndex(lines, l => l.Trim() == "# User-specific files");
            if (userSpecificFilesIndex >= 0)
            {
                updatedLines.Insert(userSpecificFilesIndex + 1, IgnoreXrmUnitTestUserConfig);
            }
            else
            {
                updatedLines.Add(UserSpecificSection);
            }
            File.WriteAllLines(gitIgnorePath, updatedLines);
        }
    }
}
