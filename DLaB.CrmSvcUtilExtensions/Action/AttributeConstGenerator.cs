using System.CodeDom;

namespace DLaB.CrmSvcUtilExtensions.Action
{
    public class AttributeConstGenerator : AttributeConstGeneratorBase
    {
        protected override string GetAttributeLogicalName(CodeMemberProperty prop)
        {
            return prop.Name;
        }
        
    }
}