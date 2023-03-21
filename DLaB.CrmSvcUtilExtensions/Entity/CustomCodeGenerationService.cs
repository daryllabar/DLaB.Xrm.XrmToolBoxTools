using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using System.Collections.Generic;

namespace DLaB.ModelBuilderExtensions.Entity
{
    public class CustomCodeGenerationService : BaseCustomCodeGenerationService
    {
        protected override bool CreateOneFilePerCodeUnit => DLaBSettings.CreateOneFilePerEntity;

        protected override List<string> ClassesToMakeStatic
        {
            get
            { 
                var value = new List<string>();
                if (DLaBSettings.GenerateOptionSetMetadataAttribute)
                {
                    value.Add("OptionSetExtension");
                }
                return value;
            }
        }

        public CustomCodeGenerationService(ICodeGenerationService service, IDictionary<string, string> parameters) : base(service, parameters)
        {
        }

        public CustomCodeGenerationService(ICodeGenerationService service, DLaBModelBuilderSettings settings) : base(service, settings)
        {
        }
    }
}
