using System.IO;

namespace EarlyBoundSettingsGenerator.SettingsUpdater
{
    public class EarlyBoundGeneratorConfig : FileUpdateBase
    {
        public const string FileName = @"EarlyBoundGeneratorConfig.cs";

        public EarlyBoundGeneratorConfig(PropertyInfo property) : base(property) { }
        
        public override void UpdateFile()
        {
            var path = GetLogicSettingsFilePath(FileName);
            var file = File.ReadAllLines(path);

            AddToConstructorExtensionConfigInitialization(file);
            File.WriteAllLines(path, file);
        }

        private void AddToConstructorExtensionConfigInitialization(string[] file)
        {
            var insertIndex = GetInsertIndexOfAlphabeticallySortedProperty(file, "ExtensionConfig = new ExtensionConfig", "};", Property.Name, "                ", 0);
            if (Property.Type == "bool")
            {
                file[insertIndex - 1] += $@"
                {Property.Name} = pocoConfig.{Property.Name} ?? defaultConfig.{Property.Name},";
            }
            else
            {
                file[insertIndex - 1] += $@"
                {Property.Name} = GetValueOrDefault(pocoConfig.{Property.Name}, defaultConfig.{Property.Name}),";

            }
        }
    }
}