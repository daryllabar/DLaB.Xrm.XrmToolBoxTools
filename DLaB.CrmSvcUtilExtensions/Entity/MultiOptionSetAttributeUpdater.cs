using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.CrmSvcUtilExtensions.Entity
{
    public class MultiOptionSetAttributeUpdater : ICustomizeCodeDomService
    {
        #region Implementation of ICustomizeCodeDomService

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            var attributesByEntity = ((IMetadataProviderService)services.GetService(typeof(IMetadataProviderService)))
                .LoadMetadata().Entities
                .ToDictionary(k => k.LogicalName, v => v.Attributes.ToDictionary(k => k.LogicalName));

            var types = codeUnit.Namespaces[0].Types;
            foreach (CodeTypeDeclaration type in types)
            {
                if (!type.IsClass || type.IsContextType() || type.IsBaseEntityType()) { continue; }

                var logicalName = type.GetFieldInitalizedValue("EntityLogicalName");
                Dictionary<string, AttributeMetadata> attributes;
                if (!attributesByEntity.TryGetValue(logicalName, out attributes))
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
