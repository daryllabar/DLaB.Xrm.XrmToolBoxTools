using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.ModelBuilderExtensions.Tests
{
    [TestClass]
    public class BlacklistLogicTests
    {
        private readonly HashSet<string> _pear = new HashSet<string>(new List<string> { "pear" });
        private readonly List<string> _or = new List<string> { "or" };
        private readonly HashSet<string> _emptySet = new HashSet<string>();
        private readonly List<string> _emptyList = new List<string>();

        [TestMethod]
        public void BlacklistLogic_Empty_Should_OnlyAllowEverything()
        {
            var sut = new BlacklistLogic(_emptySet, _emptyList);
            Assert.IsTrue(sut.IsAllowed("apple"));
            Assert.IsTrue(sut.IsAllowed("banana"));
            Assert.IsTrue(sut.IsAllowed("pear"));
            Assert.IsTrue(sut.IsAllowed("orange"));
        }


        [TestMethod]
        public void BlacklistLogic_WithBlacklists_Should_OnlyAllowNonBlacklisted()
        {
            var sut = new BlacklistLogic(_pear, _or);
            Assert.IsTrue(sut.IsAllowed("apple"));
            Assert.IsTrue(sut.IsAllowed("banana"));
            Assert.IsFalse(sut.IsAllowed("pear"));
            Assert.IsFalse(sut.IsAllowed("orange"));
        }
    }
}
