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
            var insertIndex = GetInsertIndexOfAlphabeticallySortedProperty(file, $"#region {Property.Category}", $"#endregion {Property.Category}", Property.Name, "        public ");
            file[insertIndex - 1] += $@"
        [Category(""{Property.Category}"")]
        [DisplayName(""{Property.DisplayName}"")]
        [Description(""{Property.Description.Replace(Environment.NewLine, "  ")}"")]
        public {Property.Type} {Property.Name}
        {{
            get => Config.ExtensionConfig.{Property.Name};
            set => Config.ExtensionConfig.{Property.Name} = value;
        }}";
        }
    }
}