using System;
using System.CodeDom;
using System.Collections.Generic;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Client.CodeGeneration;

namespace DLaB.CrmSvcUtilExtensions.Entity
{
    public class CustomizeCodeDomService : ICustomizeCodeDomService
    {
        public static bool AddDebuggerNonUserCode => ConfigHelper.GetAppSettingOrDefault("AddDebuggerNonUserCode", true);
        public static bool AddPrimaryAttributeConsts => ConfigHelper.GetAppSettingOrDefault("AddPrimaryAttributeConsts", true);
        public bool CreateBaseClasses => ConfigHelper.GetAppSettingOrDefault("CreateBaseClasses", false) && !UseXrmClient;
        public static bool GenerateAnonymousTypeConstructor => ConfigHelper.GetAppSettingOrDefault("GenerateAnonymousTypeConstructor", true);
        public static bool GenerateAttributeNameConsts => ConfigHelper.GetAppSettingOrDefault("GenerateAttributeNameConsts", false);
        public static bool GenerateEnumProperties => ConfigHelper.GetAppSettingOrDefault("GenerateEnumProperties", true);
        public static bool UseXrmClient => ConfigHelper.GetAppSettingOrDefault("UseXrmClient", false);
        public IDictionary<string, string> Parameters { get; set; }

        public CustomizeCodeDomService(IDictionary<string, string> parameters)
        {
          Parameters = parameters;
        }

        #region ICustomizeCodeDomService Members

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {

            new MultiOptionSetAttributeUpdater().CustomizeCodeDom(codeUnit, services);
            if (UseXrmClient)
            {
                new CodeCustomization(Parameters).CustomizeCodeDom(codeUnit, services);
            }
            if (AddPrimaryAttributeConsts)
            {
                new PrimaryAttributeGenerator().CustomizeCodeDom(codeUnit, services);
            }
            if (GenerateAttributeNameConsts)
            {
                new AttributeConstGenerator().CustomizeCodeDom(codeUnit, services);
            }
            if (GenerateAnonymousTypeConstructor)
            {
                new AnonymousTypeConstructorGenerator().CustomizeCodeDom(codeUnit, services);
            }

            var multiSelectCreated = false;
            if (GenerateEnumProperties)
            {
                var generator = new EnumPropertyGenerator(CreateBaseClasses);
                generator.CustomizeCodeDom(codeUnit, services);
                multiSelectCreated = generator.MultiSelectEnumCreated;
            }
            if (CreateBaseClasses)
            {
                new EntityBaseClassGenerator(multiSelectCreated).CustomizeCodeDom(codeUnit, services);
            }
            if (AddDebuggerNonUserCode)
            {
                new MemberAttributes().CustomizeCodeDom(codeUnit, services);
            }
        }

        #endregion
    }
}
