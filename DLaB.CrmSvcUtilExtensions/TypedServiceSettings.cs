using System.Collections.Generic;

namespace DLaB.ModelBuilderExtensions
{
    public abstract class TypedServiceSettings<T>
    {
        protected T DefaultService { get; set; }
        protected DLaBModelBuilderSettings Settings { get; set; }
        protected DLaBModelBuilder DLaBSettings => Settings.DLaBModelBuilder;

        protected TypedServiceSettings(T defaultService, DLaBModelBuilderSettings settings = null)
        {
            DefaultService = defaultService;
            Settings = settings ?? new DLaBModelBuilderSettings();
        }

        protected TypedServiceSettings(T defaultService, IDictionary<string, string> parameters)
        {
            DefaultService = defaultService;
            ConfigHelper.Initialize(parameters);
            Settings = ConfigHelper.Settings;
        }
    }
}
