using System.Collections.Generic;

namespace DLaB.ModelBuilderExtensions
{
    public abstract class TypedServiceSettings<T>: CustomServiceSettings
    {
        protected T DefaultService { get; set; }
        protected TypedServiceSettings(T defaultService, DLaBModelBuilderSettings? settings = null): base(settings)
        {
            DefaultService = defaultService;
            Settings = settings ?? new DLaBModelBuilderSettings();
        }

        protected TypedServiceSettings(T defaultService, IDictionary<string, string> parameters): base(parameters)
        {
            DefaultService = defaultService;
            ConfigHelper.Initialize(parameters);
            Settings = ConfigHelper.Settings;
        }
    }
}
