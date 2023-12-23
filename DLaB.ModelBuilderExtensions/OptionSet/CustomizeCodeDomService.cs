using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DLaB.ModelBuilderExtensions.OptionSet
{
    public class CustomizeCodeDomService : TypedServiceBase<ICustomizeCodeDomService>, ICustomizeCodeDomService
    {
        public bool AddOptionSetMetadataAttribute { get => DLaBSettings.AddOptionSetMetadataAttribute; set => DLaBSettings.AddOptionSetMetadataAttribute = value; }
        public bool EmitEntityETC { get => Settings.EmitEntityETC; set => Settings.EmitEntityETC = value; }
        public bool GenerateAllOptionSetLabelMetadata { get => DLaBSettings.GenerateAllOptionSetLabelMetadata; set => DLaBSettings.GenerateAllOptionSetLabelMetadata = value; }

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

            if (!EmitEntityETC)
            {
                // Remove Connection record1objecttypecode Enums since they are Connection_Record1ObjectTypeCode and Connection_Record2ObjectTypeCode
            }

            AddMetadataAttributes(codeUnit);
            SortOptionSets(codeUnit);
            //Trace.TraceInformation("Exiting ICustomizeCodeDomService.CustomizeCodeDom");
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
                if (value?.InitExpression is CodePrimitiveExpression primitive
                    && primitive.Value is int intValue
                    && metadataByValue.TryGetValue(intValue, out var metadata))
                {
                    var attribute = new CodeAttributeDeclaration("OptionSetMetadataAttribute", 
                        new CodeAttributeArgument(new CodePrimitiveExpression(metadata.Label.GetLocalOrDefaultText())),
                        new CodeAttributeArgument(new CodePrimitiveExpression(orderIndexByValue[intValue]))
                    );
                    var optionalArs = new Stack<string>(new[]
                    {
                        metadata.Color,
                        metadata.Description.GetLocalOrDefaultText(),
                        metadata.ExternalValue
                    });

                    if(!GenerateAllOptionSetLabelMetadata)
                    {
                        // GenerateAllOptionSetLabelMetadata will require all optional parameters to be populated
                        while (optionalArs.Count > 0 && string.IsNullOrWhiteSpace(optionalArs.Peek()))
                        {
                            optionalArs.Pop();
                        }
                    }
                    
                    if (optionalArs.Count > 0)
                    {
                        attribute.Arguments.AddRange(
                            optionalArs.Select(v => new CodeAttributeArgument(new CodePrimitiveExpression(v)))
                                .Reverse().ToArray()
                        );
                    }

                    if (GenerateAllOptionSetLabelMetadata)
                    {
                        foreach(var label in metadata.Label.LocalizedLabels)
                        {
                            attribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(label.LanguageCode)));
                            attribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(label.Label)));
                        }
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


                foreach (var optionSet in optionSets.OrderByDescending(s => s.Name))
                {
                    nameSpace.Types[indexQueue.Dequeue()] = optionSet;
                }
            }
        }
    }
}