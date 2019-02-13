using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.VSSolutionAccelerator.Tests
{
    [TestClass]
    public class VisualStudioTests
    {
        [TestMethod]
        public void InstallCodeSnippets_WhenEmpty_Should_CreateNew()
        {
            void AssertSnippetCreated(string[] snippets, string snippet)
            {
                Assert.That.ExistsLineContaining(snippets, snippet, $"Snippet {snippet} was not copied!");
            }

            var pluginPath = TestBase.GetPluginsPath();
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var vsDirectories = Directory.GetDirectories(documentsPath, "Visual Studio *");
            var folders = new Dictionary<string, string>();
            foreach (var vs in vsDirectories)
            {
                var existingFolder = Path.Combine(vs, "Code Snippets", "Visual C#", "My Code Snippets");
                if (!Directory.Exists(existingFolder))
                {
                    continue;
                }
                var backupFolder = Path.Combine(Directory.GetParent(existingFolder).FullName, "My Code Snippets Backup");
                if (!Directory.Exists(backupFolder))
                {
                    Directory.Move(existingFolder, backupFolder);
                }
                else
                {
                    TestBase.ClearDirectory(existingFolder);
                }
                Directory.CreateDirectory(existingFolder);
                folders.Add(backupFolder, existingFolder);
            }

            Logic.VisualStudio.InstallCodeSnippets(pluginPath);

            foreach (var value in folders.Select(i => new { SnippetFolder = i.Value, Backup = i.Key}))
            {
                var snippets = Directory.GetFiles(value.SnippetFolder, "*.snippet");
                // ReSharper disable StringLiteralTypo
                AssertSnippetCreated(snippets, "crmplugin.snippet");
                AssertSnippetCreated(snippets, "crmplugintest.snippet");
                AssertSnippetCreated(snippets, "crmtestmethodclass.snippet");
                // ReSharper restore StringLiteralTypo
                AssertSnippetCreated(snippets, "region.snippet");

                TestBase.ClearDirectory(value.SnippetFolder);
                Directory.Delete(value.SnippetFolder);
                Directory.Move(value.Backup, value.SnippetFolder);
            }
        }
    }
}
