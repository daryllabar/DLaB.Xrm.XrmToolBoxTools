using System;
using System.Linq;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
{
    /// <summary>
    /// Implement this class to be able to provide config information to be used by the DLaB.Xrm code base
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
    /// Searches the current assembly for the first public class that implements IDLaBConfig
    /// </summary>
    // ReSharper disable once InconsistentNaming
    internal static class DLaBEntityHelperConfig
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<IEntityHelperConfig> _config = new Lazy<IEntityHelperConfig>(CreateConfigInstance);
        public static IEntityHelperConfig Config => _config.Value;

        private static IEntityHelperConfig CreateConfigInstance()
        {
            var configType = GetFirstImplementation(typeof(IEntityHelperConfig));
            if (configType == null)
            {
                return new DefaultConfig();
            }

            return (IEntityHelperConfig)Activator.CreateInstance(configType, false);
        }

        public static Type GetFirstImplementation(Type interfaceType)
        {
            return interfaceType.Assembly.ExportedTypes.FirstOrDefault(t => !t.IsInterface && interfaceType.IsAssignableFrom(t));
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
