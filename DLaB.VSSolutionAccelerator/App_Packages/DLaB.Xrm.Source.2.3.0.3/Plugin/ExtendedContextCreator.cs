using System;
using System.Collections.Generic;
using System.Text;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Plugin
#else
namespace Source.DLaB.Xrm.Plugin
#endif
{
    /// <summary>
    /// Default Implmentation
    /// </summary>
    public class ExtendedContextCreator<T> : IExtendedContextCreator<T> where T : IExtendedPluginContext
    {
        /// <inheritdoc />
        public IExtendedPluginContext Create(IServiceProvider provider) { return (T)Activator.CreateInstance(typeof(T), provider); }
    }
}
