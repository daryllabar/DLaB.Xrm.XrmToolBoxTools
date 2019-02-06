using System.Collections.Generic;
using System.IO;
using System.Linq;
using Source.DLaB.Common;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class ProjectFile
    {
        public string Name { get; set; }
        public Dictionary<string, string> Replacements { get; set; }
        public List<string> Removals { get; set; }

        public ProjectFile()
        {
            Replacements = new Dictionary<string, string>();
            Removals = new List<string>();
        }

        public void Update(string newDirectory)
        {
            var filePath = Path.Combine(newDirectory, Name);
            var newLines = new List<string>();
            foreach (var line in File.ReadLines(filePath))
            {
                var newLine = line;
                if (Removals.Any(r => newLine.Contains(r)))
                {
                    continue;
                }

                foreach (var replacement in Replacements)
                {
                    if (newLine.Contains(replacement.Key))
                    {
                        newLine = line.Replace(replacement.Key, replacement.Value);
                    }
                }

                newLines.Add(newLine);
            }

            File.WriteAllLines(filePath, newLines);
        }
    }
}
