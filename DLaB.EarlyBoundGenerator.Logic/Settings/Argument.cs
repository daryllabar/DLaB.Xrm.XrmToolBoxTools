using System;

namespace DLaB.EarlyBoundGenerator.Settings
{
    [Serializable]
    public class Argument
    {
        public CreationType SettingType { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public Argument()
        {
        }
    
        public Argument(CreationType settingType, string name, string value)
        {
            SettingType = settingType;
            Name = name;
            Value = value;
        }

        public Argument(CreationType settingType, CrmSrvUtilService service, string value) :this(settingType, service.ToString().ToLower(), value)
        { }
    }

    public enum CrmSrvUtilService
    {
        CodeCustomization,
        CodeGenerationService,
        CodeWriterFilter,
        MetadataProviderService,
        NamingService
    }
}
