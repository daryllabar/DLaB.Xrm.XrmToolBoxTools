using System;
using System.Collections.Generic;

namespace DLaB.ModelBuilderExtensions
{
    public abstract class TypedServiceBase<T> : TypedServiceSettings<T>
    {
        private ServiceCache _serviceCache;

        public IServiceProvider ServiceProvider => ServiceCache.ServiceProvider;

        public ServiceCache ServiceCache
        {
            get
            {
                if (_serviceCache == null)
                {
                    throw new Exception($"An attempt was made to access the ServiceCache without initializing it in {this.GetType().FullName}!");
                }
                return _serviceCache;
            }
            set => _serviceCache = value;
        }

        protected TypedServiceBase(T defaultService, DLaBModelBuilderSettings settings = null) : base(defaultService, settings)
        {
        }

        protected TypedServiceBase(T defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
        }

        protected void SetServiceCache(IServiceProvider services)
        {
            ServiceCache = _serviceCache ?? ServiceCache.GetDefault(services);
        }
    }
}
