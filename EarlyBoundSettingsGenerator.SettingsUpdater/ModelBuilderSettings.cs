using System;
using System.IO;

namespace EarlyBoundSettingsGenerator.SettingsUpdater
{
    public class ModelBuilderSettings: FileUpdateBase
    {
        public const string FileName = @"DLaBModelBuilderSettings.cs";

        public ModelBuilderSettings(PropertyInfo property) : base(property) { }
        
        public override void UpdateFile()
        {
            var path = GetModelBuilderExtPath(FileName);
            var file = File.ReadAllLines(path);

            AddProperty(file);
            File.WriteAllLines(path, file);
        }

        private void AddProperty(string[] file)
        {
            var start = "public class DLaBModelBuilder";
            var end = "public DLaBModelBuilder()";
            var lineStart = "        public ";

            var firstIndex = GetInsertIndexOfAlphabeticallySortedProperty(file, start, end, " ", lineStart);
            var insertIndex = GetInsertIndexOfAlphabeticallySortedProperty(file, start, end, Property.Name, lineStart);

            file[insertIndex] += $@"
        [JsonPropertyName(""{Property.Name[0].ToString().ToLower() + Property.Name.Substring(1)}"")]
        public {Property.Type} {Property.Name} {{ get; set; }}
";
        }
    }
}