using System;
using System.Collections.Generic;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Plugin
#else
namespace Source.DLaB.Xrm.Plugin
#endif
	
{
    /// <summary>
    /// An abstract base Plugin that implements the DLaBGenericPluginBase.
    /// </summary>
    public abstract class DLaBPluginBase: DLaBGenericPluginBase<IExtendedPluginContext>
    {
        /// <inheritdoc />
        protected DLaBPluginBase(string unsecureConfig, string secureConfig): base(unsecureConfig, secureConfig)
        {
        }

        /// <inheritdoc />
        protected override IExtendedPluginContext CreatePluginContext(IServiceProvider serviceProvider)
        {
            return new DLaBExtendedPluginContextBase(serviceProvider, this);
        }

    }
}
