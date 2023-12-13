using System;
using System.Collections.Generic;
using FakeItEasy;
using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Metadata;

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
    }
}
