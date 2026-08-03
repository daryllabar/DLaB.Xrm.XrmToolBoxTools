using FakeItEasy;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;

namespace DLaB.ModelBuilderExtensions.Tests
{
    [TestClass]
    public class ObsoleteAttributesProviderServiceTests
    {
        [TestMethod]
        public void GetObsoleteAttributes_WhenObsoleteDeprecatedIsFalse_ShouldReturnEmptySet()
        {
            var sut = BuildSut(obsoleteDeprecated: false);

            var result = sut.GetObsoleteAttributes(A.Fake<IServiceProvider>());

            Assert.AreEqual(0, result.Count, "Should return an empty set when ObsoleteDeprecated is false.");
        }

        [TestMethod]
        public void GetObsoleteAttributes_WhenTokenMatchesDisplayName_ShouldReturnMatchingAttribute()
        {
            var entity = BuildEntity("account",
                ("name", "Full Name (Deprecated)"),
                ("emailaddress1", "Email"));

            var serviceProvider = BuildServiceProvider(BuildMetadata(entity));
            var sut = BuildSut(obsoleteDeprecated: true, obsoleteTokens: new List<string> { "*Deprecated*" });

            var result = sut.GetObsoleteAttributes(serviceProvider);

            Assert.IsTrue(result.Contains("account.name"),
                "Attribute whose display name matches the obsolete token should be included.");
            Assert.IsFalse(result.Contains("account.emailaddress1"),
                "Attribute whose display name does not match the obsolete token should be excluded.");
        }

        [TestMethod]
        public void GetObsoleteAttributes_WhenNoTokensMatch_ShouldReturnEmptySet()
        {
            var entity = BuildEntity("account",
                ("name", "Full Name"),
                ("emailaddress1", "Email"));

            var serviceProvider = BuildServiceProvider(BuildMetadata(entity));
            var sut = BuildSut(obsoleteDeprecated: true, obsoleteTokens: new List<string> { "*Deprecated*" });

            var result = sut.GetObsoleteAttributes(serviceProvider);

            Assert.AreEqual(0, result.Count, "No attributes should match when no display name contains the token.");
        }

        [TestMethod]
        public void GetObsoleteAttributes_WhenCalledTwice_ShouldReturnCachedResult()
        {
            var metadataProvider = A.Fake<IMetadataProviderService>();
            A.CallTo(() => metadataProvider.LoadMetadata(A<IServiceProvider>._))
                .Returns(BuildMetadata(BuildEntity("account", ("name", "Full Name (Deprecated)"))));

            var serviceProvider = A.Fake<IServiceProvider>();
            A.CallTo(() => serviceProvider.GetService(typeof(IMetadataProviderService))).Returns(metadataProvider);

            var sut = BuildSut(obsoleteDeprecated: true, obsoleteTokens: new List<string> { "*Deprecated*" });

            var first = sut.GetObsoleteAttributes(serviceProvider);
            var second = sut.GetObsoleteAttributes(serviceProvider);

            Assert.AreSame(first, second, "Second call should return the cached HashSet instance.");
            A.CallTo(() => metadataProvider.LoadMetadata(A<IServiceProvider>._))
                .MustHaveHappenedOnceExactly();
        }

        [TestMethod]
        public void GetObsoleteAttributes_WithMultipleEntities_ShouldIncludeAttributesFromAllMatchingEntities()
        {
            var account = BuildEntity("account", ("name", "Full Name (Deprecated)"), ("emailaddress1", "Email"));
            var contact = BuildEntity("contact", ("jobtitle", "Job Title (Deprecated)"), ("firstname", "First Name"));

            var serviceProvider = BuildServiceProvider(BuildMetadata(account, contact));
            var sut = BuildSut(obsoleteDeprecated: true, obsoleteTokens: new List<string> { "*Deprecated*" });

            var result = sut.GetObsoleteAttributes(serviceProvider);

            Assert.IsTrue(result.Contains("account.name"), "account.name should be obsolete.");
            Assert.IsFalse(result.Contains("account.emailaddress1"), "account.emailaddress1 should not be obsolete.");
            Assert.IsTrue(result.Contains("contact.jobtitle"), "contact.jobtitle should be obsolete.");
            Assert.IsFalse(result.Contains("contact.firstname"), "contact.firstname should not be obsolete.");
        }

        private static IServiceProvider BuildServiceProvider(IOrganizationMetadata metadata)
        {
            var metadataProvider = A.Fake<IMetadataProviderService>();
            A.CallTo(() => metadataProvider.LoadMetadata(A<IServiceProvider>._)).Returns(metadata);

            var serviceProvider = A.Fake<IServiceProvider>();
            A.CallTo(() => serviceProvider.GetService(typeof(IMetadataProviderService))).Returns(metadataProvider);

            return serviceProvider;
        }

        private static IOrganizationMetadata BuildMetadata(params EntityMetadata[] entities)
        {
            var metadata = A.Fake<IOrganizationMetadata>();
            A.CallTo(() => metadata.Entities).Returns(entities);
            return metadata;
        }

        [TestMethod]
        public void GetObsoleteAttributes_WhenGermanUserLocalizedAndGermanTokens_MatchesUserLocalizedLabels()
        {
            // Simulate a German user: attribute has German UserLocalizedLabel, with English also available in LocalizedLabels
            var entity = BuildEntityWithLanguages("account",
                ("name", "Vollst\u00e4ndiger Name (Veraltet)", 1031, "Full Name (Deprecated)", 1033),
                ("emailaddress1", "E-Mail", 1031, "Email", 1033));

            var serviceProvider = BuildServiceProvider(BuildMetadata(entity));
            // No language override: should evaluate UserLocalizedLabel (German)
            var sut = BuildSut(obsoleteDeprecated: true, obsoleteTokens: new List<string> { "*Veraltet*" });

            var result = sut.GetObsoleteAttributes(serviceProvider);

            Assert.IsTrue(result.Contains("account.name"), "Should match German UserLocalizedLabel when no language override is configured.");
            Assert.IsFalse(result.Contains("account.emailaddress1"), "Attribute without the obsolete token should be excluded.");
        }

        private static EntityMetadata BuildEntityWithLanguages(string logicalName,
            params (string logicalName, string userLabel, int userLangCode, string localizedLabel, int localizedLangCode)[] attributes)
        {
            var entity = new EntityMetadata { LogicalName = logicalName };
            var attributeList = new List<AttributeMetadata>();
            foreach (var (attrLogicalName, userLabel, userLangCode, localizedLabel, localizedLangCode) in attributes)
            {
                var attr = new StringAttributeMetadata();
                typeof(AttributeMetadata)
                    .GetProperty(nameof(AttributeMetadata.LogicalName))!
                    .SetValue(attr, attrLogicalName);
                typeof(AttributeMetadata)
                    .GetProperty(nameof(AttributeMetadata.DisplayName))!
                    .SetValue(attr, new Label(
                        new LocalizedLabel(userLabel, userLangCode),
                        new[] { new LocalizedLabel(localizedLabel, localizedLangCode) }));
                attributeList.Add(attr);
            }
            typeof(EntityMetadata)
                .GetProperty(nameof(EntityMetadata.Attributes))!
                .SetValue(entity, attributeList.ToArray());
            return entity;
        }

        private static EntityMetadata BuildEntity(string logicalName, params (string logicalName, string displayName)[] attributes)
        {
            var entity = new EntityMetadata { LogicalName = logicalName };
            var attributeList = new List<AttributeMetadata>();
            foreach (var (attrLogicalName, displayName) in attributes)
            {
                // Use StringAttributeMetadata as a concrete type instead of trying to mock AttributeMetadata
                var attr = new StringAttributeMetadata();
                // Set LogicalName via reflection since it doesn't have a public setter
                typeof(AttributeMetadata)
                    .GetProperty(nameof(AttributeMetadata.LogicalName))!
                    .SetValue(attr, attrLogicalName);
                // Set DisplayName via reflection since it doesn't have a public setter
                typeof(AttributeMetadata)
                    .GetProperty(nameof(AttributeMetadata.DisplayName))!
                    .SetValue(attr, new Label(new LocalizedLabel(displayName, 1033), Array.Empty<LocalizedLabel>()));
                attributeList.Add(attr);
            }
            // Inject attributes via reflection since the Attributes setter is internal
            typeof(EntityMetadata)
                .GetProperty(nameof(EntityMetadata.Attributes))!
                .SetValue(entity, attributeList.ToArray());
            return entity;
        }

        private static ObsoleteAttributesProviderService BuildSut(bool obsoleteDeprecated, List<string> obsoleteTokens = null)
        {
            var sut = new ObsoleteAttributesProviderService();
            sut.ObsoleteDeprecated = obsoleteDeprecated;
            sut.ObsoleteTokens = obsoleteTokens ?? new List<string>();
            return sut;
        }
    }
}
