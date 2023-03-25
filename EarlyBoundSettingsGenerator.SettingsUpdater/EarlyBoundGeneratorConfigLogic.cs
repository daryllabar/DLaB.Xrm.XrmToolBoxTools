using System;
using System.IO;

namespace EarlyBoundSettingsGenerator.SettingsUpdater
{
    public class EarlyBoundGeneratorConfigLogic: FileUpdateBase
    {
        public const string FileName = @"Logic.cs";

        public EarlyBoundGeneratorConfigLogic(PropertyInfo property) : base(property) { }
        
        public override void UpdateFile()
        {
            return;
            // Model Builder doesn't need Logic updated
            //var path = GetLogicFilePath(FileName);
            //var file = File.ReadAllLines(path);
            //
            //AddExtensionConfigInit(file);
            //File.WriteAllLines(path, file);
        }

        private void AddExtensionConfigInit(string[] file)
        {
            var start = "var extensions = earlyBoundGeneratorConfig.ExtensionConfig;";
            var end = "_configUpdated = true;";
            var name = "extensions." + Property.Name;
            var lineStart = "                    UpdateConfigAppSetting(file, ";
            var lastIndex = GetInsertIndexOfAlphabeticallySortedProperty(file, start, end, "z", lineStart);
            var insertIndex = GetInsertIndexOfAlphabeticallySortedProperty(file, start, end, name, lineStart);
            file[insertIndex - 1] += $@"{(lastIndex == insertIndex ? " |" : string.Empty)}
                    UpdateConfigAppSetting(file, ""{Property.Name}"", extensions.{Property.Name}{Property.ToStringCall}){(lastIndex == insertIndex ? string.Empty : " |")}";
        }
    }
}