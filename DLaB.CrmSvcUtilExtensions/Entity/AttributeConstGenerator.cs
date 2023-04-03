using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace DLaB.CrmSvcUtilExtensions.Entity
{
    public class AttributeConstGenerator : AttributeConstGeneratorBase
    {
        protected const string XrmAttributeLogicalName = "Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute";
        protected const string XrmRelationshipSchemaName = "Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute";

        protected override string GetAttributeLogicalName(CodeMemberProperty prop)
        {
            var info = (from CodeAttributeDeclaration att in prop.CustomAttributes
                    where IsConstGeneratingAttribute(prop, att)
                    select new
                    {
                        FieldName = ((CodePrimitiveExpression)att.Arguments[0].Value).Value.ToString(),
                        Order = att.AttributeType.BaseType == XrmRelationshipSchemaName ? 0 : 1,
                        Att = att
                    })
                .OrderBy(a => a.Order)
                .FirstOrDefault();

            return info == null
                ? null
                : GenerateAttributeLogicalName(info.FieldName, prop, info.Att);
        }

        protected override void AddNonPropertyValues(CodeTypeDeclaration constantsClass, CodeTypeDeclaration type, HashSet<string> attributes)
        {
            // None
        }

        protected virtual bool IsConstGeneratingAttribute(CodeMemberProperty prop, CodeAttributeDeclaration att)
        {
            return att.AttributeType.BaseType == XrmAttributeLogicalName
                   || HasAttributeAndRelationship(prop, att);
        }

        protected virtual string GenerateAttributeLogicalName(string fieldName, CodeMemberProperty prop, CodeAttributeDeclaration att)
        {
            return fieldName;
        }


        private static bool HasAttributeAndRelationship(CodeMemberProperty prop, CodeAttributeDeclaration att)
        {
            return att?.AttributeType.BaseType == XrmRelationshipSchemaName 
                && prop.CustomAttributes.Cast<CodeAttributeDeclaration>().Any(a => a.AttributeType.BaseType == XrmAttributeLogicalName);
        }
    }
}