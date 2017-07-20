using System.CodeDom;
using System.Linq;

namespace DLaB.CrmSvcUtilExtensions.Entity
{
    public class AttributeConstGenerator : AttributeConstGeneratorBase
    {
        private const string XrmAttributeLogicalName = "Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute";
        private const string XrmRelationshipSchemaName = "Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute";

        protected override string GetAttributeLogicalName(CodeMemberProperty prop)
        {
            return (from CodeAttributeDeclaration att in prop.CustomAttributes
                    where att.AttributeType.BaseType == XrmAttributeLogicalName || HasAttributeAndRelationship(prop, att)
                    select new
                    {
                        FieldName = ((CodePrimitiveExpression)att.Arguments[0].Value).Value.ToString(),
                        Order = att.AttributeType.BaseType == XrmRelationshipSchemaName ? 0 : 1
                    }).
                                        OrderBy(a => a.Order).
                                        FirstOrDefault()?.FieldName;
        }


        private static bool HasAttributeAndRelationship(CodeMemberProperty prop, CodeAttributeDeclaration att)
        {
            return att.AttributeType.BaseType == XrmRelationshipSchemaName 
                && prop.CustomAttributes.Cast<CodeAttributeDeclaration>().Any(a => a.AttributeType.BaseType == XrmAttributeLogicalName);
        }
    }
}