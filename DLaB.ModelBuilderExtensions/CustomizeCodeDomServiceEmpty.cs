using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using System;
using System.CodeDom;

namespace DLaB.ModelBuilderExtensions
{
    internal class CustomizeCodeDomServiceEmpty : ICustomizeCodeDomService
    {
        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
        }
    }
}
