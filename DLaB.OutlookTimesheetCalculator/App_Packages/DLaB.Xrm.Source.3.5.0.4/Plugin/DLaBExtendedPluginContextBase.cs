using System;
#if !DLAB_XRM_DEBUG
using System.Diagnostics;
#endif
using Microsoft.Xrm.Sdk;
#if DLAB_UNROOT_COMMON_NAMESPACE
using DLaB.Common;
#else
using Source.DLaB.Common;
#endif

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Plugin
#else
namespace Source.DLaB.Xrm.Plugin
#endif
{
    /// <summary>
    /// The Implementation of the IExtendedPluginContext
    /// </summary>
    // ReSharper disable once InconsistentNaming
#if !DLAB_XRM_DEBUG
    [DebuggerNonUserCode]
#endif
    public class DLaBExtendedPluginContextBase : IExtendedPluginContext
    {
#region Properties

#region IPluginContext Properties

        /// <summary>
        /// Gets or sets the plugin execution context.
        /// </summary>
        /// <value>
        /// The plugin execution context.
        /// </value>
        protected IPluginExecutionContext PluginExecutionContext { get; set; }
        /// <summary>
        /// Gets the mode.
        /// </summary>
        /// <value>
        /// The mode.
        /// </value>
        public virtual int Mode => PluginExecutionContext.Mode;
        int IExecutionContext.IsolationMode => PluginExecutionContext.IsolationMode;
        /// <summary>
        /// Gets the depth.
        /// </summary>
        /// <value>
        /// The depth.
        /// </value>
        public virtual int Depth => PluginExecutionContext.Depth;
        /// <summary>
        /// Gets the name of the message.
        /// </summary>
        /// <value>
        /// The name of the message.
        /// </value>
        public string MessageName => PluginExecutionContext.MessageName;
        /// <summary>
        /// Gets the name of the primary entity.
        /// </summary>
        /// <value>
        /// The name of the primary entity.
        /// </value>
        public string PrimaryEntityName => PluginExecutionContext.PrimaryEntityName;
        /// <summary>
        /// Gets the request identifier.
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        public virtual Guid? RequestId => PluginExecutionContext.RequestId;
        /// <summary>
        /// Gets the name of the secondary entity.
        /// </summary>
        /// <value>
        /// The name of the secondary entity.
        /// </value>
        public virtual string SecondaryEntityName => PluginExecutionContext.SecondaryEntityName;
        /// <summary>
        /// Gets the input parameters.
        /// </summary>
        /// <value>
        /// The input parameters.
        /// </value>
        public virtual ParameterCollection InputParameters => PluginExecutionContext.InputParameters;
        /// <summary>
        /// Gets the output parameters.
        /// </summary>
        /// <value>
        /// The output parameters.
        /// </value>
        public virtual ParameterCollection OutputParameters => PluginExecutionContext.OutputParameters;
        /// <summary>
        /// Gets the shared variables.
        /// </summary>
        /// <value>
        /// The shared variables.
        /// </value>
        public virtual ParameterCollection SharedVariables => PluginExecutionContext.SharedVariables;
        /// <summary>
        /// Gets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public virtual Guid UserId => PluginExecutionContext.UserId;
        /// <summary>
        /// Gets the initiating user identifier.
        /// </summary>
        /// <value>
        /// The initiating user identifier.
        /// </value>
        public virtual Guid InitiatingUserId => PluginExecutionContext.InitiatingUserId;
        /// <summary>
        /// Gets the business unit identifier.
        /// </summary>
        /// <value>
        /// The business unit identifier.
        /// </value>
        public virtual Guid BusinessUnitId => PluginExecutionContext.BusinessUnitId;
        /// <summary>
        /// Gets the organization identifier.
        /// </summary>
        /// <value>
        /// The organization identifier.
        /// </value>
        public virtual Guid OrganizationId => PluginExecutionContext.OrganizationId;
        /// <summary>
        /// Gets the name of the organization.
        /// </summary>
        /// <value>
        /// The name of the organization.
        /// </value>
        public virtual string OrganizationName => PluginExecutionContext.OrganizationName;
        /// <summary>
        /// Gets the primary entity identifier.
        /// </summary>
        /// <value>
        /// The primary entity identifier.
        /// </value>
        public virtual Guid PrimaryEntityId => PluginExecutionContext.PrimaryEntityId;
        /// <summary>
        /// Gets the pre entity images.
        /// </summary>
        /// <value>
        /// The pre entity images.
        /// </value>
        public virtual EntityImageCollection PreEntityImages => PluginExecutionContext.PreEntityImages;
        /// <summary>
        /// Gets the post entity images.
        /// </summary>
        /// <value>
        /// The post entity images.
        /// </value>
        public virtual EntityImageCollection PostEntityImages => PluginExecutionContext.PostEntityImages;
        /// <summary>
        /// Gets the owning extension.
        /// </summary>
        /// <value>
        /// The owning extension.
        /// </value>
        public virtual EntityReference OwningExtension => PluginExecutionContext.OwningExtension;
        /// <summary>
        /// Gets the correlation identifier.
        /// </summary>
        /// <value>
        /// The correlation identifier.
        /// </value>
        public virtual Guid CorrelationId => PluginExecutionContext.CorrelationId;
        /// <summary>
        /// Gets a value indicating whether this instance is executing offline.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is executing offline; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsExecutingOffline => PluginExecutionContext.IsExecutingOffline;
        /// <summary>
        /// Gets a value indicating whether this instance is offline playback.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is offline playback; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsOfflinePlayback => PluginExecutionContext.IsOfflinePlayback;
        /// <summary>
        /// Gets a value indicating whether this instance is in transaction.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is in transaction; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsInTransaction => PluginExecutionContext.IsInTransaction;
        /// <summary>
        /// Gets the operation identifier.
        /// </summary>
        /// <value>
        /// The operation identifier.
        /// </value>
        public virtual Guid OperationId => PluginExecutionContext.OperationId;
        /// <summary>
        /// Gets the operation created on.
        /// </summary>
        /// <value>
        /// The operation created on.
        /// </value>
        public virtual DateTime OperationCreatedOn => PluginExecutionContext.OperationCreatedOn;
        /// <summary>
        /// Gets the stage.
        /// </summary>
        /// <value>
        /// The stage.
        /// </value>
        public virtual int Stage => PluginExecutionContext.Stage;
        /// <summary>
        /// Gets the parent context.
        /// </summary>
        /// <value>
        /// The parent context.
        /// </value>
        public virtual IPluginExecutionContext ParentContext => PluginExecutionContext.ParentContext;

#endregion // IPluginContext Properties

#region IExtendedPluginContext Properties

        /// <summary>
        /// Gets the isolation mode of the plugin assembly.
        /// </summary>
        /// <value>
        /// The isolation mode of the plugin assembly.
        /// </value>
        public IsolationMode IsolationMode => (IsolationMode) PluginExecutionContext.IsolationMode;

        /// <summary>
        /// Returns true if the execution context is asynchronous (Mode = 1)
        /// </summary>
        public bool IsAsync => Mode == 1;

        /// <summary>
        /// Returns true if the execution context is synchronous (Mode = 0)
        /// </summary>
        public bool IsSync => Mode == 0;

        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        /// <value>
        /// The service provider.
        /// </value>
        public IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// The current event the plugin is executing for.
        /// </summary>
        public RegisteredEvent Event { get; private set; }

        private IOrganizationService _cachedOrganizationService;
        private IOrganizationService _organizationService;
        private IOrganizationService _systemOrganizationService;
        private IOrganizationService _triggeredUserOrganizationService;
        private IOrganizationServiceFactory _serviceFactory;
        private ITracingService _tracingService;

        /// <summary>
        /// The IOrganizationService of the plugin, Impersonated as the user that the plugin is was initiated by
        /// </summary>
        public IOrganizationService InitiatingUserOrganizationService => _triggeredUserOrganizationService ?? (_triggeredUserOrganizationService = Settings.InitializeIOrganizationService(ServiceFactory, InitiatingUserId, TracingService));

        /// <inheritdoc />
        /// <summary>
        /// The IOrganizationService of the plugin, Impersonated as the user that the plugin is registered to run as.
        /// </summary>
        public IOrganizationService OrganizationService => _organizationService ?? (_organizationService = Settings.InitializeIOrganizationService(ServiceFactory, UserId, TracingService));

        /// <summary>
        /// The IPluginExecutionContext of the plugin.
        /// </summary>
        ///internal IPluginExecutionContext PluginExecutionContext { get; private set; }
        
        /// <summary>
        /// The Type.FullName of the plugin.
        /// </summary>
        /// 
        public string PluginTypeName { get; private set; }

        /// <summary>
        /// Pulls the PrimaryEntityName, and PrimaryEntityId from the context and returns it as an Entity Reference
        /// </summary>
        /// <value>
        /// The primary entity.
        /// </value>
        public virtual EntityReference PrimaryEntity => new EntityReference(PrimaryEntityName, PrimaryEntityId);

        /// <summary>
        /// Service Factory
        /// </summary>
        public IOrganizationServiceFactory ServiceFactory => _serviceFactory ?? (_serviceFactory = Settings.InitializeServiceFactory(ServiceProvider, TracingService));

        /// <summary>
        /// A service that will cache the retrieve/retrieve multiple results and reuse them.  Uses the SystemService to prevent different users from retrieving different results.
        /// </summary>
        public IOrganizationService CachedOrganizationService => _cachedOrganizationService ?? (_cachedOrganizationService = new ReadOnlyCachedService(SystemOrganizationService));

        /// <summary>
        /// The IOrganizationService of the plugin, using the System User
        /// </summary>
        public IOrganizationService SystemOrganizationService => _systemOrganizationService ?? (_systemOrganizationService = Settings.InitializeIOrganizationService(ServiceFactory, null, TracingService));

        /// <summary>
        /// The ITracingService of the plugin.
        /// </summary>
        public ITracingService TracingService => _tracingService ?? (_tracingService = Settings.InitializeTracingService(ServiceProvider));

#endregion IExtendedPluginContext Properties

        private IExtendedPluginContextInitializer Settings { get; }

#endregion Properties

#region ImageNames struct

        /// <summary>
        /// Struct for the Standard Plugin Image Names
        /// </summary>
        public struct PluginImageNames
        {
            /// <summary>
            /// The default pre image
            /// </summary>
            public const string PreImage = "PreImage";
            /// <summary>
            /// The default post image
            /// </summary>
            public const string PostImage = "PostImage";
        }

#endregion ImageNames struct

#region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DLaBExtendedPluginContextBase"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="plugin">The plugin.</param>
        /// <param name="settings">Settings used to control services of context</param>
        /// <exception cref="System.ArgumentNullException">
        /// serviceProvider
        /// or
        /// plugin
        /// </exception>
        public DLaBExtendedPluginContextBase(IServiceProvider serviceProvider, IRegisteredEventsPlugin plugin, IExtendedPluginContextInitializer settings = null)
        {
            if (plugin == null)
            {
                throw new ArgumentNullException(nameof(plugin));
            }

            Settings = settings ?? new DLaBExtendedPluginContextSettings();
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            PluginExecutionContext = Settings.InitializePluginExecutionContext(serviceProvider, TracingService);
            InitializePluginProperties(plugin);
        }

#endregion Constructors

#region PropertyInitializers

        /// <summary>
        /// Initializes the plugin properties.
        /// </summary>
        /// <param name="plugin">The plugin.</param>
        private void InitializePluginProperties(IRegisteredEventsPlugin plugin)
        {
            Event = PluginExecutionContext.GetEvent(plugin.RegisteredEvents);
            if (Event == null)
            {
                var message = $"No RegisteredEvent found for the current context of Stage: {this.GetPipelineStage()}, Message: {MessageName}, Entity: {PrimaryEntityName}.  Either Unregister the plugin for this event, or include this as a RegisteredEvent in the Plugin's RegisteredEvents.";
                try
                {
                    TracingService.Trace(message);
                    TracingService.Trace(this.GetContextInfo());
                }
                finally
                {
                    throw new InvalidPluginExecutionException(message);
                }
            }
            if (Event.Message == RegisteredEvent.Any)
            {
                Event = new RegisteredEvent(Event.Stage, PluginExecutionContext.GetMessageType(), Event.Execute);
            }
            PluginTypeName = plugin.GetType().FullName;
        }

#endregion PropertyInitializers

#region Exception Logging

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        public virtual void LogException(Exception ex)
        {
            TracingService.Trace("Exception: {0}", ex.ToStringWithCallStack());
            TracingService.Trace(this.GetContextInfo());
        }

#endregion Exception Logging

#region Trace

        /// <summary>
        /// Traces the specified message.  By default, is guaranteed to not throw an exception.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">Optional Args</param>
        public void Trace(string format, params object[] args)
        {
            TracingService.Trace(format, args);
        }

        /// <summary>
        /// Traces the entire context.
        /// </summary>
        public virtual void TraceContext()
        {
            Trace(this.ToStringDebug());
        }

        /// <summary>
        /// Traces the time from call to dispose.  Designed to be used in a using statement
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public IDisposable TraceTime(string format, params object[] args)
        {
            return new TraceTimer(TracingService, string.Format(format, args));
        }

#endregion Trace
    }
}
