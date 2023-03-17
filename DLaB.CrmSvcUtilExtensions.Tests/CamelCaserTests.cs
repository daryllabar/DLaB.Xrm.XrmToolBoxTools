using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DLaB.ModelBuilderExtensions.Tests
{
    [TestClass]
    public class CamelCaserTests
    {
        [TestMethod]
        public void Case_ShouldPreferId_WhenCasingClonedId()
        {
            var dictionary = new Dictionary<int, HashSet<string>>
            {
                [2] = new HashSet<string>(new[] { "id" }),
                [3] = new HashSet<string>(new[] { "did" }),
                [5] = new HashSet<string>(new[] { "clone" }),
                [6] = new HashSet<string>(new[] { "cloned" })
            };
            var sut = new CamelCaser(dictionary);

            Assert.AreEqual("ClonedId", sut.CaseWord("clonedid"));
        }
    }
}
