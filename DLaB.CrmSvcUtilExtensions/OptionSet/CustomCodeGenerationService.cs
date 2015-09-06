using Microsoft.Crm.Services.Utility;

namespace DLaB.CrmSvcUtilExtensions.OptionSet
{
    public class CustomCodeGenerationService : BaseCustomCodeGenerationService
    {
        public CustomCodeGenerationService(ICodeGenerationService service) : base(service) {}

        protected override bool CreateOneFilePerCodeUnit
        {
            get { return ConfigHelper.GetAppSettingOrDefault("CreateOneFilePerOptionSet", false); }
        }

        protected override CodeUnit SplitByCodeUnit
        {
            get { return CodeUnit.Enum; }
        }
    }
}
