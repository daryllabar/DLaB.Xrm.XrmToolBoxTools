using Microsoft.Crm.Services.Utility;

namespace DLaB.CrmSvcUtilExtensions.OptionSet
{
    public class CustomCodeGenerationService : BaseCustomCodeGenerationService
    {
        public CustomCodeGenerationService(ICodeGenerationService service) : base(service) {}

        protected override string CommandLineText => ConfigHelper.GetAppSettingOrDefault("OptionSetCommandLineText", string.Empty);

        protected override bool CreateOneFilePerCodeUnit => ConfigHelper.GetAppSettingOrDefault("CreateOneFilePerOptionSet", false);

        protected override CodeUnit SplitByCodeUnit => CodeUnit.Enum;
    }
}
