using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.CrmSvcUtilExtensions.Tests
{
    [TestClass]
    public class WhitelistBlacklistLogicTests
    {
        private readonly HashSet<string> _apple = new HashSet<string>(new List<string> { "apple" });
        private readonly List<string> _ba = new List<string> { "ba" };
        private readonly HashSet<string> _pear = new HashSet<string>(new List<string> { "pear" });
        private readonly List<string> _or = new List<string> { "or" };
        private readonly HashSet<string> _emptySet = new HashSet<string>();
        private readonly List<string> _emptyList = new List<string>();

        [TestMethod]
        public void WhitelistBlacklistLogic_WithWhitelist_Should_OnlyAllowWhitelisted()
        {
            var sut = new WhitelistBlacklistLogic(_apple, _ba, _emptySet, _emptyList);
            Assert.IsTrue(sut.IsAllowed("apple"));
            Assert.IsTrue(sut.IsAllowed("banana"));
            Assert.IsFalse(sut.IsAllowed("pear"));
            Assert.IsFalse(sut.IsAllowed("orange"));
        }

        [TestMethod]
        public void WhitelistBlacklistLogic_WithNothing_Should_OnlyAllowAnything()
        {
            var sut = new WhitelistBlacklistLogic(_emptySet, _emptyList, _emptySet, _emptyList);
            Assert.IsTrue(sut.IsAllowed("apple"));
            Assert.IsTrue(sut.IsAllowed("banana"));
            Assert.IsTrue(sut.IsAllowed("pear"));
            Assert.IsTrue(sut.IsAllowed("orange"));
        }

        [TestMethod]
        public void WhitelistBlacklistLogic_WithWhiteAndBlacklists_Should_OnlyAllowWhitelisted()
        {
            var sut = new WhitelistBlacklistLogic(_apple, _ba, _pear, _or);
            Assert.IsTrue(sut.IsAllowed("apple"));
            Assert.IsTrue(sut.IsAllowed("banana"));
            Assert.IsFalse(sut.IsAllowed("pear"));
            Assert.IsFalse(sut.IsAllowed("orange"));
        }

        [TestMethod]
        public void WhitelistBlacklistLogic_WithBlacklists_Should_OnlyAllowNonBlacklisted()
        {
            var sut = new WhitelistBlacklistLogic(_emptySet, _emptyList, _pear, _or);
            Assert.IsTrue(sut.IsAllowed("apple"));
            Assert.IsTrue(sut.IsAllowed("banana"));
            Assert.IsFalse(sut.IsAllowed("pear"));
            Assert.IsFalse(sut.IsAllowed("orange"));
        }
    }
}
