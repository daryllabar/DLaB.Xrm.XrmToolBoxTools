using System;
using System.CodeDom;
using System.Collections.Generic;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
namespace DLaB.ModelBuilderExtensions.OptionSet
{
    public class CustomizeCodeDomService : ICustomizeCodeDomService
    {
        private IDictionary<string, string> _parameters;

        #region ICustomizeCodeDomService Members

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
        }

        #endregion

        public CustomizeCodeDomService(IDictionary<string, string> parameters)
        {
            _parameters = parameters;
        }
    }
}
