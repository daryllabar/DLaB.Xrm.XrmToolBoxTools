using System;
using System.CodeDom;
using Microsoft.Crm.Services.Utility;
namespace DLaB.CrmSvcUtilExtensions.OptionSet
{
    public class CustomizeCodeDomService : ICustomizeCodeDomService
    {
        #region ICustomizeCodeDomService Members

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            new CreateOptionSetEnums().CustomizeCodeDom(codeUnit, services);
            new LocalMultiOptionSetGenerator().CustomizeCodeDom(codeUnit, services);
        }

        #endregion
    }
}
