using System;
using System.IO;
using DLaB.Log;

namespace DLaB.VSSolutionAccelerator.Logic
{
    public class VisualStudio
    {
        public static void InstallCodeSnippets(string pluginPath)
        {
            var codeGenPath = Path.GetFullPath(Path.Combine(pluginPath, Settings.TemplateFolder, "CodeGeneration"));
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var vsDirectories = Directory.GetDirectories(documentsPath, "Visual Studio *");
            if (vsDirectories.Length == 0)
            {
                Logger.Show("Unable to find any Visual Studio directories in " + documentsPath);
            }

            foreach (var vs in vsDirectories)
            {
                foreach (var file in Directory.GetFiles(codeGenPath, "*.snippet"))
                {
                    var snippetFolder = Path.Combine(vs, "Code Snippets", "Visual C#", "My Code Snippets");
                    if (!Directory.Exists(snippetFolder))
                    {
                        continue;
                    }
                    var newFile = Path.Combine(snippetFolder, Path.GetFileName(file));
                    if (File.Exists(newFile))
                    {
                        Logger.AddDetail($"File {newFile} already exists!  Skipping installing snippet.");
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(newFile) ?? "");
                        Logger.AddDetail($"Installing snippet '{newFile}'...");
                        File.Copy(file, newFile);
                    }
                }
            }
        }

    }
}
