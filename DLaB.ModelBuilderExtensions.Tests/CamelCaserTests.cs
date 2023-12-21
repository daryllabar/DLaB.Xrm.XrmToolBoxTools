using System.Activities.Expressions;
using System.Collections.Generic;
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
        public void Case_ShouldUsePreferredWords()
        {
            var sut = new CamelCaser("final", "finals", "sent", "ent");

            Assert.AreEqual("FinalSent", sut.CaseWord("finalsent"));
        }

        [TestMethod]
        public void Case_ShouldPreferBackwards()
        {
            var sut = new CamelCaser(new List<string>
                {
                    "AssignedTo"
                },
                "all", "records", "on", "son", "record",
                "contact", "contacts", "at", "sat", "location",
                "sale", "sales", "steam", "team",
                "assign", "assigned", "to", "ops", "too", "edt", "oops",
                "create", "a", "lead", "al", "ad", "re", "tea");
            
            
            Assert.AreEqual("AllRecordsOn", sut.CaseWord("allrecordson"));
            Assert.AreEqual("ContactsAtLocation", sut.CaseWord("contactsatlocation"));
            Assert.AreEqual("SalesTeam", sut.CaseWord("salesteam"));
            Assert.AreEqual("AssignedToOps", sut.CaseWord("assignedtoops"));
            Assert.AreEqual("_AssignedTo_", sut.CaseWord("_assignedto_"));
            Assert.AreEqual("_AssignedTo", sut.CaseWord("_assignedto"));
            Assert.AreEqual("AssignedTo_", sut.CaseWord("assignedto_"));
            Assert.AreEqual("CreateA", sut.CaseWord("createa"));
            Assert.AreEqual("CreateALead", sut.CaseWord("createalead"));
        }

        [TestMethod]
        public void Case_ShouldNotPreferSingleLetterWords()
        {
            var sut = new CamelCaser("costa", "co", "star");

            Assert.AreEqual("CoStar", sut.CaseWord("costar"));
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
            var sut = new CamelCaser("do", "not", "note", "mail", "email",
                "rec","ur","ring", "recurring",
                "or","ted",
                "is", "back", "office", "customer", "ice");

            Assert.AreEqual("DoNotEmail", sut.CaseWord("donotemail"));
            Assert.AreEqual("NotRecurring", sut.CaseWord("notrecurring"));
            Assert.AreEqual("IsBackOfficeCustomer", sut.CaseWord("isbackofficecustomer"));
        }

        [TestMethod]
        public void Case_ShouldCasePicklist()
        {
            var sut = new CamelCaser("pick", "list", "picklist");
            Assert.AreEqual("Picklist", sut.CaseWord("picklist"));
        }
    }
}
