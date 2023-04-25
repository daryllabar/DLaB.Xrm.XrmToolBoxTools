using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;

namespace DLaB.ModelBuilderExtensions.Entity
{
    public class AttributeConstGenerator : AttributeConstGeneratorBase
    {
        private readonly ServiceCache _serviceCache;

        public override bool GenerateAttributeNameConsts { get => DLaBSettings.GenerateAttributeNameConsts; set => DLaBSettings.GenerateAttributeNameConsts = value; }

        public AttributeConstGenerator(ICustomizeCodeDomService defaultService, IDictionary<string, string> parameters, ServiceCache cache) : base(defaultService, parameters)
        {
            _serviceCache = cache;
        }

        public AttributeConstGenerator(ICustomizeCodeDomService defaultService, ServiceCache cache, DLaBModelBuilderSettings settings = null) : base(defaultService, settings)
        {
            _serviceCache = cache;
        }

        public override void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            foreach (var entity in codeUnit.GetEntityTypes())
            {
                var constsClass = entity.GetMembers<CodeSnippetTypeMember>().FirstOrDefault(s => s.Text.Contains($"public static class {OobConstsClassName}"));
                if (constsClass == null)
                {
                    continue;
                }

                
                if (GenerateAttributeNameConsts)
                {
                    var lines = constsClass.Text.Replace($"public static class {OobConstsClassName}", $"public static partial class {AttributeConstsClassName}")
                        .Replace($"\" +{Environment.NewLine}\t\t\"","")
                        .Split(new [] {
                        Environment.NewLine
                    }, StringSplitOptions.None).ToList();
                    
                    var start = lines.FindIndex(l => l.StartsWith("\t\t\tpublic const string "));
                    var end = lines.FindIndex(l => l.StartsWith("\t\t}"));
                    var atts = lines.Skip(start).Take(end - start).ToList();
                    lines.RemoveRange(start,end-start);

                    var metadata = _serviceCache.EntityMetadataByLogicalName[entity.GetEntityLogicalName()];
                    var createdNames = new HashSet<string>();
                    foreach (var relationship in metadata.ManyToOneRelationships)
                    {
                        if (createdNames.Contains(relationship.ReferencingAttribute))
                        {
                            continue;
                        }
                        createdNames.Add(relationship.ReferencingAttribute);
                        var attLine = atts.FirstOrDefault(a => a.ToLower().Contains(" " + relationship.ReferencingAttribute + " "));
                        if (attLine == null && entity.Name.ToLower() == relationship.ReferencingAttribute)
                        {
                            attLine = atts.FirstOrDefault(a => a.ToLower().Contains(" " + relationship.ReferencingAttribute + "1 "));
                        }

                        if (attLine == null)
                        {
                            throw new Exception($"Error attempting to GenerateAttributeNameConsts!  Unable to find attribute {relationship.ReferencingAttribute} in {entity.Name}");
                        }

                        atts.Add(attLine.Replace(" =", "Name =").Replace("\";", "name\";"));
                    }

                    lines.InsertRange(start, atts.OrderBy(a => a.Split(' ').Last()));
                    constsClass.Text = string.Join(Environment.NewLine, lines);
                }
                else
                {
                    entity.Members.Remove(constsClass);
                }
            }
        }
    }
}