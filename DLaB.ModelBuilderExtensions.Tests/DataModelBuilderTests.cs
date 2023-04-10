using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Source.DLaB.Common;
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
        private readonly HashSet<string> _switches = new HashSet<string>(new[]
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

        private readonly HashSet<string> _notValid = new HashSet<string>(new[]
        {
            "entitytypesfolder",
            "language",
            "messagestypesfolder",
            "optionsetstypesfolder",
            "includeMessages"
        });

        /// <summary>
        /// Used to manually test generation using either a real connection, or serialized metadata.
        /// </summary>
        [TestMethod]
        public void CreateTestFileGeneration()
        {
            if (!Debugger.IsAttached && !Config.GetAppSettingOrDefault("TestFileCreation", false))
            {
                Assert.Inconclusive("No Debugger Attached and Test File Creation is not true!");
                return;
            }
            IOrganizationService client = null;

            var serializedMetadataPath = Config.GetAppSettingOrDefault("SerializedMetadataPath", "NONE");
            if (serializedMetadataPath == "NONE")
            {
                var connectionString = Config.GetAppSettingOrDefault("DLaB.EarlyBoundGenerator.ConnectionString", "NONE");
                if (connectionString == "NONE")
                {
                    Console.WriteLine("Add DLaB.EarlyBoundGenerator.ConnectionString to the app.config or to your machine.config or serialize the metadata and set the SerializedMetadataPath!");
                    Assert.Inconclusive("No Connection string found!");
                    return;
                }
                client = new ServiceClient(connectionString);
            }

            using (var tmp = TempDir.Create())
            {
                var settingsPath = CreateBuilderSettingsConfig(tmp, serializedMetadataPath);
                var parameters = CreateParameters(settingsPath, tmp);
                var runner = new ProcessModelInvoker(parameters);
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
                if (_notValid.Contains(kvp.Key))
                {
                    // do nothing...
                }
                else if (_switches.Contains(kvp.Key))
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

        private static string CreateBuilderSettingsConfig(ITempDir tmp, string serializedMetadataPath)
        {
            var settingsPath = Path.Combine(tmp.Name, "builderSettings.json");
            var path = Config.GetAppSettingOrDefault("JsonFileToUse", "DEFAULT");
            var json = path.ToUpper() == "DEFAULT" ? TestProject.GetResourceText("Resources.DefaultBuilderSettings.json") : File.ReadAllText(path);
            json = UpdateJson(json, path, serializedMetadataPath);
            File.WriteAllText(settingsPath, json);
            return settingsPath;
        }

        private static string UpdateJson(string json, string sourceJsonPath, string serializedMetadataPath)
        {
            if (string.IsNullOrWhiteSpace(serializedMetadataPath))
            {
                return json;
            }

            var lines = json.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
            serializedMetadataPath = $"    \"serializedMetadataRelativeFilePath\": \"{serializedMetadataPath.Replace("\\", "\\\\")}\",";
            var readSerialized = "    \"readSerializedMetadata\": true,";
            var modelBuilderIndex = -1;
            var updateJsonPath = sourceJsonPath != "DEFAULT";
            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.Contains("\"dLaB.ModelBuilder\""))
                {
                    modelBuilderIndex = i;
                }

                if (line.Contains("\"serializedMetadataRelativeFilePath\""))
                {
                    lines[i] = serializedMetadataPath;
                    serializedMetadataPath = null;
                }

                if (line.Contains("\"readSerializedMetadata\""))
                {
                    lines[i] = readSerialized;
                    readSerialized = null;
                }

                if (updateJsonPath && line.Contains("\"camelCaseNamesDictionaryRelativePath\""))
                {
                    var path = Config.GetAppSettingOrDefault("DictionaryPath", "MISSING DICTIONARY PATH APP.CONFIG VALUES!").Replace("\\", "\\\\");
                    lines[i] = $"    \"camelCaseNamesDictionaryRelativePath\": \"{path}\",";
                    updateJsonPath = false;
                }
            }

            if (serializedMetadataPath != null)
            {
                lines.Insert(modelBuilderIndex+1, serializedMetadataPath);
            }

            if (readSerialized != null)
            {
                lines.Insert(modelBuilderIndex + 1, readSerialized);
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}
