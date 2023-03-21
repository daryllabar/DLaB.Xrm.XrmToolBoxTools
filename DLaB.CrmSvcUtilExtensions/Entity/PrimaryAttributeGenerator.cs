// ReSharper disable BitwiseOperatorOnEnumWithoutFlags
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using System;
using System.CodeDom;
using System.Linq;

namespace DLaB.ModelBuilderExtensions.Entity
{
    public class PrimaryAttributeGenerator : ICustomizeCodeDomService
    {
        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)   
        {
            foreach (var entity in codeUnit.GetEntityTypes(services).Select(i=> new {Type = i.Item1, Metadata = i.Item2}))
            {
                // insert at 2, to be after the constructor and the entity logical name
                entity.Type.Members.Insert(2,
                    new CodeMemberField
                    {
                        Attributes = System.CodeDom.MemberAttributes.Public | System.CodeDom.MemberAttributes.Const,
                        Name = "EntitySchemaName",
                        Type = new CodeTypeReference(typeof(string)),
                        InitExpression = new CodePrimitiveExpression(entity.Metadata.SchemaName)
                    });

                if (entity.Metadata.PrimaryNameAttribute != null)
                {
                    entity.Type.Members.Insert(2,
                        new CodeMemberField
                        {
                            Attributes = System.CodeDom.MemberAttributes.Public | System.CodeDom.MemberAttributes.Const,
                            Name = "PrimaryNameAttribute",
                            Type = new CodeTypeReference(typeof(string)),
                            InitExpression = new CodePrimitiveExpression(entity.Metadata.PrimaryNameAttribute)
                        });
                }

                entity.Type.Members.Insert(2, 
                    new CodeMemberField
                    {
                        Attributes = System.CodeDom.MemberAttributes.Public | System.CodeDom.MemberAttributes.Const,
                        Name = "PrimaryIdAttribute",
                        Type = new CodeTypeReference(typeof(string)),
                        InitExpression = new CodePrimitiveExpression(entity.Metadata.PrimaryIdAttribute)
                    });

                if (entity.Metadata.Keys != null && entity.Metadata.Keys.Length > 0)
                {
                    var value = string.Join("|", entity.Metadata.Keys.Select(k => string.Join(",", k.KeyAttributes)));
                    entity.Type.Members.Insert(1,
                        new CodeMemberField
                        {
                            Attributes = System.CodeDom.MemberAttributes.Public | System.CodeDom.MemberAttributes.Const,
                            Name = "AlternateKeys",
                            Type = new CodeTypeReference(typeof(string)),
                            InitExpression = new CodePrimitiveExpression(value)
                        });
                }
            }
        }
    }
}