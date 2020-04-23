using System;
using System.CodeDom;
using Microsoft.Crm.Services.Utility;

namespace DLaB.CrmSvcUtilExtensions.Entity
{
    public class CustomizeCodeDomService : ICustomizeCodeDomService
    {
        public static bool AddDebuggerNonUserCode => ConfigHelper.GetAppSettingOrDefault("AddDebuggerNonUserCode", true);
        public static bool AddPrimaryAttributeConsts => ConfigHelper.GetAppSettingOrDefault("AddPrimaryAttributeConsts", true);
        public static bool CreateBaseClasses => ConfigHelper.GetAppSettingOrDefault("CreateBaseClasses", false);
        public static bool GenerateAnonymousTypeConstructor => ConfigHelper.GetAppSettingOrDefault("GenerateAnonymousTypeConstructor", true);
        public static bool GenerateAttributeNameConsts => ConfigHelper.GetAppSettingOrDefault("GenerateAttributeNameConsts", false);
        public static bool GenerateConstructorsSansLogicalName => ConfigHelper.GetAppSettingOrDefault("GenerateConstructorsSansLogicalName", false);
        public static bool GenerateEntityTypeCode => ConfigHelper.GetAppSettingOrDefault("GenerateEntityTypeCode", false);
        public static bool GenerateEnumProperties => ConfigHelper.GetAppSettingOrDefault("GenerateEnumProperties", true);
        public static bool GenerateOptionSetMetadataAttribute => ConfigHelper.GetAppSettingOrDefault("GenerateOptionSetMetadataAttribute", false);
        public static bool ReplaceOptionSetPropertiesWithEnum => ConfigHelper.GetAppSettingOrDefault("ReplaceOptionSetPropertiesWithEnum", true);

        public static bool UpdateMultiOptionSetAttributes => ConfigHelper.GetAppSettingOrDefault("UpdateMultiOptionSetAttributes", true);
        public static bool UpdateEnumerableEntityProperties => ConfigHelper.GetAppSettingOrDefault("UpdateEnumerableEntityProperties", true);

        #region ICustomizeCodeDomService Members

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            if (UpdateMultiOptionSetAttributes)
            {
                new MultiOptionSetAttributeUpdater().CustomizeCodeDom(codeUnit, services);
            }
            if (UpdateEnumerableEntityProperties)
            {
                new EnumerableEntityPropertyUpdater().CustomizeCodeDom(codeUnit, services);
            }
            if (AddPrimaryAttributeConsts)
            {
                new PrimaryAttributeGenerator().CustomizeCodeDom(codeUnit, services);
            }
            if (GenerateConstructorsSansLogicalName)
            {
                new EntityConstructorsGenerator().CustomizeCodeDom(codeUnit, services);
            }
            if (GenerateAttributeNameConsts)
            {
                new AttributeConstGenerator().CustomizeCodeDom(codeUnit, services);
            }
            if (GenerateAnonymousTypeConstructor)
            {
                new AnonymousTypeConstructorGenerator().CustomizeCodeDom(codeUnit, services);
            }
            if (!GenerateEntityTypeCode)
            {
                new RemoveEntityTypeCodeService().CustomizeCodeDom(codeUnit, services);
            }

            var multiSelectCreated = false;
            if (GenerateEnumProperties)
            {
                var generator = new EnumPropertyGenerator(CreateBaseClasses, ReplaceOptionSetPropertiesWithEnum);
                generator.CustomizeCodeDom(codeUnit, services);
                multiSelectCreated = generator.MultiSelectEnumCreated;
            }
        
            if (GenerateOptionSetMetadataAttribute)
            {
                new OptionSetMetadataAttributeGenerator().CustomizeCodeDom(codeUnit, services);
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
