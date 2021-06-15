using System.CodeDom;
using System.Collections.Generic;

namespace DLaB.CrmSvcUtilExtensions.Action
{
    public class AttributeConstGenerator : AttributeConstGeneratorBase
    {
        public static string ActionLogicalFieldName => ConfigHelper.GetAppSettingOrDefault("ActionLogicalFieldName", "ActionLogicalName");

        protected override string GetAttributeLogicalName(CodeMemberProperty prop)
        {
            return prop.Name;
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