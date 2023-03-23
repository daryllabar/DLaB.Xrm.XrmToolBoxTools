using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.ModelBuilderExtensions.Tests
{
    [TestClass]
    public class CamelCaserTests
    {
        [TestMethod]
        public void Case_ShouldPreferId()
        {
            var sut = new CamelCaser("id", "arc", "did", "lid", "guid", "block", "clone", "cloned", "parcel");

            Assert.AreEqual("ClonedId", sut.CaseWord("clonedid"));
            Assert.AreEqual("ClonedGuid", sut.CaseWord("clonedguid"));
            Assert.AreEqual("ParcelIdBlock", sut.CaseWord("parcelidblock"));
        }

        [TestMethod]
        public void Case_ShouldPreferLessWords_WhenCasingCreditOnHold()
        {
            var sut = new CamelCaser("re", "di", "on", "ton", "hold", "credit");

            Assert.AreEqual("CreditOnHold", sut.CaseWord("creditonhold"));
        }

        [TestMethod]
        public void Case_ShouldIgnoreSplitByNumbers()
        {
            var sut = new CamelCaser("id", "crm", "mid");

            Assert.AreEqual("CrmId2", sut.CaseWord("crmid2"));
        }

        [TestMethod]
        public void Case_ShouldFavorBackwardsProcessing()
        {
            var sut = new CamelCaser("do", "not", "note", "mail", "email");

            Assert.AreEqual("DoNotEmail", sut.CaseWord("donotemail"));
        }
    }
}
