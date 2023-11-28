using System.Collections.Specialized;
using System.Configuration;

#if DLAB_UNROOT_COMMON_NAMESPACE
namespace DLaB.Common
#else
namespace Source.DLaB.Common
#endif
{
    /// <summary>
    /// Allows for Injection Custom Config provider.  Defaults to ConfigurationManager.AppSettings
    /// </summary>
#if DLAB_PUBLIC
    public class ConfigProvider
#else
    internal class ConfigProvider
# endif
    {
        private static readonly object SingletonLock = new object();
        private static NameValueCollection _instance;
        
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static NameValueCollection Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                lock (SingletonLock)
                {
                    if (_instance != null)
                    {
                        return _instance;
                    }

                    InitializeProvider(ConfigurationManager.AppSettings); 
                }

                return _instance;
            }
        }

        private ConfigProvider() { }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        public static void InitializeProvider(NameValueCollection appSettings)
        {
            _instance = appSettings;
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="appSettings">The application settings.</param>
        public static void InitializeProvider(KeyValueConfigurationCollection appSettings)
        {
            _instance =  appSettings.ToNameValueCollection();
        }
}
}
