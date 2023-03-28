using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DLaB.ModelBuilderExtensions.Tests
{
    [TestClass]
    public class DataModelBuilderTests
    {
        private readonly HashSet<string> Switches = new HashSet<string>(new[]
        {
            "emitfieldsclasses",
            "generateActions",
            "generateGlobalOptionSets",
            "emitfieldsclasses",
            "interactivelogin",
            "help",
            "legacyMode",
            "nologo",
            "splitfiles",
            "suppressGeneratedCodeAttribute",
            "suppressINotifyPattern",
            "writesettingsTemplateFile"
        });

        private readonly HashSet<string> NotValid = new HashSet<string>(new[]
        {
            "entitytypesfolder",
            "language",
            "messagestypesfolder",
            "optionsetstypesfolder",
            "includeMessages"
        });

        [TestMethod]
        public void CreateTestFileGeneration()
        {
            if (!Debugger.IsAttached && !ConfigHelper.GetAppSettingOrDefault("TestFileCreation", false))
            {
                Assert.Inconclusive("No Debugger Attached and Test File Creation is not true!");
                return;
            }
            var connectionString = ConfigHelper.GetAppSettingOrDefault("DLaB.EarlyBoundGenerator.ConnectionString", "NONE");
            if (connectionString == "NONE")
            {
                Console.WriteLine("Add DLaB.EarlyBoundGenerator.ConnectionString to the app.config or to your machine.config!");
                Assert.Inconclusive("No Connection string found!");
                return;
            }

            using (var tmp = TempDir.Create())
            {
                var settingsPath = CreateBuilderSettingsConfig(tmp);
                var parameters = CreateParameters(settingsPath, tmp);
                var runner = new ProcessModelInvoker(parameters);
                var client = new ServiceClient(connectionString);
                var invoke = runner.Invoke(client);
                Assert.AreEqual(0, invoke);
            }
        }

        private string[] CreateParameters(string settingsPath, ITempDir tmp)
        {
            var parameters = new ModelBuilderInvokeParameters
            {
                SettingsTemplateFile = settingsPath,
                OutDirectory = tmp.Name,
                SplitFilesByObject = true,
            };

            var lines = new List<string>();

            foreach (var kvp in parameters.ToDictionary().Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value)))
            {
                if (NotValid.Contains(kvp.Key))
                {
                    // do nothing...
                }
                else if (Switches.Contains(kvp.Key))
                {
                    if (bool.TryParse(kvp.Value, out var boolVal) && boolVal)
                    {
                        lines.Add($"/{kvp.Key}");
                    }
                }
                else
                {
                    lines.Add($"/{kvp.Key}:{kvp.Value}");
                }
            }

            var linesArray = lines.OrderBy(v => v).ToArray();
            return linesArray;
        }

        private static string CreateBuilderSettingsConfig(ITempDir tmp)
        {
            var settingsPath = Path.Combine(tmp.Name, "builderSettings.json");
            var json = TestProject.GetResourceText("Resources.DefaultBuilderSettings.json");
            json = json.Replace(",DLaB.EarlyBoundGenerator", ",DLaB.ModelBuilderExtensions");
            File.WriteAllText(settingsPath, json);
            return settingsPath;
        }
    }
}
