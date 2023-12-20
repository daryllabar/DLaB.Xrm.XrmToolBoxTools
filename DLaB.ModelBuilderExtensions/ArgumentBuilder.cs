using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DLaB.ModelBuilderExtensions
{
    public class ArgumentBuilder
    {
        // ReSharper disable StringLiteralTypo
        private static readonly HashSet<string> ModelBuilderSwitches = new HashSet<string>(new[]
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

        private static readonly HashSet<string> ModelBuilderParametersToSkip = new HashSet<string>(new[]
        {
            // Parameters that are in the template file
            "emitEntityETC",
            "emitVirtualAttributes",
            "entitytypesfolder",
            "generatesdkmessages",
            "language",
            "messagestypesfolder",
            "optionsetstypesfolder"
        });
        // ReSharper restore StringLiteralTypo

        private readonly Action<string> _log;
        public string SettingsPath { get; }
        public string OutputPath { get; }

        public ArgumentBuilder(string settingsPath, string outputPath, Action<string> log = null)
        {
            _log = log ?? Console.WriteLine;
            SettingsPath = settingsPath;
            OutputPath = outputPath;
        }

        public string[] GetArguments()
        {
            var parameters = new ModelBuilderInvokeParameters(new ModeBuilderLoggerService("EarlyBoundGenerator"))
            {
                SettingsTemplateFile = SettingsPath,
                SplitFilesByObject = true,
                OutDirectory = OutputPath,
                //SystemDefaultLanguageId = 1033, Don't think this works or is valid
            };
            var lines = new List<string>();
            var commandLine = new List<string>();
            _log("Generating ProcessModelInvoker Arguments:");
            foreach (var kvp in parameters.ToDictionary().Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value)))
            {
                if (ModelBuilderParametersToSkip.Contains(kvp.Key))
                {
                    // skip...
                }
                else if (ModelBuilderSwitches.Contains(kvp.Key))
                {
                    if (bool.TryParse(kvp.Value, out var boolVal) && boolVal)
                    {
                        var flag = "--" + kvp.Key;
                        commandLine.Add(flag);
                        _log(flag);
                        lines.Add($"/{kvp.Key}");
                    }
                }
                else
                {
                    var kvpParameter = $"--{kvp.Key} {kvp.Value}";
                    commandLine.Add(kvpParameter);
                    _log(kvpParameter);
                    lines.Add($"/{kvp.Key}:{kvp.Value}");
                }
            }

            _log("Finished Generating ProcessModelInvoker Arguments.");
            var values = lines.OrderBy(v => v).ToArray();

            _log("Command line for Cloud generation:");
            _log($"PAC modelbuilder build {string.Join(" ", commandLine.Where(v => !v.Contains("splitfiles")))}");

            return values;
        }
    }
}
