using System.Collections.Generic;

namespace DLaB.ModelBuilderExtensions
{
    public abstract class CustomServiceSettings
    {
        protected DLaBModelBuilderSettings Settings { get; set; }
        protected DLaBModelBuilder DLaBSettings => Settings.DLaBModelBuilder;

        protected CustomServiceSettings(DLaBModelBuilderSettings? settings = null)
        {
            Settings = settings ?? new DLaBModelBuilderSettings();
        }

        protected CustomServiceSettings(IDictionary<string, string> parameters)
        {
            ConfigHelper.Initialize(parameters);
            Settings = ConfigHelper.Settings;
        }
    }
}
