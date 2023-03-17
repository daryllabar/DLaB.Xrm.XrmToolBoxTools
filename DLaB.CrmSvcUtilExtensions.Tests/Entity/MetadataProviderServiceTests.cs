using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.ModelBuilderExtensions.Tests.Entity
{
    [TestClass]
    public class MetadataProviderServiceTests
    {
        [TestMethod]
        public void CanDeserializeMetadata()
        {
            var factory = new ServiceFactory();
            var provider = factory.GetService<IMetadataProviderService>();
            Assert.IsNotNull(provider);
            var metadata = provider.LoadMetadata(factory);
            Assert.AreNotEqual(0, metadata.Entities.Length);
        }
    }
}
