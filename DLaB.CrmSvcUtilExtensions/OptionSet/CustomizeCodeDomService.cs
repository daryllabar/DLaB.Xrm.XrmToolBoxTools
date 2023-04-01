using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace DLaB.ModelBuilderExtensions.OptionSet
{
    public class CustomizeCodeDomService : TypedServiceBase<ICustomizeCodeDomService>, ICustomizeCodeDomService
    {
        public bool AddOptionSetMetadataAttribute { get => DLaBSettings.AddOptionSetMetadataAttribute; set => DLaBSettings.AddOptionSetMetadataAttribute = value; }

        #region Constructors

        public CustomizeCodeDomService(ICustomizeCodeDomService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        { }

        public CustomizeCodeDomService(ICustomizeCodeDomService defaultService, DLaBModelBuilderSettings settings = null) : base(defaultService, settings)
        { }

        #endregion Constructors

        /// <summary>
        /// Remove the unnecessary classes that we generated for entities. 
        /// </summary>
        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services) 
        {
            SetServiceCache(services);
            //Trace.TraceInformation("Entering ICustomizeCodeDomService.CustomizeCodeDom");
            //Trace.TraceInformation("Number of Namespaces generated: {0}", codeUnit.Namespaces.Count);

            //RemoveNonOptionSetDefinitions(codeUnit);
            AddLocalMultiSelectOptionSets(codeUnit, services);
            AddMetadataAttributes(codeUnit);
            SortOptionSets(codeUnit);
            //Trace.TraceInformation("Exiting ICustomizeCodeDomService.CustomizeCodeDom");
        }

        /// <summary>
        /// For some reason, CrmSvcUtil doesn't generate enums for Local MultiSelectOptionSets
        /// </summary>
        /// <param name="codeUnit"></param>
        /// <param name="services"></param>
        public void AddLocalMultiSelectOptionSets(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            var namingService = (INamingService)services.GetService(typeof(INamingService));
            foreach (var entityLogicalName in codeUnit.GetEntityTypes().Select(t => t.GetEntityLogicalName()))
            {
                if (!ServiceCache.LocalMultiSelectOptionSetAttributesByEntityLogicalName.TryGetValue(entityLogicalName, out var attributes))
                {
                    continue;
                }

                foreach (var attribute in attributes)
                {
                    var type = GenerateEnum(ServiceCache.EntityMetadataByLogicalName[entityLogicalName], attribute, services, namingService);
                    AddMetadataAttributesForSet(type, attribute.OptionSet);
                    codeUnit.Namespaces[0].Types.Add(type);
                }
            }
        }

        private CodeTypeDeclaration GenerateEnum(EntityMetadata entityMetadata, MultiSelectPicklistAttributeMetadata metadata, IServiceProvider services, INamingService service)
        {
            var name = service.GetNameForOptionSet(entityMetadata, metadata.OptionSet, services);
            var type = new CodeTypeDeclaration(name)
            {
                IsEnum = true
            };
            type.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(DataContractAttribute))));

            //if (!SuppressGeneratedCodeAttribute)
            //{
            //    var version = System.Diagnostics.FileVersionInfo.GetVersionInfo(typeof(INamingService).Assembly.Location).ProductVersion;
            //    type.CustomAttributes.Add(new CodeAttributeDeclaration(
            //        new CodeTypeReference(typeof(GeneratedCodeAttribute)), 
            //        new CodeAttributeArgument(new CodePrimitiveExpression("CrmSvcUtil")),
            //        new CodeAttributeArgument(new CodePrimitiveExpression(version))));
            //}

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

        private void AddMetadataAttributes(CodeCompileUnit codeUnit)
        {
            if (!AddOptionSetMetadataAttribute)
            {
                return;
            }

            var metadataByName = ServiceCache.MetadataForEnumsByName;
            foreach (var type in codeUnit.GetTypes())
            {
                if (metadataByName.TryGetValue(type.Name, out var osMetadata))
                {
                    AddMetadataAttributesForSet(type, osMetadata);
                }
                else
                {
                    Trace.TraceInformation("Unable to find metadata for {0}", type.Name);
                }
            }
        }

        public void AddMetadataAttributesForSet(CodeTypeDeclaration type, OptionSetMetadataBase osMetadata)
        {
            if (!AddOptionSetMetadataAttribute)
            {
                return;
            }

            var options = osMetadata.GetOptions();
            var metadataByValue = options.ToDictionary(k => k.Value);
            var orderIndexByValue = new Dictionary<int, int>();
            for (var i = 0; i < options.Count; i++)
            {
                if (options[i].Value is int intValue)
                {
                    orderIndexByValue.Add(intValue, i);
                }
                else
                {
                    Trace.TraceInformation("Unable to find orderIndexByValue for {0}", type.Name);
                }
            }

            for (var i = 0; i < type.Members.Count; i++)
            {
                var value = type.Members[i] as CodeMemberField;
                if (value != null 
                    && value.InitExpression is CodePrimitiveExpression primitive 
                    && primitive.Value is int intValue 
                    && metadataByValue.TryGetValue(intValue, out var metadata))
                {
                    var attribute = new CodeAttributeDeclaration("OptionSetMetadataAttribute", 
                        new CodeAttributeArgument(new CodePrimitiveExpression(metadata.Label.GetLocalOrDefaultText())),
                        new CodeAttributeArgument(new CodePrimitiveExpression(orderIndexByValue[intValue]))
                    );
                    var optionalArs = new Stack<string>(new []
                    {
                        metadata.Color,
                        metadata.Description.GetLocalOrDefaultText(),
                        metadata.ExternalValue
                    });

                    while (optionalArs.Count > 0 && string.IsNullOrWhiteSpace(optionalArs.Peek()))
                    {
                        optionalArs.Pop();
                    }

                    if (optionalArs.Count > 0)
                    {
                        attribute.Arguments.AddRange(
                            optionalArs.Select(v => new CodeAttributeArgument(new CodePrimitiveExpression(v)))
                                       .Reverse().ToArray()
                        );
                    }

                    value.CustomAttributes.Add(attribute);
                }
                else
                {
                    Trace.TraceInformation("Unable to determine OptionSetMetadataAttribute for {0}", value?.Name);
                }
            }
        }

        private static void SortOptionSets(CodeCompileUnit codeUnit)
        {
            // Attempt to order by name by copying to temp collection, removing all from real collection, then adding back ordered by the name
            var temp = new CodeNamespace[codeUnit.Namespaces.Count];
            codeUnit.Namespaces.CopyTo(temp, 0);

            for (var i = 0; i < codeUnit.Namespaces.Count; i++)
            {
                codeUnit.Namespaces.RemoveAt(i);
            }
            codeUnit.Namespaces.AddRange(temp.OrderBy(n => n.Name).ToArray());

            for (var i = codeUnit.Namespaces.Count - 1; i >= 0; i--)
            {
                var nameSpace = codeUnit.Namespaces[i];
                var optionSets = new List<CodeTypeDeclaration>();
                var indexQueue = new Queue<int>();

                for (var j = nameSpace.Types.Count - 1; j >= 0; j--)
                {
                    var type = nameSpace.Types[j];
                    if (!type.IsEnum)
                    {
                        continue;
                    }

                    optionSets.Add(type);
                    indexQueue.Enqueue(j);

                    var tmpMember = new CodeTypeMember[type.Members.Count];
                    type.Members.CopyTo(tmpMember, 0);

                    for (var k = type.Members.Count - 1; k >= 0; k--)
                    {
                        type.Members.RemoveAt(k);
                    }
                    type.Members.AddRange(tmpMember.OrderBy(m => m.Name).ToArray());
                }


                foreach (var optionSet in optionSets.OrderBy(s => s.Name))
                {
                    nameSpace.Types[indexQueue.Dequeue()] = optionSet;
                }
            }
        }
    }
}