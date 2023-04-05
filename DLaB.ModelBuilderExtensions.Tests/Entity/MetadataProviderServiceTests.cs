using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
