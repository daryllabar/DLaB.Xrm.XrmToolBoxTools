using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Crm.Services.Utility;

namespace DLaB.CrmSvcUtilExtensions
{
    public class BaseMetadataProviderService: IMetadataProviderService
    {
        private IOrganizationMetadata Metadata { get; set; }

        protected IMetadataProviderService DefaultService { get; }

        protected string FilePath { get;}

        // ReSharper disable once UnusedParameter.Local
        public BaseMetadataProviderService(IMetadataProviderService defaultService, IDictionary<string, string> parameters)
        {
            if (ConfigHelper.GetAppSettingOrDefault("WaitForAttachedDebugger", false))
            {
                while (!Debugger.IsAttached)
                {
                    Console.WriteLine("[**** Waiting For Debugger ****]");
                    Thread.Sleep(3000);
                }
            }
            DefaultService = defaultService;
            FilePath = ConfigHelper.GetAppSettingOrDefault("SerializedMetadataFilePath", "metadata.xml");
        }

        public IOrganizationMetadata LoadMetadata() { return Metadata ?? (Metadata = LoadMetadataInternal()); }

        protected virtual IOrganizationMetadata LoadMetadataInternal()
        {
            IOrganizationMetadata metadata;
            if (ConfigHelper.GetAppSettingOrDefault("ReadSerializedMetadata", false))
            {
               metadata = DeserializeMetadata(FilePath);
            }
            else
            {
                metadata = DefaultService.LoadMetadata();

                if (ConfigHelper.GetAppSettingOrDefault("SerializeMetadata", false))
                {
                    SerializeMetadata(metadata, FilePath);
                }
            }

            return metadata;
        }

        public static void SerializeMetadata(IOrganizationMetadata metadata, string filePath)
        {
            var localMetadata = new Metadata
            {
                Entities = metadata.Entities,
                OptionSets = metadata.OptionSets,
                Messages = metadata.Messages
            };

            filePath = RootPath(filePath);
            File.WriteAllText(filePath, Serialize(localMetadata,true));
        }

        private static string RootPath(string filePath)
        {
            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
            }
            return filePath;
        }

        public static IOrganizationMetadata DeserializeMetadata(string filePath)
        {
            filePath = RootPath(filePath);
            return DeserializeDataObject<Metadata>(File.ReadAllText(filePath));
        }

        /// <summary>
        /// Deserializes the string xml value to an IExtensibleDataObject
        /// </summary>
        /// <param name="xml"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T DeserializeDataObject<T>(string xml) where T : IExtensibleDataObject
        {
            var serializer = new DataContractSerializer(typeof(T));
            return (T)(serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(xml))));
        }

        public static string Serialize<T>(T obj, bool indent = false)
        {
            DataContractSerializer contractSerializer = new DataContractSerializer(obj.GetType());
            using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                using (var xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    if (indent)
                    {
                        xmlTextWriter.Formatting = Formatting.Indented;
                        xmlTextWriter.Indentation = 2;
                    }
                    contractSerializer.WriteObject(xmlTextWriter, obj);
                    return stringWriter.ToString();
                }
            }
        }
    }
}
