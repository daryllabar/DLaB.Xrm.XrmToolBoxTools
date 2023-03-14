using System.CodeDom;

namespace DLaB.ModelBuilderExtensions.Entity
{
    public class RelationshipConstGenerator : AttributeConstGenerator
    {
        public static string RelationshipConstsClassName => ConfigHelper.GetAppSettingOrDefault("RelationshipConstsClassName", "Relationships");

        protected override string GetCodeTypeName()
        {
            return RelationshipConstsClassName;
        }

        protected override bool IsConstGeneratingAttribute(CodeMemberProperty prop, CodeAttributeDeclaration att)
        {
            return IsManyToMany(prop, att);
        }

        protected override string GenerateAttributeLogicalName(string fieldName, CodeMemberProperty prop, CodeAttributeDeclaration att)
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