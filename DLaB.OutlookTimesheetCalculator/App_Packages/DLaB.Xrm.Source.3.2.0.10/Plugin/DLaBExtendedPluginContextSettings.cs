using System;
using Microsoft.Xrm.Sdk;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Plugin
#else
namespace Source.DLaB.Xrm.Plugin
#endif
{
    /// <summary>
    /// Settings to be used for the DLaBExtendedPluginContext
    /// </summary>
    public class DLaBExtendedPluginContextSettings: IExtendedPluginContextInitializer
    {
        /// <summary>
        /// Settings for the IOrganization Service
        /// </summary>
        public ExtendedOrganizationServiceSettings OrganizationServiceSettings { get; set; }
        /// <summary>
        /// The max length of the trace log.
        /// </summary>
        public int? MaxTraceLength { get; set; }

        /// <summary>
        /// Creates a DLaBExtendedPluginContextSettings
        /// </summary>
        public DLaBExtendedPluginContextSettings()
        {
            OrganizationServiceSettings = new ExtendedOrganizationServiceSettings();
        }

        #region Initializers

        /// <summary>
        /// Allows for defining the IOrganizationServiceFactory
        /// </summary>
        /// <param name="serviceProvider">Service Provider</param>
        /// <param name="tracingService">Tracing Service</param>
        /// <returns></returns>
        public virtual IOrganizationServiceFactory InitializeServiceFactory(IServiceProvider serviceProvider, ITracingService tracingService)
        {
            return serviceProvider.GetService<IOrganizationServiceFactory>();
        }

        /// <summary>
        /// Allows for defining the ITracingService
        /// </summary>
        /// <param name="serviceProvider">Service Provider</param>
        /// <returns></returns>
        public virtual ITracingService InitializeTracingService(IServiceProvider serviceProvider)
        {
            if (MaxTraceLength.HasValue)
            {
                return new ExtendedTracingService(serviceProvider.GetService<ITracingService>(), MaxTraceLength.Value);
            }
            else
            {
                return new ExtendedTracingService(serviceProvider.GetService<ITracingService>());
            }
        }

        /// <summary>
        /// Allows for defining the initialization of IOrganizationServices
        /// </summary>
        /// <param name="factory">Factory</param>
        /// <param name="userId">User Id</param>
        /// <param name="tracingService">Tracing Service</param>
        /// <returns></returns>
        public virtual IOrganizationService InitializeIOrganizationService(IOrganizationServiceFactory factory, Guid? userId, ITracingService tracingService)
        {
            return new ExtendedOrganizationService(factory.CreateOrganizationService(userId), tracingService, OrganizationServiceSettings);
        }

        /// <summary>
        /// Allows for defining the IPluginExecutionContext
        /// </summary>
        /// <param name="serviceProvider">Service Provider</param>
        /// <param name="tracingService">Tracing Service</param>
        /// <returns></returns>
        public virtual IPluginExecutionContext InitializePluginExecutionContext(IServiceProvider serviceProvider, ITracingService tracingService)
        {
            return (IPluginExecutionContext) serviceProvider.GetService(typeof(IPluginExecutionContext));
        }

        #endregion Initializers

    }
}
