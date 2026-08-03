using FakeItEasy;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DLaB.ModelBuilderExtensions.Tests.Entity
{
    [TestClass]
    public class MetadataProviderServiceTests
    {
        [TestMethod]
        public void CanDeserializeMetadata()
        {
            var provider = new Metadata.Provider("2016.xml");
            var metadata = provider.LoadMetadata(null);
            Assert.AreNotEqual(0, metadata.Entities.Length);
        }

        [TestMethod]
        public void LoadMetadata_WhenObsoleteDeprecatedIsTrue_ShouldSetDeprecatedVersionForMatchingAttributes()
        {
            var entity = BuildEntity("account",
                ("name", "Full Name (Deprecated)", null),
                ("emailaddress1", "Email", null));
            var metadata = BuildMetadata(entity);

            var defaultService = A.Fake<IMetadataProviderService>();
            A.CallTo(() => defaultService.LoadMetadata(A<IServiceProvider>._)).Returns(metadata);

            var sut = new MetadataProviderService(defaultService, new DLaBModelBuilderSettings
            {
                DLaBModelBuilder = new DLaBModelBuilder
                {
                    ObsoleteDeprecated = true,
                    ObsoleteTokens = new List<string> { "*Deprecated*" }
                }
            });

            sut.LoadMetadata(A.Fake<IServiceProvider>());

            Assert.AreEqual(string.Empty, entity.Attributes.Single(a => a.LogicalName == "name").DeprecatedVersion);
            Assert.IsNull(entity.Attributes.Single(a => a.LogicalName == "emailaddress1").DeprecatedVersion);
        }

        [TestMethod]
        public void LoadMetadata_WhenObsoleteDeprecatedIsFalse_ShouldLeaveDeprecatedVersionUnset()
        {
            var entity = BuildEntity("account",
                ("name", "Full Name (Deprecated)", null));
            var metadata = BuildMetadata(entity);

            var defaultService = A.Fake<IMetadataProviderService>();
            A.CallTo(() => defaultService.LoadMetadata(A<IServiceProvider>._)).Returns(metadata);

            var sut = new MetadataProviderService(defaultService, new DLaBModelBuilderSettings
            {
                DLaBModelBuilder = new DLaBModelBuilder
                {
                    ObsoleteDeprecated = false,
                    ObsoleteTokens = new List<string> { "*Deprecated*" }
                }
            });

            sut.LoadMetadata(A.Fake<IServiceProvider>());

            Assert.IsNull(entity.Attributes.Single().DeprecatedVersion);
        }

        private static IOrganizationMetadata BuildMetadata(params EntityMetadata[] entities)
        {
            var metadata = A.Fake<IOrganizationMetadata>();
            A.CallTo(() => metadata.Entities).Returns(entities);
            return metadata;
        }

        private static EntityMetadata BuildEntity(string logicalName, params (string logicalName, string displayName, string deprecatedVersion)[] attributes)
        {
            var entity = new EntityMetadata { LogicalName = logicalName };
            var attributeList = new List<AttributeMetadata>();
            foreach (var (attributeLogicalName, displayName, deprecatedVersion) in attributes)
            {
                var attribute = new StringAttributeMetadata();
                typeof(AttributeMetadata)
                    .GetProperty(nameof(AttributeMetadata.LogicalName))!
                    .SetValue(attribute, attributeLogicalName);
                typeof(AttributeMetadata)
                    .GetProperty(nameof(AttributeMetadata.DisplayName))!
                    .SetValue(attribute, new Label(new LocalizedLabel(displayName, 1033), Array.Empty<LocalizedLabel>()));

                if (deprecatedVersion != null)
                {
                    typeof(AttributeMetadata)
                        .GetProperty(nameof(AttributeMetadata.DeprecatedVersion), BindingFlags.Public | BindingFlags.Instance)!
                        .GetSetMethod(true)!
                        .Invoke(attribute, [deprecatedVersion]);
                }

                attributeList.Add(attribute);
            }

            typeof(EntityMetadata)
                .GetProperty(nameof(EntityMetadata.Attributes))!
                .SetValue(entity, attributeList.ToArray());
            return entity;
        }
    }
}
