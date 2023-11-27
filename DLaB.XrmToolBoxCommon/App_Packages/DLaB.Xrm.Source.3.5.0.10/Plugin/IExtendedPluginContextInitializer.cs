using System;
using Microsoft.Xrm.Sdk;


#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Plugin
#else
namespace Source.DLaB.Xrm.Plugin
#endif
{
    /// <summary>
    /// Interface for Initializing a Plugin Context
    /// </summary>
    public interface IExtendedPluginContextInitializer
    {
        
        #region Initializers
        
        /// <summary>
        /// Allows for defining the IPluginExecutionContext
        /// </summary>
        /// <param name="serviceProvider">Service Provider</param>
        /// <param name="tracingService">Tracing Service</param>
        /// <returns></returns>
        IPluginExecutionContext InitializePluginExecutionContext(IServiceProvider serviceProvider, ITracingService tracingService);

        /// <summary>
        /// Allows for defining the IOrganizationServiceFactory
        /// </summary>
        /// <param name="serviceProvider">Service Provider</param>
        /// <param name="tracingService">Tracing Service</param>
        /// <returns></returns>
        IOrganizationServiceFactory InitializeServiceFactory(IServiceProvider serviceProvider, ITracingService tracingService);
        
        /// <summary>
        /// Allows for defining the ITracingService
        /// </summary>
        /// <param name="serviceProvider">Service Provider</param>
        /// <returns></returns>
        ITracingService InitializeTracingService(IServiceProvider serviceProvider);
        
        /// <summary>
        /// Allows for defining the initialization of IOrganizationServices
        /// </summary>
        /// <param name="factory">Factory</param>
        /// <param name="userId">User Id</param>
        /// <param name="tracingService">Tracing Service</param>
        /// <returns></returns>
        IOrganizationService InitializeIOrganizationService(IOrganizationServiceFactory factory, Guid? userId, ITracingService tracingService);

        #endregion Initializers
    }
}
