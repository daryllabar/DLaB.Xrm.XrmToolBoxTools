using System;

namespace DLaB.EarlyBoundGenerator.Settings
{
    /// <summary>
    /// POCO for arguments to add to the command line.
    /// </summary>
    [Serializable]
    public class Argument
    {
        /// <summary>
        /// The creation type this argument applies to
        /// </summary>
        public CreationType SettingType { get; set; }
        /// <summary>
        /// Argument Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Argument Value
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Valueless parameters are generated without a value
        /// </summary>
        public bool Valueless { get; set; }

        /// <summary>
        /// Initializes the Argument
        /// </summary>
        public Argument()
        {
            Valueless = false;
        }

        /// <summary>
        /// Initializes the Argument
        /// </summary>
        /// <param name="settingType"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Argument(CreationType settingType, string name, string value)
        {
            SettingType = settingType;
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Initializes the Argument
        /// </summary>
        /// <param name="settingType"></param>
        /// <param name="service"></param>
        /// <param name="value"></param>
        public Argument(CreationType settingType, CrmSrvUtilService service, string value) :this(settingType, service.ToString().ToLower(), value)
        { }
    }

    /// <summary>
    /// List of possible CrmSrvUtil Services
    /// </summary>
    public enum CrmSrvUtilService
    {
        CodeCustomization,
        CodeGenerationService,
        CodeWriterFilter,
        MetadataProviderService,
        NamingService
    }
}
