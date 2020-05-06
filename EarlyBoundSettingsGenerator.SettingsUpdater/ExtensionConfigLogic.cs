using System;
using System.IO;

namespace EarlyBoundSettingsGenerator.SettingsUpdater
{
    public class ExtensionConfigLogic: FileUpdateBase
    {
        public const string FileName = @"ExtensionConfig.cs";

        public ExtensionConfigLogic(PropertyInfo property) : base(property) { }
        
        public override void UpdateFile()
        {
            var path = GetLogicSettingsFilePath(FileName);
            var file = File.ReadAllLines(path);

            AddProperty(file);
            AddToExtensionConfigGetDefault(file);
            AddToExtensionConfigSetPopulatedValues(file);
            AddPropertyToPoco(file);
            File.WriteAllLines(path, file);
        }

        private void AddProperty(string[] file)
        {
            var insertIndex = GetInsertIndexOfAlphabeticallySortedProperty(file, "public class", "#region NonSerialized Properties", Property.Name, "        public ");
            file[insertIndex - 1] += $@"
        /// <summary>
        /// {Property.Description.Replace(Environment.NewLine, Environment.NewLine + @"        /// ")}
        /// </summary>
        public {Property.Type} {Property.Name} {{ get; set; }}";
        }

        private void AddToExtensionConfigGetDefault(string[] file)
        {
            var insertIndex = GetInsertIndexOfAlphabeticallySortedProperty(file, "return new ExtensionConfig", "};", Property.Name, "                ", 0);
            file[insertIndex - 1] += $@"
                {Property.Name} = {Property.DefaultValue},";
        }

        private void AddPropertyToPoco(string[] file)
        {
            var insertIndex = GetInsertIndexOfAlphabeticallySortedProperty(file, "namespace DLaB.EarlyBoundGenerator.Settings.POCO", null, Property.Name, "        public ", initialInsertOffset:7);
            file[insertIndex - 1] += $@"
        public {Property.PocoType} {Property.Name} {{ get; set; }}";
        }

        private void AddToExtensionConfigSetPopulatedValues(string[] file)
        {
            var insertIndex = GetInsertIndexOfAlphabeticallySortedProperty(file, "SetPopulatedValues", "string GetValueOrDefault", Property.Name, "            ", 0);
            if (Property.Type == "bool")
            {
                file[insertIndex - 1] += $@"
                {Property.Name} = poco.{Property.Name} ?? {Property.Name};";
            }
            else
            {
                file[insertIndex - 1] += $@"
                {Property.Name} = GetValueOrDefault(poco.{Property.Name}, {Property.Name});";

            }
        }
    }
}