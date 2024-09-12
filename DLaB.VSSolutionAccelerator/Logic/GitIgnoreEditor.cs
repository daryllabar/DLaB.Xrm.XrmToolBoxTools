using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DLaB.Log;

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
                Logger.AddDetail($"Creating '.gitignore' at path: {gitIgnorePath}");
                File.WriteAllText(gitIgnorePath, UserSpecificSection);
                return;
            }

            var lines = File.ReadAllLines(gitIgnorePath);
            if (lines.Any(l => l.Trim() == IgnoreXrmUnitTestUserConfig))
            {
                Logger.AddDetail($"'.gitignore' at '{gitIgnorePath}' already has XrmUnitTest User Config ignore.  Skipping update of file.");
                return;
            }

            var updatedLines = new List<string>(lines);
            var userSpecificFilesIndex = Array.FindIndex(lines, l => l.Trim() == "# User-specific files");
            if (userSpecificFilesIndex >= 0)
            {
                Logger.AddDetail($"Adding XrmUnitTest User Config ignore to '.gitignore' at '{gitIgnorePath}'.");
                updatedLines.Insert(userSpecificFilesIndex + 1, IgnoreXrmUnitTestUserConfig);
            }
            else
            {
                Logger.AddDetail($"Adding User ignore section to '.gitignore' at '{gitIgnorePath}'.");
                updatedLines.Add(UserSpecificSection);
            }
            File.WriteAllLines(gitIgnorePath, updatedLines);
        }
    }
}
