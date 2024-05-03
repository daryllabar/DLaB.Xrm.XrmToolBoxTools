using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.ModelBuilderExtensions.Tests
{
    [TestClass]
    public class AttributeBlacklistLogicTests
    {
        public struct Filters
        {
            public const string EndsWithMsdn_ = "*msdn_";
            public const string StartsWithMsdn_ = "msdn_*";
            public const string Msdn_Anywhere = "*msdn_*";
        }

        [TestMethod]
        public void AttributeBlacklistLogic_ForContactWithAnywhereFilter()
        {
            var sut = new AttributeBlacklistLogic(new HashSet<string>(new[] { "contact." + Filters.Msdn_Anywhere }));
            Assert.IsTrue(sut.IsAllowed("a", "msdn_"));
            Assert.IsTrue(sut.IsAllowed("a", "msdn_s"));
            Assert.IsTrue(sut.IsAllowed("a", "s_msdn_"));
            Assert.IsTrue(sut.IsAllowed("a", "s_msdn_s"));
            Assert.IsFalse(sut.IsAllowed("contact", "msdn_"));
            Assert.IsFalse(sut.IsAllowed("contact", "msdn_s"));
            Assert.IsFalse(sut.IsAllowed("contact", "s_msdn_"));
            Assert.IsFalse(sut.IsAllowed("contact", "s_msdn_s"));
        }


        [TestMethod]
        public void AttributeBlacklistLogic_ForContactWithEndsWithFilter()
        {
            var sut = new AttributeBlacklistLogic(new HashSet<string>(new[] { "contact." + Filters.EndsWithMsdn_ }));
            Assert.IsTrue(sut.IsAllowed("a", "msdn_"));
            Assert.IsTrue(sut.IsAllowed("a", "msdn_s"));
            Assert.IsTrue(sut.IsAllowed("a", "s_msdn_"));
            Assert.IsTrue(sut.IsAllowed("a", "s_msdn_s"));
            Assert.IsFalse(sut.IsAllowed("contact", "msdn_"));
            Assert.IsTrue(sut.IsAllowed("contact", "msdn_s"));
            Assert.IsFalse(sut.IsAllowed("contact", "s_msdn_"));
            Assert.IsTrue(sut.IsAllowed("contact", "s_msdn_s"));
        }

        [TestMethod]
        public void AttributeBlacklistLogic_ForContactWithStartsWithFilter()
        {
            var sut = new AttributeBlacklistLogic(new HashSet<string>(new[] { "contact." + Filters.StartsWithMsdn_ }));
            Assert.IsTrue(sut.IsAllowed("a", "msdn_"));
            Assert.IsTrue(sut.IsAllowed("a", "msdn_s"));
            Assert.IsTrue(sut.IsAllowed("a", "s_msdn_"));
            Assert.IsTrue(sut.IsAllowed("a", "s_msdn_s"));
            Assert.IsFalse(sut.IsAllowed("contact", "msdn_"));
            Assert.IsFalse(sut.IsAllowed("contact", "msdn_s"));
            Assert.IsTrue(sut.IsAllowed("contact", "s_msdn_"));
            Assert.IsTrue(sut.IsAllowed("contact", "s_msdn_s"));
        }

        [TestMethod]
        public void AttributeBlacklistLogic_WithAnywhereFilter()
        {
            var sut = new AttributeBlacklistLogic(new HashSet<string>(new[] { Filters.Msdn_Anywhere }));
            Assert.IsFalse(sut.IsAllowed("a", "msdn_"));
            Assert.IsFalse(sut.IsAllowed("a", "msdn_s"));
            Assert.IsFalse(sut.IsAllowed("a", "s_msdn_"));
            Assert.IsFalse(sut.IsAllowed("a", "s_msdn_s"));
        }

        [TestMethod]
        public void AttributeBlacklistLogic_WithEndsWithFilter()
        {
            var sut = new AttributeBlacklistLogic(new HashSet<string>(new[] { Filters.EndsWithMsdn_ }));
            Assert.IsFalse(sut.IsAllowed("a", "msdn_"));
            Assert.IsTrue(sut.IsAllowed("a", "msdn_s"));
            Assert.IsFalse(sut.IsAllowed("a", "s_msdn_"));
            Assert.IsTrue(sut.IsAllowed("a", "s_msdn_s"));
        }

        [TestMethod]
        [DataRow()]
        [DataRow("a")]
        [DataRow("a", "b")]
        [DataRow("a*", "*a")]
        public void AttributeBlacklistLogic_WithNoMatches(params string[] values)
        {
            var sut = new AttributeBlacklistLogic(new HashSet<string>(values));
            Assert.IsTrue(sut.IsAllowed("a", "msdn_"));
            Assert.IsTrue(sut.IsAllowed("a", "msdn_s"));
            Assert.IsTrue(sut.IsAllowed("a", "s_msdn_"));
            Assert.IsTrue(sut.IsAllowed("a", "s_msdn_s"));
        }

        [TestMethod]
        public void AttributeBlacklistLogic_WithStartsAndEndsWithFilters()
        {
            var sut = new AttributeBlacklistLogic(new HashSet<string>(new[] { Filters.StartsWithMsdn_, Filters.EndsWithMsdn_ }));
            Assert.IsFalse(sut.IsAllowed("a", "msdn_"));
            Assert.IsFalse(sut.IsAllowed("a", "msdn_s"));
            Assert.IsFalse(sut.IsAllowed("a", "s_msdn_"));
            Assert.IsTrue(sut.IsAllowed("a", "s_msdn_s"));
        }

        [TestMethod]
        public void AttributeBlacklistLogic_WithStartsWithFilter()
        {
            var sut = new AttributeBlacklistLogic(new HashSet<string>(new[] { Filters.StartsWithMsdn_ }));
            Assert.IsFalse(sut.IsAllowed("a", "msdn_"));
            Assert.IsFalse(sut.IsAllowed("a", "msdn_s"));
            Assert.IsTrue(sut.IsAllowed("a", "s_msdn_"));
            Assert.IsTrue(sut.IsAllowed("a", "s_msdn_s"));
        }
    }
}
