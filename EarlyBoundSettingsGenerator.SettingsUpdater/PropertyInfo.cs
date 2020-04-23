
namespace EarlyBoundSettingsGenerator.SettingsUpdater
{
    public class PropertyInfo
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string DefaultValue { get; set; }
        public string PocoType => Type == "bool" ? "bool?" : Type;
        public string ToStringCall => Type == "bool" ? ".ToString()" : string.Empty;

    }
}
