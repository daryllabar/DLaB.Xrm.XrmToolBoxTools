﻿using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.CodeDom;
using System.Linq;

namespace DLaB.ModelBuilderExtensions.Entity
{
    public class MultiOptionSetAttributeUpdater : ICustomizeCodeDomService
    {
        #region Implementation of ICustomizeCodeDomService

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            var attributesByEntity = ((IMetadataProviderService)services.GetService(typeof(IMetadataProviderService)))
                .LoadMetadata(services).Entities
                .ToDictionary(k => k.LogicalName, v => v.Attributes.ToDictionary(k => k.LogicalName));

            foreach (var type in codeUnit.GetEntityTypes())
            {
                var logicalName = type.GetEntityLogicalName();
                if (!attributesByEntity.TryGetValue(logicalName, out var attributes))
                {
                    continue;
                }

                foreach (var member in type.Members)
                {
                    if (!(member is CodeMemberProperty property)
                        || !IsObjectProperty(property) 
                        || !attributes.TryGetValue(property.GetLogicalName(), out var metadata)
                        || !(metadata is MultiSelectPicklistAttributeMetadata))
                    {
                        continue;
                    }
                    
                    // Update Property Type
                    property.Type = new CodeTypeReference(typeof(OptionSetValueCollection));

                    // Update Generic Type Argument For GetAttributeValue
                    var returnStatement = (CodeMethodReturnStatement) property.GetStatements[0];
                    var invoke = (CodeMethodInvokeExpression) returnStatement.Expression;
                    invoke.Method.TypeArguments.Clear();
                    invoke.Method.TypeArguments.Add(property.Type);
                }
            }
        }

        private static bool IsObjectProperty(CodeMemberProperty property)
        {
            // By default this check will work
            return property.Type.BaseType == "System.Object";
        }

        #endregion
    }
}
