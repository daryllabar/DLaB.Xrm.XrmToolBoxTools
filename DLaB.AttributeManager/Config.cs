using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace DLaB.AttributeManager
{
    [Serializable]
    public class Config
    {
        #region Properties

        /// <summary>
        /// The Postfix for Temp Attributes
        /// </summary>
        /// <value>
        /// The Postfix
        /// </value>
        public string TempSchemaPostfix { get; set; }
        
        /// <summary>
        /// The version of the AttributeManager Plugin
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string Version { get; set; }

        #region NonSerialized Properties

        [NonSerialized]
        private string _filePath;

        #endregion NonSerialized Properties

        #endregion // Properties

        private Config()
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        #region Add Missing Default settings

        /// <summary>
        /// Initializes a new instance of the <see cref="Config"/> class.
        /// </summary>
        /// <param name="poco">The poco.</param>
        /// <param name="filePath">The file path.</param>
        private Config(POCO.Config poco, string filePath) : this()
        {
            var @default = GetDefault();

            TempSchemaPostfix = poco.TempSchemaPostfix ?? @default.TempSchemaPostfix ;

            _filePath = filePath;
        }

        public static Config GetDefault()
        {
            return new Config
            {
                TempSchemaPostfix = @"_t_"   
            };
        }

        #endregion // Add Missing Default settings

        public static Config Load(string filePath)
        {
            filePath = Path.Combine(filePath, "DLaB.AttributeManager.Settings.xml");
            if (!File.Exists(filePath))
            {
                var config = GetDefault();
                config._filePath = filePath;
                return config;
            }

            var serializer = new XmlSerializer(typeof(POCO.Config));
            POCO.Config poco;
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                poco = (POCO.Config)serializer.Deserialize(fs);
                fs.Close();
            }
            var settings = new Config(poco, filePath);
            return settings;
        }

        public void Save()
        {
            var serializer = new XmlSerializer(typeof (Config));
            var settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true,
            };
            using (var xmlWriter = XmlWriter.Create(_filePath, settings))
            {
                serializer.Serialize(xmlWriter, this);
                xmlWriter.Close();
            }
        }
    }
}

namespace DLaB.AttributeManager.POCO
{
    /// <summary>
    /// Serializable Class with Nullable types to be able to tell if they are populated or not
    /// </summary>
    public class Config
    {
        public string TempSchemaPostfix { get; set; }
        public string Version { get; set; }
    }
}