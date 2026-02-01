using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.ModelBuilderExtensions.Tests
{
    [TestClass]
    public class DLaBModelBuilderSettingsTests
    {
        [TestMethod]
        public void ProcessFlags_ShouldNotCopyNullValues()
        {
            // Arrange
            var settings = new DLaBModelBuilderSettings
            {
                DLaBModelBuilderFlags = new DLaBModelBuilderFlags
                {
                    MessageLogicalFieldName = null
                },
                DLaBModelBuilder = new DLaBModelBuilder
                {
                    MessageLogicalFieldName = "OriginalValue"
                }
            };

            // Act
            settings.ProcessCustomFlags();

            // Assert
            Assert.AreEqual("OriginalValue", settings.DLaBModelBuilder.MessageLogicalFieldName);
        }

        [TestMethod]
        public void ProcessFlags_ShouldNotCopyEmptyStrings()
        {
            // Arrange
            var settings = new DLaBModelBuilderSettings
            {
                DLaBModelBuilderFlags = new DLaBModelBuilderFlags
                {
                    OrgEntityClassName = string.Empty
                },
                DLaBModelBuilder = new DLaBModelBuilder
                {
                    OrgEntityClassName = "OriginalValue"
                }
            };

            // Act
            settings.ProcessCustomFlags();

            // Assert
            Assert.AreEqual("OriginalValue", settings.DLaBModelBuilder.OrgEntityClassName);
        }

        [TestMethod]
        public void ProcessFlags_ShouldNotCopyEmptyCollections()
        {
            // Arrange
            var originalList = new List<string> { "Item1", "Item2" };
            var settings = new DLaBModelBuilderSettings
            {
                DLaBModelBuilderFlags = new DLaBModelBuilderFlags
                {
                    InvalidStringsForPropertiesNeedingNullableTypes = new List<string>()
                },
                DLaBModelBuilder = new DLaBModelBuilder
                {
                    InvalidStringsForPropertiesNeedingNullableTypes = originalList
                }
            };

            // Act
            settings.ProcessCustomFlags();

            // Assert
            Assert.AreEqual(2, settings.DLaBModelBuilder.InvalidStringsForPropertiesNeedingNullableTypes.Count);
            Assert.AreEqual("Item1", settings.DLaBModelBuilder.InvalidStringsForPropertiesNeedingNullableTypes[0]);
        }

        [TestMethod]
        public void ProcessFlags_ShouldCopyNonNullStringValues()
        {
            // Arrange
            var settings = new DLaBModelBuilderSettings
            {
                DLaBModelBuilderFlags = new DLaBModelBuilderFlags
                {
                    MessageLogicalFieldName = "CustomValue"
                },
                DLaBModelBuilder = new DLaBModelBuilder
                {
                    MessageLogicalFieldName = "OriginalValue"
                }
            };

            // Act
            settings.ProcessCustomFlags();

            // Assert
            Assert.AreEqual("CustomValue", settings.DLaBModelBuilder.MessageLogicalFieldName);
        }

        [TestMethod]
        public void ProcessFlags_ShouldCopyBooleanValues()
        {
            // Arrange
            var settings = new DLaBModelBuilderSettings
            {
                DLaBModelBuilderFlags = new DLaBModelBuilderFlags
                {
                    AddPrimaryAttributeConsts = false
                },
                DLaBModelBuilder = new DLaBModelBuilder
                {
                    AddPrimaryAttributeConsts = true
                }
            };

            // Act
            settings.ProcessCustomFlags();

            // Assert
            Assert.IsFalse(settings.DLaBModelBuilder.AddPrimaryAttributeConsts);
        }

        [TestMethod]
        public void ProcessFlags_ShouldCopyNonEmptyCollections()
        {
            // Arrange
            var customList = new List<string> { "Custom1", "Custom2" };
            var settings = new DLaBModelBuilderSettings
            {
                DLaBModelBuilderFlags = new DLaBModelBuilderFlags
                {
                    InvalidStringsForPropertiesNeedingNullableTypes = customList
                },
                DLaBModelBuilder = new DLaBModelBuilder
                {
                    InvalidStringsForPropertiesNeedingNullableTypes = new List<string> { "Original" }
                }
            };

            // Act
            settings.ProcessCustomFlags();

            // Assert
            Assert.AreEqual(2, settings.DLaBModelBuilder.InvalidStringsForPropertiesNeedingNullableTypes.Count);
            Assert.AreEqual("Custom1", settings.DLaBModelBuilder.InvalidStringsForPropertiesNeedingNullableTypes[0]);
            Assert.AreEqual("Custom2", settings.DLaBModelBuilder.InvalidStringsForPropertiesNeedingNullableTypes[1]);
        }

        [TestMethod]
        public void ProcessFlags_ShouldCopyDictionaryValues()
        {
            // Arrange
            var customDict = new Dictionary<string, string>
            {
                { "Key1", "Value1" },
                { "Key2", "Value2" }
            };
            var settings = new DLaBModelBuilderSettings
            {
                DLaBModelBuilderFlags = new DLaBModelBuilderFlags
                {
                    LabelTextReplacement = customDict
                },
                DLaBModelBuilder = new DLaBModelBuilder
                {
                    LabelTextReplacement = new Dictionary<string, string> { { "OldKey", "OldValue" } }
                }
            };

            // Act
            settings.ProcessCustomFlags();

            // Assert
            Assert.AreEqual(2, settings.DLaBModelBuilder.LabelTextReplacement.Count);
            Assert.AreEqual("Value1", settings.DLaBModelBuilder.LabelTextReplacement["Key1"]);
        }

        [TestMethod]
        public void ProcessFlags_ShouldHandleNullDLaBModelBuilderFlags()
        {
            // Arrange
            var settings = new DLaBModelBuilderSettings
            {
                DLaBModelBuilderFlags = null,
                DLaBModelBuilder = new DLaBModelBuilder
                {
                    MessageLogicalFieldName = "OriginalValue"
                }
            };

            // Act
            settings.ProcessCustomFlags();

            // Assert
            Assert.AreEqual("OriginalValue", settings.DLaBModelBuilder.MessageLogicalFieldName);
        }

        [TestMethod]
        public void ProcessFlags_ShouldCopyEnumValues()
        {
            // Arrange
            var settings = new DLaBModelBuilderSettings
            {
                DLaBModelBuilderFlags = new DLaBModelBuilderFlags
                {
                    NamingServiceMethodsToUseDefault = NamingServiceMethods.GetNameForServiceContext
                },
                DLaBModelBuilder = new DLaBModelBuilder
                {
                    NamingServiceMethodsToUseDefault = NamingServiceMethods.None
                }
            };

            // Act
            settings.ProcessCustomFlags();

            // Assert
            Assert.AreEqual(NamingServiceMethods.GetNameForServiceContext, settings.DLaBModelBuilder.NamingServiceMethodsToUseDefault);
        }
    }
}