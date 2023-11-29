using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml;

namespace DLaB.ModelBuilderExtensions
{
    public class MetadataProviderService : TypedServiceBase<IMetadataProviderService>, IMetadataProviderService
    {
        private IOrganizationMetadata Metadata { get; set; }
        public IOrganizationService ServiceConnection { get; set; }
        public bool IsLiveConnectionRequired => !ReadSerializedMetadata;

        public string FilePath { get => DLaBSettings.SerializedMetadataRelativeFilePath; set => DLaBSettings.SerializedMetadataRelativeFilePath = value; }
        public bool MakeReadonlyFieldsEditable { get => DLaBSettings.MakeReadonlyFieldsEditable; set => DLaBSettings.MakeReadonlyFieldsEditable = value; }
        public bool MakeAllFieldsEditable { get => DLaBSettings.MakeAllFieldsEditable; set => DLaBSettings.MakeAllFieldsEditable = value; }
        public bool ReadSerializedMetadata { get => DLaBSettings.ReadSerializedMetadata; set => DLaBSettings.ReadSerializedMetadata = value; }
        public bool WriteMetadata { get => DLaBSettings.SerializeMetadata; set => DLaBSettings.SerializeMetadata = value; }

        public MetadataProviderService(IMetadataProviderService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
            WaitForDebugger();
        }

        public MetadataProviderService(IMetadataProviderService defaultService, DLaBModelBuilderSettings settings = null) : base(defaultService, settings)
        {
            WaitForDebugger();
        }

        private void WaitForDebugger()
        {
            if (!DLaBSettings.WaitForAttachedDebugger)
            {
                return;
            }

            while (!Debugger.IsAttached)
            {
                Console.WriteLine("[**** Waiting For Debugger ****]");
                Thread.Sleep(1000);
            }
            Console.WriteLine("[**** Debugger Attached.  Starting Generation. ****]");
        }

        public IOrganizationMetadata LoadMetadata(IServiceProvider service)
        {
            return Metadata ?? (Metadata = LoadMetadataInternal(service));
        }

        private IOrganizationMetadata LoadMetadataInternal(IServiceProvider service)
        {
            try
            {
                DefaultService.ServiceConnection = ServiceConnection;

                IOrganizationMetadata metadata;
                if (ReadSerializedMetadata)
                {
                    metadata = DeserializeMetadata(FilePath);
                }
                else
                {
                    metadata = DefaultService.LoadMetadata(service);

                    if (WriteMetadata)
                    {
                        SerializeMetadata(metadata, FilePath);
                    }
                }

                UpdateEntityMetadata(metadata);
                return metadata;
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                throw;
            }
        }

        private void UpdateEntityMetadata(IOrganizationMetadata metadata)
        {
            MakeReadonlyEntityAttributesEditable(metadata);
        }

        private void MakeReadonlyEntityAttributesEditable(IOrganizationMetadata metadata)
        {
            var prop = typeof(AttributeMetadata).GetProperty("IsValidForCreate", BindingFlags.Public | BindingFlags.Instance);
            if (prop == null)
            {
                throw new NotImplementedException("No IsValidForCreate public instance property for type AttributeMetadata!  Unable to update entity metadata.");
            }

            foreach (var att in metadata.Entities.SelectMany(entity => entity.Attributes))
            {
                switch (att.LogicalName)
                {
                    // ReSharper disable StringLiteralTypo
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
                        if (MakeReadonlyFieldsEditable || MakeAllFieldsEditable)
                        {
                            prop.SetValue(att, true);
                        }

                        break;
                    case "statecode":
                        att.SchemaName = "StateCode";
                        break;
                    case "statuscode":
                        att.SchemaName = "StatusCode";
                        break;
                    // ReSharper restore StringLiteralTypo
                }

                if (MakeAllFieldsEditable
                    && att.IsValidForCreate != true)
                {
                    prop.SetValue(att, true);
                }
            }
        }

        public static void SerializeMetadata(IOrganizationMetadata metadata, string filePath)
        {
            var localMetadata = new Metadata
            {
                Entities = metadata.Entities,
                OptionSets = metadata.OptionSets,
                Messages = metadata.Messages
            };

            filePath = filePath.RootPath();
            Console.WriteLine("[**** Writing Metadata to File {0} ****]", filePath);
            File.WriteAllText(filePath, Serialize(localMetadata,true));
            Console.WriteLine("[**** Finished Writing Metadata ****]", filePath);
        }

        public static IOrganizationMetadata DeserializeMetadata(string filePath)
        {
            return DeserializeDataObject<Metadata>(File.ReadAllText(filePath.RootPath()));
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
            var contractSerializer = new DataContractSerializer(obj.GetType());
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
