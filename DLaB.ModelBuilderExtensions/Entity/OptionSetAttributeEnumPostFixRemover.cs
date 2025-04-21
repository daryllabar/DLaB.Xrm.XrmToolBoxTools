using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.ModelBuilderExtensions.Entity
{
    public class OptionSetAttributeEnumPostFixRemover : TypedServiceBase<ICustomizeCodeDomService>
    {

        #region Constructors

        public OptionSetAttributeEnumPostFixRemover(ICustomizeCodeDomService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
        }

        public OptionSetAttributeEnumPostFixRemover(ICustomizeCodeDomService defaultService, DLaBModelBuilderSettings settings = null) : base(defaultService, settings)
        {
        }

        #endregion Constructors

        #region ICustomizeCodeDomService Members

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            SetServiceCache(services);

            foreach (var type in codeUnit.GetEntityTypes())
            {
                var entityMetadata = ServiceCache.EntityMetadataByLogicalName[type.GetEntityLogicalName()];
                foreach (var member in type.Members)
                {
                    RemoveIncorrectlyPostfixedEnumFromPropertyType(services, member as CodeTypeMember, entityMetadata);
                }
            }
        }

        public void RemoveIncorrectlyPostfixedEnumFromPropertyType(IServiceProvider services, CodeTypeMember member, EntityMetadata entityMetadata)
        {
            if (!(member is CodeMemberProperty property))
            {
                return;
            }

            var typeName = property.Type.BaseType.Replace("?", string.Empty);
            if (!typeName.EndsWith("Enum"))
            {
                return;
            }
            

            var attributeMetadata = entityMetadata.Attributes.First(a => a.LogicalName == property.GetLogicalName());
            if (!attributeMetadata.IsOptionSet()
                || ServiceCache.MetadataForEnumsByName.ContainsKey(typeName))
            {
                return;
            }
                    
            // Model Builder has post fixed Enum (or Enum?) to the end of the type but the option set type has not been generated.  Remove the postfix.
            var typeWithoutPostfix = typeName.Substring(0, typeName.Length - "Enum".Length);
            if (ServiceCache.MetadataForEnumsByName.ContainsKey(typeWithoutPostfix))
            {
                // Expected name of Type exists, use it
                property.Type.BaseType = typeWithoutPostfix;
                return;
            }

            // Might not be possible, but lookup the name of the type as defined by the naming service if the expected type name doesn't exist.
            var namingService = services.Get<INamingService>();
            property.Type.BaseType = namingService.GetNameForOptionSet(entityMetadata, ((PicklistAttributeMetadata)attributeMetadata).OptionSet, services);
        }

        #endregion
   }
}
