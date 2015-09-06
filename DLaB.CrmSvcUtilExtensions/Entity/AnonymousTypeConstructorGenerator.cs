using Microsoft.Crm.Services.Utility;
using System;
using System.CodeDom;

namespace DLaB.CrmSvcUtilExtensions.Entity
{
    class AnonymousTypeConstructorGenerator : ICustomizeCodeDomService
    {
        #region ICustomizeCodeDomService Members

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            var types = codeUnit.Namespaces[0].Types;
            foreach (CodeTypeDeclaration type in types)
            {
                if (!type.IsClass || type.IsContextType()) { continue; }

                type.Members.Add(GetAnonymousTypeConstructor(type));
            }
        }

        #endregion

        private CodeConstructor GetAnonymousTypeConstructor(CodeTypeDeclaration type)
        {
            var data = CodeWriterFilterService.EntityMetadata[type.GetFieldInitalizedValue("EntityLogicalName")];
            var constructor = new CodeConstructor
            {
                Attributes = System.CodeDom.MemberAttributes.Public,
                Name = type.Name
            };

            constructor.Comments.AddRange(new[] {
					new CodeCommentStatement(@"<summary>", true),
					new CodeCommentStatement(@"Constructor for populating via LINQ queries given a LINQ anonymous type", true),
					new CodeCommentStatement(@"<param name=""anonymousType"">LINQ anonymous type.</param>",true),
					new CodeCommentStatement(@"</summary>", true)});


            constructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof (Object), "anonymousType"));
            constructor.ChainedConstructorArgs.Add(new CodeSnippetExpression(""));
            const string indent = "            ";
            // Rather than attempt to do this all through CodeDom, hard code this as C#
            constructor.Statements.Add(new CodeSnippetStatement(String.Format(indent +
            "foreach (var p in anonymousType.GetType().GetProperties()){0}" +
            "{{{0}" +
            "    var value = p.GetValue(anonymousType, null);{0}" +
            "    var name = p.Name.ToLower();{0}{0}" +
            "    if (name.EndsWith(\"enum\") && value.GetType().BaseType == typeof(System.Enum)){0}" +
            "    {{{0}" +
            "        value = new Microsoft.Xrm.Sdk.OptionSetValue((int) value);{0}" +
            "        name = name.Remove(name.Length - \"enum\".Length);{0}" +
            "    }}{0}{0}" +
            "    switch (name){0}" +
            "    {{{0}" +
            "        case \"id\":{0}" +
            "            base.Id = (System.Guid)value;{0}" +
            "            Attributes[\"{1}\"] = base.Id;{0}" +
            "            break;{0}" +
            "        case \"{1}\":{0}" +
            "            var id = (System.Nullable<System.Guid>) value;{0}" +
            "            if(id == null){{ continue; }}{0}" +
            "            base.Id = id.Value;{0}" +
            "            Attributes[name] = base.Id;{0}" +
            "            break;{0}" +
            "        case \"formattedvalues\":{0}" +
            "            // Add Support for FormattedValues{0}" +
            "            FormattedValues.AddRange((Microsoft.Xrm.Sdk.FormattedValueCollection)value);{0}" +
            "            break;{0}" +
            "        default:{0}" +
            "            Attributes[name] = value;{0}" +
            "            break;{0}" +
            "    }}{0}" +
            "}}", Environment.NewLine + indent, data.PrimaryIdAttribute)));

            return constructor;
        }
    }
}
