using System;
using System.IO;

namespace EarlyBoundSettingsGenerator.SettingsUpdater
{
    public class SettingsMap: FileUpdateBase
    {
        public const string FileName = @"SettingsMap.cs";

        public SettingsMap(PropertyInfo property) : base(property) { }
        
        public override void UpdateFile()
        {
            var path = GetGeneratorSettingsFilePath(FileName);
            var file = File.ReadAllLines(path);

            AddProperty(file);
            File.WriteAllLines(path, file);
        }

        private void AddProperty(string[] file)
        {
            var start = $"#region {Property.Category}";
            var end = $"#endregion {Property.Category}";
            var lineStart = "        public ";

            var firstIndex = GetInsertIndexOfAlphabeticallySortedProperty(file, start, end, " ", lineStart);
            var insertIndex = GetInsertIndexOfAlphabeticallySortedProperty(file, start, end, Property.Name, lineStart);
            if (firstIndex != insertIndex)
            {
                insertIndex += 6;
            }
            file[insertIndex - 1] += $@"
        [Category(""{Property.Category}"")]
        [DisplayName(""{Property.DisplayName}"")]
        [Description(""{Property.Description.Replace(Environment.NewLine, "  ")}"")]
        public {Property.Type} {Property.Name}
        {{
            get => Config.ExtensionConfig.{Property.Name};
            set => Config.ExtensionConfig.{Property.Name} = value;
        }}
";
        }
    }
}