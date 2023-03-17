using System;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Plugin
#else
namespace Source.DLaB.Xrm.Plugin
#endif
{
    /// <summary>
    /// Interface for defining how to get an IExtendedPluginContext
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IExtendedContextCreator<T> where T: IExtendedPluginContext
    {
        /// <summary>
        /// Creates an IExtendedPluginContext from the Service Provider.
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        IExtendedPluginContext Create(IServiceProvider provider);
    }
}
