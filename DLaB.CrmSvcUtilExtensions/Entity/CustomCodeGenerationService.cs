using System;
using System.Collections.Generic;
using System.Windows.Documents;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;

namespace DLaB.ModelBuilderExtensions.Entity
{
    public class CustomCodeGenerationService : BaseCustomCodeGenerationService
    {
        protected override bool CreateOneFilePerCodeUnit => ConfigHelper.GetAppSettingOrDefault("CreateOneFilePerEntity", false);

        protected override List<string> ClassesToMakeStatic
        {
            get
            { 
                var value = new List<string>();
                if (CustomizeCodeDomService.GenerateOptionSetMetadataAttribute)
                {
                    value.Add("OptionSetExtension");
                }
                return value;
            }
        }

        public CustomCodeGenerationService(ICodeGenerationService service) : base(service) {}
    }
}
