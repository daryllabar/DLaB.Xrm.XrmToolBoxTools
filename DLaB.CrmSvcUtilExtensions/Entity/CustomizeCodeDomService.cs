using System;
using System.CodeDom;
using System.Collections.Generic;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Client.CodeGeneration;

namespace DLaB.CrmSvcUtilExtensions.Entity
{
    class CustomizeCodeDomService : ICustomizeCodeDomService
    {
        public static bool AddDebuggerNonUserCode { get { return ConfigHelper.GetAppSettingOrDefault("AddDebuggerNonUserCode", true); } }
        public static bool GenerateAnonymousTypeConstructor { get { return ConfigHelper.GetAppSettingOrDefault("GenerateAnonymousTypeConstructor", true); } }
        public static bool GenerateAttributeNameConsts { get { return ConfigHelper.GetAppSettingOrDefault("GenerateAttributeNameConsts", false); } }
        public static bool GenerateEnumProperties { get { return ConfigHelper.GetAppSettingOrDefault("GenerateEnumProperties", true); } }
        public static bool UseXrmClient { get { return ConfigHelper.GetAppSettingOrDefault("UseXrmClient", false); } }
        public IDictionary<string, string> Parameters { get; set; }

        public CustomizeCodeDomService(IDictionary<string, string> parameters)
        {
          Parameters = parameters;
        }

        #region ICustomizeCodeDomService Members

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            if (UseXrmClient)
            {
                new CodeCustomization(Parameters).CustomizeCodeDom(codeUnit, services);
            }
            if (GenerateAnonymousTypeConstructor)
            {
                new AnonymousTypeConstructorGenerator().CustomizeCodeDom(codeUnit, services);
            }
            if (GenerateAttributeNameConsts)
            {
                new AttributeConstGenerator().CustomizeCodeDom(codeUnit, services);
            }
            if (GenerateEnumProperties)
            {
                new EnumPropertyGenerator().CustomizeCodeDom(codeUnit, services);
            }
            if (AddDebuggerNonUserCode)
            {
                new MemberAttributes().CustomizeCodeDom(codeUnit, services);
            }
        }

        #endregion
    }
}
