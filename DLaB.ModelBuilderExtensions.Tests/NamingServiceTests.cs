using FakeItEasy;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace DLaB.ModelBuilderExtensions.Tests
{
    [TestClass]
    public class NamingServiceTests
    {
        [TestMethod]
        [DataRow("acme_Something", "Test", DisplayName = "Name First In Settings Should Map")]
        [DataRow("acme_SomethingElse", "Test2", DisplayName = "Name Last In Settings Should Map")]
        [DataRow("acme_Never", "acme_Never", DisplayName = "Name Not In Settings Should Not Map")]
        public void GetNameForEntity_Tests(string schemaName, string expectedName)
        {
            var fakeNamingService = A.Fake<INamingService>();
            A.CallTo(() => fakeNamingService.GetNameForEntity(A<EntityMetadata>._, A<IServiceProvider>._)).Returns(schemaName);
            var sut = new NamingService(fakeNamingService,  new DLaBModelBuilderSettings
            {
                DLaBModelBuilder = new DLaBModelBuilder
                {
                    EntityClassNameOverrides = new Dictionary<string, string>
                    {
                        {"acme_something", "Test"},
                        {"acme_somethingelse", "Test2"}
                    }
                }
            });

            Assert.AreEqual(expectedName, sut.GetNameForEntity(new EntityMetadata { LogicalName = schemaName.ToLower()}, A.Fake<IServiceProvider>()));
        }

        [TestMethod]
        [DataRow("10Th", "_10th", DisplayName = "10Th is 10th")]
        [DataRow("1St", "_1st", DisplayName = "1St is 1st")]
        [DataRow("2Nd", "_2nd", DisplayName = "2Nd is 2nd")]
        [DataRow("3Rd", "_3rd", DisplayName = "3Rd is 3rd")]
        [DataRow("4Th", "_4th", DisplayName = "4Th is 4th")]
        [DataRow("5Th", "_5th", DisplayName = "5Th is 5th")]
        [DataRow("6Th", "_6th", DisplayName = "6Th is 6th")]
        [DataRow("7Th", "_7th", DisplayName = "7Th is 7th")]
        [DataRow("8Th", "_8th", DisplayName = "8Th is 8th")]
        [DataRow("9Th", "_9th", DisplayName = "9Th is 9th")]
        public void GetNameForOption_Tests(string schemaName, string expected)
        {
            var fakeNamingService = A.Fake<INamingService>();
            A.CallTo(() => fakeNamingService.GetNameForEntity(A<EntityMetadata>._, A<IServiceProvider>._)).Returns(expected ?? schemaName);

            var optionSetMetadata = new OptionSetMetadata();

            var optionMetadata = new OptionMetadata
            {
                Label = new Label
                {
                    UserLocalizedLabel = new LocalizedLabel
                    {
                        Label = schemaName
                    }
                },
            };

            var sut = new NamingService(fakeNamingService, new DLaBModelBuilderSettings
            {
                DLaBModelBuilder = new DLaBModelBuilder
                {
                    AdjustCasingForEnumOptions = true,
                    OptionNameOverrides = new Dictionary<string, string>
                    {
                        { "1st", "1st" },
                        { "2nd", "2nd" },
                        { "3rd", "3rd" },
                        { "4th", "4th" },
                        { "5th", "5th" },
                        { "6th", "6th" },
                        { "7th", "7th" },
                        { "8th", "8th" },
                        { "9th", "9th" },
                        { "0th", "0th" }
                    }
                }
            });

            A.CallTo(() => fakeNamingService.GetNameForOption(A<OptionSetMetadataBase>._, A<OptionMetadata>._, A<IServiceProvider>._)).Returns(schemaName);
            var name = sut.GetNameForOption(optionSetMetadata, optionMetadata, A.Fake<IServiceProvider>());
            Assert.AreEqual(expected ?? schemaName.ToLower(), name);
        }

        [TestMethod]
        //[DataRow("Acme_Something", "Acme_SomethingElse", DisplayName = "Entity name in State and Status Code names are replaced")]
        [DataRow("Acme_Something", null, DisplayName = "Entity name in State and Status Code names are replaced")]
        public void GetNameForOptionSet_Tests(string schemaName, string overrideName)
        {
            var fakeNamingService = A.Fake<INamingService>();
            A.CallTo(() => fakeNamingService.GetNameForEntity(A<EntityMetadata>._, A<IServiceProvider>._)).Returns(overrideName ?? schemaName);
            
            var serviceCache = ServiceCache.GetDefault(A.Fake<IServiceProvider>());
            var entityMetadata = new EntityMetadata {
                LogicalName = schemaName.ToLower()
            };

            typeof(EntityMetadata).GetProperty(nameof(EntityMetadata.Attributes))?.SetValue(entityMetadata, new AttributeMetadata[] { });

            serviceCache.EntityMetadataByLogicalName = new Dictionary<string, EntityMetadata>
            {
                { schemaName.ToLower(), entityMetadata },
            };

            var sut = new NamingService(fakeNamingService, new DLaBModelBuilderSettings
            {
                DLaBModelBuilder = new DLaBModelBuilder
                {
                    EntityClassNameOverrides = overrideName == null 
                        ? new Dictionary<string, string>()
                        : new Dictionary<string, string>
                        {
                            { schemaName.ToLower(), overrideName},
                        }
                }
            });

            A.CallTo(() => fakeNamingService.GetNameForOptionSet(A<EntityMetadata>._, A<OptionSetMetadataBase>._, A<IServiceProvider>._)).Returns(schemaName.ToLower() + "State");
            Assert.AreEqual((overrideName ?? schemaName.ToLower()) + "State", sut.GetNameForOptionSet(entityMetadata, new OptionSetMetadata
            {
                Name = schemaName.ToLower() + "_statecode"
            }, A.Fake<IServiceProvider>()));
            A.CallTo(() => fakeNamingService.GetNameForOptionSet(A<EntityMetadata>._, A<OptionSetMetadataBase>._, A<IServiceProvider>._)).Returns(schemaName.ToLower() + "_statuscode");
            Assert.AreEqual((overrideName ?? schemaName) + "_StatusCode", sut.GetNameForOptionSet(entityMetadata, new OptionSetMetadata
            {
                OptionSetType = OptionSetType.Status,
                IsGlobal = false,
                Name = schemaName.ToLower() + "_statuscode"
            }, A.Fake<IServiceProvider>()));
        }

        [TestMethod]
        [DataRow("acme_Something", "acme_Something", "acme_Something__Member", DisplayName = "Matching Names, should append postfix")]
        [DataRow("acme_Something", "acme_SomethingElse", "acme_SomethingElse", DisplayName = "Unique Names, should not append postfix")]
        public void GetNameForAttribute_Tests(string entitySchemaName, string attributeSchemaName, string expectedName)
        {
            var fakeNamingService = A.Fake<INamingService>();
            A.CallTo(() => fakeNamingService.GetNameForEntity(A<EntityMetadata>._, A<IServiceProvider>._)).Returns(entitySchemaName);
            A.CallTo(() => fakeNamingService.GetNameForAttribute(A<EntityMetadata>._, A<AttributeMetadata>._, A<IServiceProvider>._)).Returns(attributeSchemaName);
            var sut = new NamingService(fakeNamingService, new DLaBModelBuilderSettings
            {
                DLaBModelBuilder = new DLaBModelBuilder()
            });

            Assert.AreEqual(expectedName, sut.GetNameForAttribute(
                new EntityMetadata { LogicalName = entitySchemaName.ToLower() },
                new AttributeMetadata { LogicalName = attributeSchemaName.ToLower()},
                A.Fake<IServiceProvider>()));
        }

        [TestMethod]
        public void GetNameFromLabel_Test()
        {
            var sut = new NamingService(null, new DLaBModelBuilderSettings());

            Assert.AreEqual("NoShow", sut.GetNameFromLabel("no show"));
            Assert.AreEqual("NoShow", sut.GetNameFromLabel("no-show"));
            Assert.AreEqual("NoShow", sut.GetNameFromLabel("no--show"));
            Assert.AreEqual("NoShow", sut.GetNameFromLabel("no!$#show"));
            Assert.AreEqual("NoShow", sut.GetNameFromLabel("nO sHOW"));
            Assert.AreEqual("NoShow", sut.GetNameFromLabel("NO SHOW"));
            Assert.AreEqual("NoShows", sut.GetNameFromLabel("NO SHOW's"));
            Assert.AreEqual("NosShow", sut.GetNameFromLabel("NO's Show"));
        }
    }
}
