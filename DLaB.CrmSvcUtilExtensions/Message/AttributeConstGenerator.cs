using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using System.CodeDom;
using System.Collections.Generic;

namespace DLaB.ModelBuilderExtensions.Message
{
    public class AttributeConstGenerator : AttributeConstGeneratorBase
    {
        public static string ActionLogicalFieldName => ConfigHelper.GetAppSettingOrDefault("ActionLogicalFieldName", "ActionLogicalName");
        public override bool GenerateAttributeNameConsts { get => DLaBSettings.GenerateActionAttributeNameConsts; set => DLaBSettings.GenerateActionAttributeNameConsts = value; }

        public AttributeConstGenerator(ICustomizeCodeDomService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
        }

        public AttributeConstGenerator(ICustomizeCodeDomService defaultService, DLaBModelBuilderSettings settings) : base(defaultService, settings)
        {
        }

        protected override void AddNonPropertyValues(CodeTypeDeclaration constantsClass, CodeTypeDeclaration type, HashSet<string> attributes)
        {
            var req = type.GetRequestProxyAttribute() ?? type.GetResponseProxyAttribute();
            AddAttributeConstToAction(type, new HashSet<string>(), req);
        }

        private void AddAttributeConstToAction(CodeTypeDeclaration type, HashSet<string> attributes, string req)
        {
            var index = AddAttributeConstIfNotExists(type, ActionLogicalFieldName, req, attributes);
            if (index >= 0)
            {
                var att = type.Members[index];
                type.Members.RemoveAt(index);
                type.Members.Insert(0, att);
            }
        }
    }
}