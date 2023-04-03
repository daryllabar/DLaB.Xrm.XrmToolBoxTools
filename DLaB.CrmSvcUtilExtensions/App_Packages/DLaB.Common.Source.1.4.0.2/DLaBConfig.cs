using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;


#if DLAB_UNROOT_COMMON_NAMESPACE
namespace DLaB.Common
#else
namespace Source.DLaB.Common
#endif
{
    /// <summary>
    /// The different uses of encoding within the DLaB code base.
    /// </summary>
#if DLAB_PUBLIC
    public enum EncodingUses
#else
    internal enum EncodingUses
#endif
    {
        /// <summary>
        /// Used for Base64 conversions
        /// </summary>
        Base64,
        /// <summary>
        /// Used for JSON conversions
        /// </summary>
        Json,
        /// <summary>
        /// Used for Zipping/Unzipping
        /// </summary>
        Zip,
    }

    /// <summary>
    /// Implement this class to be able to provide config information to be used by the DLaB code base
    /// </summary>
#if DLAB_PUBLIC
    public interface IDLaBConfig
#else
    internal interface IDLaBConfig
#endif
    {
        /// <summary>
        /// Defines the DataContactJsonSerializerSettings class to use by default for the SerializeToJson and DeserializeJson extension methods if none is provided
        /// </summary>
        /// <param name="textOrValue">The text string when called from DeserializeJson and the value to serialize when called from SerializeToJson.</param>
        /// <param name="encoding">The encoding, either the passed in value, or the default value if none is given.</param>
        DataContractJsonSerializerSettings GetJsonSerializerSettings(object textOrValue = null, Encoding encoding = null);

        /// <summary>
        /// Defines the Encoding class to use by default for the given extension methods if none is provided
        /// </summary>
        /// <param name="use">The current usage of the Encoding.</param>
        /// <param name="textOrValue">The text string to encode from, or the object to encode to.</param>
        Encoding GetEncoding(EncodingUses use, object textOrValue = null);
    }

    /// <summary>
    /// Handles loading the DLaBConfig
    /// </summary>
    internal static class DLaBConfig
    {
        private static readonly Lazy<IDLaBConfig> LazyConfig = new Lazy<IDLaBConfig>(CreateConfigInstance);
        /// <summary>
        /// The Config, lazily loaded getting the first implementation of the interface.
        /// </summary>
        public static IDLaBConfig Config => LazyConfig.Value;

        private static IDLaBConfig CreateConfigInstance()
        {
            var configType = typeof(IDLaBConfig).GetFirstImplementation();
            if (configType == null)
            {
                return new DefaultConfig();
            }

            return (IDLaBConfig)Activator.CreateInstance(configType, false);
        }

        private class DefaultConfig : IDLaBConfig
        {
            public DataContractJsonSerializerSettings GetJsonSerializerSettings(object text = null, Encoding encoding = null)
            {
                return new DataContractJsonSerializerSettings
                {
                    // ReSharper disable once StringLiteralTypo
                    DateTimeFormat = new DateTimeFormat("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK"),
                    UseSimpleDictionaryFormat = true
                };
            }

            public Encoding GetEncoding(EncodingUses use, object text = null)
            {
                return Encoding.UTF8;
            }
        }
    }
}
