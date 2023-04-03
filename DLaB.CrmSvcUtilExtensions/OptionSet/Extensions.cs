using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.CrmSvcUtilExtensions.OptionSet
{
    public static class Extensions
    {
        public static Dictionary<string, OptionSetMetadataBase> GetMetadataForEnumsByName(this IServiceProvider services)
        {
            var metadataByEnumName = new Dictionary<string, OptionSetMetadataBase>();
            var metadata = ((IMetadataProviderService)services.GetService(typeof(IMetadataProviderService))).LoadMetadata();
            var filterService = ((ICodeWriterFilterService)services.GetService(typeof(ICodeWriterFilterService)));
            var namingService = (INamingService)services.GetService(typeof(INamingService));
            foreach (var entity in metadata.Entities)
            {
                foreach (var attribute in entity.Attributes.OfType<EnumAttributeMetadata>()
                                                .Where(a => a.OptionSet != null
                                                            && filterService.GenerateOptionSet(a.OptionSet, services)))
                {
                    var name = namingService.GetNameForOptionSet(entity, attribute.OptionSet, services);
                    if (!metadataByEnumName.ContainsKey(name))
                    {
                        metadataByEnumName[name] = attribute.OptionSet;
                    }
                }
            }

            foreach (var optionSet in metadata.OptionSets)
            {
                var name = namingService.GetNameForOptionSet(null, optionSet, services);
                if (!metadataByEnumName.ContainsKey(name))
                {
                    metadataByEnumName[name] = optionSet;
                }
            }

            return metadataByEnumName;
        }

        public static Dictionary<string, Tuple<string, OptionSetMetadata>> GetMetadataForLocalEnumsByName(this IServiceProvider services)
        {
            var metadataByEnumName = new Dictionary<string, Tuple<string, OptionSetMetadata>>();
            var metadata = ((IMetadataProviderService)services.GetService(typeof(IMetadataProviderService))).LoadMetadata();
            var filterService = ((ICodeWriterFilterService)services.GetService(typeof(ICodeWriterFilterService)));
            var namingService = (INamingService)services.GetService(typeof(INamingService));
            foreach (var entity in metadata.Entities)
            {
                foreach (var attribute in entity.Attributes.OfType<EnumAttributeMetadata>()
                                                .Where(a => a.OptionSet != null
                                                            && filterService.GenerateOptionSet(a.OptionSet, services)))
                {
                    var name = namingService.GetNameForOptionSet(entity, attribute.OptionSet, services);
                    if (!metadataByEnumName.ContainsKey(name))
                    {
                        metadataByEnumName[name] = new Tuple<string, OptionSetMetadata>(namingService.GetNameForEntity(entity, services), attribute.OptionSet);
                    }
                }
            }

            return metadataByEnumName;
        }
    }
}
