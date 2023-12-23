using System.IO;
using System.Linq;

namespace EarlyBoundSettingsGenerator.SettingsUpdater
{
    public class TestDefaultBuilderSettingsJson: FileUpdateBase
    {
        public const string FileName = @"DefaultBuilderSettings.json";

        public TestDefaultBuilderSettingsJson(PropertyInfo property) : base(property) { }
        
        public override void UpdateFile()
        {
            var path = GetTestResourcesPath(FileName);
            var file = File.ReadAllLines(path);

            AddProperty(file);
            File.WriteAllLines(path, file);
        }

        private void AddProperty(string[] file)
        {
            const string start = "\"dLaB.ModelBuilder\": {";
            const string end = "},";
            const string lineStart = "    \"";

            var jsonName = Property.Name[0].ToString().ToLower() + new string(Property.Name.Skip(1).ToArray());
            var insertIndex = GetInsertIndexOfAlphabeticallySortedProperty(file, start, end, "\"" +jsonName, lineStart, indexOfWordInLine: 0);

            var jsonValue = Property.Type == "bool" ? Property.DefaultValue : "\"" + Property.DefaultValue + "\"";
            file[insertIndex] += $@"
    ""{jsonName}"": {jsonValue},";
        }
    }
}