using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk.Metadata;
using Source.DLaB.Xrm;
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

        public IServiceProvider ServiceProvider { get; }

        public ServiceCache(IServiceProvider services)
        {
            ServiceProvider = services;
        }

        public static ServiceCache GetDefault(IServiceProvider services)
        {
            return _default ?? (_default = new ServiceCache(services));
        }

    }
}
