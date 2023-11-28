using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk.Metadata;
using DLaB.Xrm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DLaB.ModelBuilderExtensions
{
    public class ServiceCache
    {
        private Dictionary<string, EntityMetadata> _entities;
        private static ServiceCache _default;

        public Dictionary<string, EntityMetadata> EntityMetadataByLogicalName
        {
            get
            {
                return _entities ?? (_entities = ServiceProvider.GetService<IMetadataProviderService>().LoadMetadata(ServiceProvider).Entities.ToDictionary(e => e.LogicalName));
            }
            set => _entities = value;
        }

        private Dictionary<string, List<MultiSelectPicklistAttributeMetadata>> _localMultiSelectOptionSets;
        public Dictionary<string, List<MultiSelectPicklistAttributeMetadata>> LocalMultiSelectOptionSetAttributesByEntityLogicalName
        {
            get
            {
                if (_localMultiSelectOptionSets != null)
                {
                    return _localMultiSelectOptionSets;
                }

                _localMultiSelectOptionSets = new Dictionary<string, List<MultiSelectPicklistAttributeMetadata>>();
                var metadata = ServiceProvider.GetService<IMetadataProviderService>().LoadMetadata(ServiceProvider);

                foreach (var entity in metadata.Entities)
                {
                    var attributes = new List<MultiSelectPicklistAttributeMetadata>();
                    foreach (var attribute in entity.Attributes.Where(a => a.AttributeType == AttributeTypeCode.Virtual
                                                                           && a is MultiSelectPicklistAttributeMetadata enumMeta
                                                                           && enumMeta.OptionSet.IsGlobal == false))
                    {
                        attributes.Add((MultiSelectPicklistAttributeMetadata)attribute);
                    }

                    if (attributes.Any())
                    {
                        _localMultiSelectOptionSets[entity.LogicalName] = attributes;
                    }
                }

                return _localMultiSelectOptionSets;
            }

            set => _localMultiSelectOptionSets = value;
        }

        private Dictionary<string, OptionSetMetadataBase> _metadataForEnumsByName;
        public Dictionary<string, OptionSetMetadataBase> MetadataForEnumsByName
        {
            get
            {
                if (_metadataForEnumsByName != null)
                {
                    return _metadataForEnumsByName;
                }

                _metadataForEnumsByName = new Dictionary<string, OptionSetMetadataBase>();
                var metadata = ((IMetadataProviderService)ServiceProvider.GetService(typeof(IMetadataProviderService))).LoadMetadata(ServiceProvider);
                var filterService = ((ICodeWriterFilterService)ServiceProvider.GetService(typeof(ICodeWriterFilterService)));
                var namingService = (INamingService)ServiceProvider.GetService(typeof(INamingService));
                foreach (var entity in metadata.Entities)
                {
                    foreach (var attribute in entity.Attributes.OfType<EnumAttributeMetadata>()
                                 .Where(a => a.OptionSet != null
                                             && filterService.GenerateOptionSet(a.OptionSet, ServiceProvider)))
                    {
                        var name = namingService.GetNameForOptionSet(entity, attribute.OptionSet, ServiceProvider);
                        if (!_metadataForEnumsByName.ContainsKey(name))
                        {
                            _metadataForEnumsByName[name] = attribute.OptionSet;
                        }
                    }
                }

                foreach (var optionSet in metadata.OptionSets)
                {
                    var name = namingService.GetNameForOptionSet(null, optionSet, ServiceProvider);
                    if (!_metadataForEnumsByName.ContainsKey(name))
                    {
                        _metadataForEnumsByName[name] = optionSet;
                    }
                }


                return _metadataForEnumsByName;
            }

            set => _metadataForEnumsByName = value;
        }

        private Dictionary<string, Tuple<string, OptionSetMetadata>> _metadataForLocalEnumsByName;
        public Dictionary<string, Tuple<string, OptionSetMetadata>> MetadataForLocalEnumsByName
        {
            get
            {
                if (_metadataForLocalEnumsByName != null)
                {
                    return _metadataForLocalEnumsByName;
                }

                _metadataForLocalEnumsByName = new Dictionary<string, Tuple<string, OptionSetMetadata>>();
                var metadata = ((IMetadataProviderService)ServiceProvider.GetService(typeof(IMetadataProviderService))).LoadMetadata(ServiceProvider);
                var filterService = ((ICodeWriterFilterService)ServiceProvider.GetService(typeof(ICodeWriterFilterService)));
                var namingService = (INamingService)ServiceProvider.GetService(typeof(INamingService));
                foreach (var entity in metadata.Entities)
                {
                    foreach (var attribute in entity.Attributes.OfType<EnumAttributeMetadata>()
                                 .Where(a => a.OptionSet != null
                                             && filterService.GenerateOptionSet(a.OptionSet, ServiceProvider)))
                    {
                        var name = namingService.GetNameForOptionSet(entity, attribute.OptionSet, ServiceProvider);
                        if (!_metadataForLocalEnumsByName.ContainsKey(name))
                        {
                            _metadataForLocalEnumsByName[name] = new Tuple<string, OptionSetMetadata>(namingService.GetNameForEntity(entity, ServiceProvider), attribute.OptionSet);
                        }
                    }
                }


                return _metadataForLocalEnumsByName;
            }

            set => _metadataForLocalEnumsByName = value;
        }

        public IServiceProvider ServiceProvider { get; }

        public ServiceCache(IServiceProvider services)
        {
            ServiceProvider = services;
        }

        public static ServiceCache GetDefault(IServiceProvider services)
        {
            return _default ?? (_default = new ServiceCache(services));
        }

        public static void ClearCache()
        {
            _default = null;
        }
    }
}
