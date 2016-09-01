using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Services.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.CrmSvcUtilExtensions.Tests.Entity
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
            var metadata = provider.LoadMetadata();
            Assert.AreNotEqual(0, metadata.Entities.Length);
        }
    }
}
