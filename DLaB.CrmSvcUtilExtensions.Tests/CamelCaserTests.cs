using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DLaB.ModelBuilderExtensions.Tests
{
    [TestClass]
    public class CamelCaserTests
    {
        [TestMethod]
        public void Case_ShouldPreferId()
        {
            var dictionary = new Dictionary<int, HashSet<string>>
            {
                [2] = new HashSet<string>(new[] { "id", }),
                [3] = new HashSet<string>(new[] { "arc", "did", "lid" }),
                [5] = new HashSet<string>(new[] { "block", "clone" }),
                [6] = new HashSet<string>(new[] { "cloned", "parcel" })
            };
            var sut = new CamelCaser(dictionary);

            //Assert.AreEqual("ClonedId", sut.CaseWord("clonedid"));
            Assert.AreEqual("ParcelIdBlock", sut.CaseWord("parcelidblock"));
        }

        [TestMethod]
        public void Case_ShouldPreferLessWords_WhenCasingCreditOnHold()
        {
            var dictionary = new Dictionary<int, HashSet<string>>
            {
                [2] = new HashSet<string>(new[] { "re", "di", "on" }),
                [3] = new HashSet<string>(new[] { "ton" }),
                [4] = new HashSet<string>(new[] { "hold" }),
                [6] = new HashSet<string>(new[] { "credit" })
            };
            var sut = new CamelCaser(dictionary);

            Assert.AreEqual("CreditOnHold", sut.CaseWord("creditonhold"));
        }

        [TestMethod]
        public void Case_ShouldIgnoreSplitByNumbers()
        {
            var dictionary = new Dictionary<int, HashSet<string>>
            {
                [2] = new HashSet<string>(new[] { "id" }),
                [3] = new HashSet<string>(new[] { "crm", "mid" }),
            };
            var sut = new CamelCaser(dictionary);

            Assert.AreEqual("CrmId2", sut.CaseWord("crmid2"));
        }
    }
}
