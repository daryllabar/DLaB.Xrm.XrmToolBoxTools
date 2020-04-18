using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Crm.Services.Utility;

namespace DLaB.CrmSvcUtilExtensions.Tests
{
    /// <summary>
    /// Class needed to get default services from CrmSvcUtil.  Using reflection, this depends on the following things being unchanged:<para/>
    /// - CrmSvcUtil contains a CrmSvcUtilParameters Type, with a constructor that takes 0 arguments <para/>
    /// - CrmSvcUtil contains a ServiceProvider Type, with a constructor that takes 0 arguments <para/>
    /// - ServiceProvider contains an InitializeServices method, that takes a CrmSvcUtilParameters type<para/> 
    /// - ServiceProvider's InitializeServices method populates a private _services field.<para/>
    /// </summary>
    /// <seealso cref="System.IServiceProvider" />
    public class ServiceFactory : IServiceProvider
    {
        private readonly Dictionary<Type, object> _defaultServices;
        public IServiceProvider ServiceProvider { get; private set; }

        public ServiceFactory()
        {
            var parameters = GetCrmSvcUtilTypeInstance("Microsoft.Crm.Services.Utility.CrmSvcUtilParameters");
            var provider = GetCrmSvcUtilTypeInstance("Microsoft.Crm.Services.Utility.ServiceProvider");
            CallMethod(provider, "InitializeServices", new [] {parameters});
            _defaultServices = GetField<Dictionary<Type, object>>(provider, "_services");

            Add<IMetadataProviderService>(new Metadata.Provider("2016.xml"));
            ServiceProvider = (IServiceProvider) provider;
        }

        public void Add<T>(T service) { _defaultServices[typeof(T)] = service; }

        public T GetService<T>() { return (T)_defaultServices[typeof(T)]; }
        public object GetService(Type serviceType) { return _defaultServices[serviceType]; }

        public IOrganizationMetadata GetMetadata(string fileName = null)
        {
            if (fileName != null)
            {
                Add<IMetadataProviderService>(new Metadata.Provider(fileName));
            }
            var provider = GetService<IMetadataProviderService>();
            return provider.LoadMetadata();
        }

        private object GetCrmSvcUtilTypeInstance(string typeName, object[] arguments = null)
        {
            var type = typeof(ICodeGenerationService).Assembly.GetType(typeName);
            var ctor = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];

            return ctor.Invoke(arguments);
        }

        private object CallMethod(object obj, string name, object[] arguments = null)
        {
            var types = new List<Type>();
            if (arguments != null)
            {
                foreach (var arg in arguments)
                {
                    types.Add(arg.GetType());
                }
            }
            var method = obj.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance, null, types.ToArray(), new ParameterModifier[0]);
            return method.Invoke(obj, arguments);
        }

        private T GetField<T>(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)field.GetValue(obj);
        }
    }
}
