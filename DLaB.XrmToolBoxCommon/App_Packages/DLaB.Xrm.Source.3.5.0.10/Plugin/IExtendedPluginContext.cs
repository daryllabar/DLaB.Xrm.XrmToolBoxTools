using System;
using Microsoft.Xrm.Sdk;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Plugin
#else
namespace Source.DLaB.Xrm.Plugin
#endif

{
    /// <summary>
    /// Plugin Context Interface for Handling Additional functionality
    /// </summary>
    public interface IExtendedPluginContext : IPluginExecutionContext, IExtendedExecutionContext
    {
        #region Properties

        /// <summary>
        /// The current event the plugin is executing for.
        /// </summary>
        RegisteredEvent Event { get; }

        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        /// <value>
        /// The service provider.
        /// </value>
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the isolation mode of the plugin assembly.
        /// </summary>
        /// <value>
        /// The isolation mode of the plugin assembly.
        /// </value>
        new IsolationMode IsolationMode { get; }

        /// <summary>
        /// The Type.FullName of the plugin.
        /// </summary>
        string PluginTypeName { get; }

        /// <summary>
        /// Pulls the PrimaryEntityName, and PrimaryEntityId from the context and returns it as an Entity Reference
        /// </summary>
        /// <value>
        /// The primary entity.
        /// </value>
        EntityReference PrimaryEntity { get; }

        #endregion Properties
    }
}
