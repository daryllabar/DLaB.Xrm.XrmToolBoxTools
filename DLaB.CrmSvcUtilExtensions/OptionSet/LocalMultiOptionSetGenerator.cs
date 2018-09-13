using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.CrmSvcUtilExtensions.OptionSet
{
    /// <summary>
    /// For some reason, CrmSvcUtil doesn't generate enums for Local MultiSelectOptionSets
    /// </summary>
    public class LocalMultiOptionSetGenerator : ICustomizeCodeDomService
    {
        #region Implementation of ICustomizeCodeDomService

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            var metadata = ((IMetadataProviderService) services.GetService(typeof(IMetadataProviderService))).LoadMetadata();
            var namingService = (INamingService) services.GetService(typeof(INamingService));
            foreach (var entity in metadata.Entities)
            {
                foreach (var attribute in entity.Attributes.Where(a => a.AttributeType == AttributeTypeCode.Virtual 
                                                                       && a is MultiSelectPicklistAttributeMetadata enumMeta
                                                                       && enumMeta.OptionSet.IsGlobal == false))
                {
                    codeUnit.Namespaces[0].Types.Add(GenerateEnum(entity, (MultiSelectPicklistAttributeMetadata) attribute, services, namingService));
                }
            }
        }

        #endregion

        private CodeTypeDeclaration GenerateEnum(EntityMetadata entityMetadata, MultiSelectPicklistAttributeMetadata metadata, IServiceProvider services, INamingService service)
        {
            var name = service.GetNameForOptionSet(entityMetadata, metadata.OptionSet, services);
            var type = new CodeTypeDeclaration(name)
            {
                IsEnum = true
            };
            type.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(DataContractAttribute))));

            var version = System.Diagnostics.FileVersionInfo.GetVersionInfo(typeof(INamingService).Assembly.Location).ProductVersion;
            type.CustomAttributes.Add(new CodeAttributeDeclaration(
                new CodeTypeReference(typeof(GeneratedCodeAttribute)), 
                new CodeAttributeArgument(new CodePrimitiveExpression("CrmSvcUtil")),
                new CodeAttributeArgument(new CodePrimitiveExpression(version))));

            foreach (var option in metadata.OptionSet.Options)
            {
                // Creates the enum member
                CodeMemberField value = new CodeMemberField
                {
                    Name = service.GetNameForOption(metadata.OptionSet, option, services),
                    InitExpression = new CodePrimitiveExpression(option.Value.GetValueOrDefault())
                };
                value.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(EnumMemberAttribute))));

                type.Members.Add(value);
            }

            return type;
        }
    }
}
