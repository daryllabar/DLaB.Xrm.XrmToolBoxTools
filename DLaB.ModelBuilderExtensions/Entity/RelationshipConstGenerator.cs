using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace DLaB.ModelBuilderExtensions.Entity
{
    public class RelationshipConstGenerator : AttributeConstGeneratorBase
    {
        protected const string XrmAttributeLogicalName = "Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute";
        protected const string XrmRelationshipSchemaName = "Microsoft.Xrm.Sdk.RelationshipSchemaNameAttribute";

        public override string AttributeConstsClassName { get => DLaBSettings.RelationshipConstsClassName; set => DLaBSettings.RelationshipConstsClassName = value; }
        public override bool GenerateAttributeNameConsts { get => Settings.EmitFieldsClasses ; set => Settings.EmitFieldsClasses = value; }
        public override int InsertIndex => 1;

        public RelationshipConstGenerator(ICustomizeCodeDomService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
        }

        public RelationshipConstGenerator(ICustomizeCodeDomService defaultService, DLaBModelBuilderSettings settings = null) : base(defaultService, settings)
        {
        }


        protected override string GetAttributeLogicalName(CodeMemberProperty prop)
        {
            var info = (from CodeAttributeDeclaration att in prop.CustomAttributes
                    where IsManyToMany(prop, att)
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


        protected string GenerateAttributeLogicalName(string fieldName, CodeMemberProperty prop, CodeAttributeDeclaration att)
        {
            return GetManyToManyName(fieldName, prop, att, "Referencing")
                   ?? GetManyToManyName(fieldName, prop, att, "Referenced")
                   ?? fieldName;
        }

        private static bool IsManyToMany(CodeTypeMember prop, CodeAttributeDeclaration att)
        {
            return att.AttributeType.BaseType == XrmRelationshipSchemaName
                   && prop.Comments?.Count > 1
                   && prop.Comments[1]?.Comment?.Text?.Trim().StartsWith("N:N ") == true;
        }

        private static string GetManyToManyName(string name, CodeTypeMember prop, CodeAttributeDeclaration att, string @ref)
        {
            return prop.Name.StartsWith(@ref)
                   && IsManyToMany(prop, att)
                ? @ref + RemovalString + name
                : null;
        }
    }
}