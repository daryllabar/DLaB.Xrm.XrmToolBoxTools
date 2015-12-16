using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.CrmSvcUtilExtensions.Entity
{
    public class MetadataProviderService: IMetadataProviderService
    {
        public bool MakeReadonlyFieldsEditable { get; }

        private IOrganizationMetadata Metadata { get; }

        public MetadataProviderService(IMetadataProviderService defaultService, IDictionary<string, string> paramters)
        {
            MakeReadonlyFieldsEditable = ConfigHelper.GetAppSettingOrDefault("MakeReadonlyFieldsEditable", false);
            Metadata = defaultService.LoadMetadata();
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
    }
}
