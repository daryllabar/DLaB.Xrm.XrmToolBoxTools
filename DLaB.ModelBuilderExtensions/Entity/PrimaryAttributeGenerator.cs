// ReSharper disable BitwiseOperatorOnEnumWithoutFlags
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace DLaB.ModelBuilderExtensions.Entity
{
    public class PrimaryAttributeGenerator : TypedServiceBase<ICustomizeCodeDomService>, ICustomizeCodeDomService
    {

        public PrimaryAttributeGenerator(ICustomizeCodeDomService defaultService, DLaBModelBuilderSettings settings = null) : base(defaultService, settings)
        {
        }

        public PrimaryAttributeGenerator(ICustomizeCodeDomService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
        }

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)   
        {
            SetServiceCache(services);

            foreach (var entity in codeUnit.GetEntityTypes(ServiceCache.EntityMetadataByLogicalName).Select(i=> new {Type = i.Item1, Metadata = i.Item2}))
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
                    var value = GenerateAlternateKeyValue(entity.Metadata.Keys);
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

        public static string GenerateAlternateKeyValue(IEnumerable<EntityKeyMetadata> keys)
        {
            return string.Join("|", keys.Select(k => string.Join(",", k.KeyAttributes.OrderBy(a => a.ToLower()))).OrderBy(k => k.ToLower()));
        }
    }
}