using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.CrmSvcUtilExtensions.Entity
{
    public class MetadataProviderService: IMetadataProviderService
    {
        public bool MakeReadonlyFieldsEditable { get; }

        private IOrganizationMetadata Metadata { get; }

        private string FilePath { get; set; }

        public MetadataProviderService(IMetadataProviderService defaultService, IDictionary<string, string> paramters)
        {
            FilePath = ConfigHelper.GetAppSettingOrDefault("SerializedMetadataFilePath", "metadata.xml");
            MakeReadonlyFieldsEditable = ConfigHelper.GetAppSettingOrDefault("MakeReadonlyFieldsEditable", false);
            if (ConfigHelper.GetAppSettingOrDefault("ReadSerializedMetadata", false))
            {
                Metadata = DeserializeMetadata(FilePath);
            }
            else
            {
                Metadata = defaultService.LoadMetadata();

                if (ConfigHelper.GetAppSettingOrDefault("SerializeMetadata", false))
                {
                    SerializeMetadata(Metadata, FilePath);
                }
            }
            
            var prop = typeof(AttributeMetadata).GetProperty("IsValidForCreate", BindingFlags.Public | BindingFlags.Instance);
            foreach (var att in Metadata.Entities.SelectMany(entity => entity.Attributes)) {
                switch (att.LogicalName)
                {
                    case "modifiedonbehalfby":
                    case "createdonbehalfby":
                    case "overriddencreatedon":
                            
                        prop.SetValue(att, true);
                        break;

                    case "createdby":
                    case "createdon":
                    case "modifiedby":
                    case "modifiedon":
                    case "owningbusinessunit":
                    case "owningteam":
                    case "owninguser":
                        if (MakeReadonlyFieldsEditable)
                        {
                            prop.SetValue(att, true);
                        }
                        break;
                }
            }
        }

        public IOrganizationMetadata LoadMetadata()
        {
            return Metadata;
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
