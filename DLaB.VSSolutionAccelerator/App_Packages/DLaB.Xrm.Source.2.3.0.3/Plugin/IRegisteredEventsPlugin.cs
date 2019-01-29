using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Plugin
#else
namespace Source.DLaB.Xrm.Plugin
#endif
	
{
    /// <summary>
    /// An IPlugin that defines Registered Events
    /// </summary>
    public interface IRegisteredEventsPlugin : IPlugin
    {
        /// <summary>
        /// Gets the registered events that the plug-in should fire for, and what the action should be.
        /// </summary>
        /// <value>
        /// The registered events.
        /// </value>
        IEnumerable<RegisteredEvent> RegisteredEvents { get; }
    }
}
