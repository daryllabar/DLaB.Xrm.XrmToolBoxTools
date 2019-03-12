using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.CrmSvcUtilExtensions.Entity
{
    public class MetadataProviderService: BaseMetadataProviderService
    {
        public bool MakeReadonlyFieldsEditable { get; }
        public bool MakeAllFieldsEditable { get; }

        public MetadataProviderService(IMetadataProviderService defaultService, IDictionary<string, string> parameters) : base (defaultService, parameters)
        {
            MakeReadonlyFieldsEditable = ConfigHelper.GetAppSettingOrDefault("MakeReadonlyFieldsEditable", false);
            MakeAllFieldsEditable = ConfigHelper.GetAppSettingOrDefault("MakeAllFieldsEditable", false);
        }

        protected override IOrganizationMetadata LoadMetadataInternal()
        {
            var metadata = base.LoadMetadataInternal();
            var prop = typeof(AttributeMetadata).GetProperty("IsValidForCreate", BindingFlags.Public | BindingFlags.Instance);
            foreach (var att in metadata.Entities.SelectMany(entity => entity.Attributes))
            {
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
                    case "statecode":
                        att.SchemaName = "StateCode";
                        break;
                    case "statuscode":
                        att.SchemaName = "StatusCode";
                        break;
                }

                if (MakeAllFieldsEditable)
                {
                    if (att.IsValidForCreate != true)
                    {
                        prop.SetValue(att, true);
                    }
                }
            }
            return metadata;
        }
    }
}
