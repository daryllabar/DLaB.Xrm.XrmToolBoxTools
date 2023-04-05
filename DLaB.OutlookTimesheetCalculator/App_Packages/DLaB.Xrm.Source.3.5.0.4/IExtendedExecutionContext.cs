using System;
using Microsoft.Xrm.Sdk;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
{
    /// <summary>
    /// BaseIExtendedExecutionContext interface
    /// </summary>
    public interface IExtendedExecutionContext : IExecutionContext, ITracingService
    {
        /// <summary>
        /// The IOrganizationService of the plugin, Impersonated as the user that triggered the services using the InitiatingUserId.
        /// </summary>
        IOrganizationService InitiatingUserOrganizationService { get; }

        /// <summary>
        /// Returns true if the execution context is asynchronous (Mode = 1)
        /// </summary>
        bool IsAsync { get; }

        /// <summary>
        /// Returns true if the execution context is synchronous (Mode = 0)
        /// </summary>
        bool IsSync { get; }

        /// <summary>
        /// A service that will cache the retrieve/retrieve multiple results and reuse them.  Uses the SystemService to prevent different users from retrieving different results.
        /// </summary>
        IOrganizationService CachedOrganizationService { get; }

        /// <summary>
        /// The IOrganizationService of the plugin, Impersonated as the user that plugin is registered to run as, using the PluginExecutionContext.UserId.
        /// </summary>
        IOrganizationService OrganizationService { get; }

        /// <summary>
        /// The IOrganizationServiceFactory of the plugin.
        /// </summary>
        IOrganizationServiceFactory ServiceFactory { get; }

        /// <summary>
        /// The IOrganizationService of the plugin, using the System User by not specifying a UserId.
        /// </summary>
        IOrganizationService SystemOrganizationService { get; }

        /// <summary>
        /// The ITracingService of the plugin.
        /// </summary>
        ITracingService TracingService { get; }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        void LogException(Exception ex);

        /// <summary>
        /// Traces the entire context.
        /// </summary>
        void TraceContext();

        /// <summary>
        /// Traces the time from call to dispose.  Designed to be used in a using statement
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        IDisposable TraceTime(string format, params object[] args);
    }
}
