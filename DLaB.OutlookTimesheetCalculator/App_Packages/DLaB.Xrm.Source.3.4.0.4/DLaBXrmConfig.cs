using System;
#if DLAB_UNROOT_COMMON_NAMESPACE
using DLaB.Common;
#else
using Source.DLaB.Common;
#endif

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
{
    /// <summary>
    /// Implement this class to be able to provide config information to be used by the DLaB.Xrm code base.
    /// Currently this isn't needed, but may additional config at a later date.
    /// </summary>
    public interface IDLaBXrmConfig: IEntityHelperConfig { }

    /// <summary>
    /// Implement this class to be able to provide config information to be used by the Entity Helper code base
    /// </summary>
    public interface IEntityHelperConfig
    {
        /// <summary>
        /// Defines any id logical names that don't follow the standard conventions.
        /// </summary>
        /// <param name="logicalName">Logical Name of the Entity</param>
        /// <returns></returns>
        string GetIrregularIdAttributeName(string logicalName);

        /// <summary>
        /// Defines the primaryFieldInfo for any entities that don't follow the standard conventions.
        /// </summary>
        /// <param name="logicalName">Logical Name of the Entity</param>
        /// <param name="defaultInfo">Default Primary Field Info</param>
        /// <returns></returns>
        PrimaryFieldInfo GetIrregularPrimaryFieldInfo(string logicalName, PrimaryFieldInfo defaultInfo = null);
    }

    /// <summary>
    /// Searches the current assembly for the first public class that implements the request config interfaces searching for IDLaBXrmConfig first, then the specific interfaces later
    /// </summary>
    // ReSharper disable once InconsistentNaming
    internal static class DLaBXrmConfig
    {
        private static readonly object SearchLock = new object();
        private static bool _searchedForConfig;

        // ReSharper disable once InconsistentNaming
        private static IDLaBXrmConfig _dLaBConfig;
        private static readonly Lazy<IEntityHelperConfig> LazyEntityHelperConfig = new Lazy<IEntityHelperConfig>(CreateConfigInstance<IEntityHelperConfig>);
        public static IEntityHelperConfig EntityHelperConfig => LazyEntityHelperConfig.Value;

        private static T CreateConfigInstance<T>()
        {
            if (!_searchedForConfig)
            {
                SearchForIDLaBXrmConfig();
            }

            if (_dLaBConfig != null)
            {
                return (T)_dLaBConfig;
            }

            var configType = typeof(T).GetFirstImplementation();
            if (configType == null)
            {
                return (T)(object)new DefaultConfig();
            }

            return (T)Activator.CreateInstance(configType, false);
        }

        // ReSharper disable once InconsistentNaming
        private static void SearchForIDLaBXrmConfig()
        {
            lock (SearchLock)
            {
                if (_searchedForConfig)
                {
                    return;
                }

                var dLaBConfig = typeof(IDLaBXrmConfig).GetFirstImplementation();
                if (dLaBConfig != null)
                {
                    _dLaBConfig = (IDLaBXrmConfig) Activator.CreateInstance(dLaBConfig, false);
                }
                _searchedForConfig = true;
            }
        }

        private class DefaultConfig : IEntityHelperConfig
        {
            public string GetIrregularIdAttributeName(string logicalName)
            {
                return null;
            }

            public PrimaryFieldInfo GetIrregularPrimaryFieldInfo(string logicalName, PrimaryFieldInfo defaultInfo = null)
            {
                return null;
            }
        }
    }
}
