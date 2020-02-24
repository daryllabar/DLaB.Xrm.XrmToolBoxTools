// ReSharper disable BitwiseOperatorOnEnumWithoutFlags
using Microsoft.Crm.Services.Utility;
using System;
using System.CodeDom;
using System.Linq;

namespace DLaB.CrmSvcUtilExtensions.Entity
{
    public class PrimaryAttributeGenerator : ICustomizeCodeDomService
    {
        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)   
        {
            var metadataService = (IMetadataProviderService)services.GetService(typeof(IMetadataProviderService));
            var metadata = metadataService.LoadMetadata();
            var types = codeUnit.Namespaces[0].Types;

            foreach (var type in types.Cast<CodeTypeDeclaration>().
                                 Where(type => type.IsClass && !type.IsContextType()))
            {
                var entityMetadata = metadata.Entities.First(e => e.LogicalName == ((CodePrimitiveExpression)(type.CustomAttributes.Cast<CodeAttributeDeclaration>()
                                                                                        .First(a => a.Name == "Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute")
                                                                                        .Arguments[0].Value)).Value.ToString());


                Console.WriteLine($"objecttypecode={entityMetadata.ObjectTypeCode}");
                CodeMemberField entitytypecodefield = null;

                foreach (var member in type.Members)
                {
                    if (member.GetType().ToString() == "System.CodeDom.CodeMemberField") {
                        var f = (CodeMemberField)member;
                        if (f.Name == "EntityTypeCode")
                        {
                            entitytypecodefield = f;
                        }
                        Console.WriteLine($"FieldName={f.Name}");
                    }
                    Console.WriteLine(member.GetType());
                }
                type.Members.Remove(entitytypecodefield);


                // insert at 2, to be after the constructor and the entity logical name
                if (entityMetadata.PrimaryNameAttribute != null)
                {
                    type.Members.Insert(2,
                        new CodeMemberField
                        {
                            
                            Attributes = System.CodeDom.MemberAttributes.Public | System.CodeDom.MemberAttributes.Const,
                            Name = "PrimaryNameAttribute",
                            Type = new CodeTypeReference(typeof(string)),
                            InitExpression = new CodePrimitiveExpression(entityMetadata.PrimaryNameAttribute)
                        });
                }
                type.Members.Insert(2, 
                    new CodeMemberField
                    {
                        Attributes = System.CodeDom.MemberAttributes.Public | System.CodeDom.MemberAttributes.Const,
                        Name = "PrimaryIdAttribute",
                        Type = new CodeTypeReference(typeof(string)),
                        InitExpression = new CodePrimitiveExpression(entityMetadata.PrimaryIdAttribute)
                    });

                type.Members.Insert(2,
                                    new CodeMemberField
                                    {
                                        Attributes = System.CodeDom.MemberAttributes.Public | System.CodeDom.MemberAttributes.Const,
                                        Name = "EntitySchemaName",
                                        Type = new CodeTypeReference(typeof(string)),
                                        InitExpression = new CodePrimitiveExpression(entityMetadata.SchemaName)
                                    });

                Console.Write("Hello");
                Console.Write($"type.name={type.Name}");
            }
        }
    }
}