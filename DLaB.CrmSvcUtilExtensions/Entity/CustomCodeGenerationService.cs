using Microsoft.Crm.Services.Utility;

namespace DLaB.CrmSvcUtilExtensions.Entity
{
    public class CustomCodeGenerationService : BaseCustomCodeGenerationService
    {
        protected override bool CreateOneFilePerCodeUnit
        {
            get { return ConfigHelper.GetAppSettingOrDefault("CreateOneFilePerEntity", false); }
        }

        public CustomCodeGenerationService(ICodeGenerationService service) : base(service) {}
    }
}
