using System;
using System.IO;
using Microsoft.Crm.Services.Utility;

namespace DLaB.CrmSvcUtilExtensions.Action
{
    public class CustomCodeGenerationService : BaseCustomCodeGenerationService
    {
        public CustomCodeGenerationService(ICodeGenerationService service) : base(service) {}

        protected override string CommandLineText
        {
            get { return ConfigHelper.GetAppSettingOrDefault("ActionCommandLineText", string.Empty); }
        }

        protected override bool CreateOneFilePerCodeUnit
        {
            get { return ConfigHelper.GetAppSettingOrDefault("CreateOneFilePerAction", false); }
        }

        protected override void WriteInternal(IOrganizationMetadata organizationMetadata, string language, string outputFile, string targetNamespace, IServiceProvider services)
        {
            base.WriteInternal(organizationMetadata, language, outputFile, targetNamespace, services);
            // Since no Actions.cs class gets created by default, they are all request / response classes, delete the file if there is no CommandLineText to be added
            if (!CreateOneFilePerCodeUnit && string.IsNullOrEmpty(CommandLineText))
            {
                File.Delete(outputFile);
            }
        }
    }
}
