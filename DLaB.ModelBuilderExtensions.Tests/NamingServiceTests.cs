using FakeItEasy;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;

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
        //[DataRow("Acme_Something", "Acme_SomethingElse", DisplayName = "Entity name in State and Status Code names are replaced")]
        [DataRow("Acme_Something", null, DisplayName = "Entity name in State and Status Code names are replaced")]
        public void GetNameForOptionSet_Tests(string schemaName, string overrideName)
        {
            ;
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
        public void GetNameFromLabel_Test()
        {
            var sut = new NamingService(null, new DLaBModelBuilderSettings());

            Assert.AreEqual("NoShow", sut.GetNameFromLabel("no show"));
            Assert.AreEqual("NoShow", sut.GetNameFromLabel("no-show"));
            Assert.AreEqual("NoShow", sut.GetNameFromLabel("no--show"));
            Assert.AreEqual("NoShow", sut.GetNameFromLabel("no!$#show"));
            Assert.AreEqual("NoShow", sut.GetNameFromLabel("nO sHOW"));
            Assert.AreEqual("NoShow", sut.GetNameFromLabel("NO SHOW"));
        }
    }
}
