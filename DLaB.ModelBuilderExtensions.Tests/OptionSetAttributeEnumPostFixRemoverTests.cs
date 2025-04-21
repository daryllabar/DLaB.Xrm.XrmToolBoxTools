using DLaB.ModelBuilderExtensions.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.CodeDom;
using System.Collections.Generic;

namespace DLaB.ModelBuilderExtensions.Tests
{
    [TestClass]
    public class OptionSetAttributeEnumPostFixRemoverTests
    {
        [TestInitialize]
        public void Initialize()
        {

        }

        [TestMethod]
        public void RemoveIncorrectlyPostfixedEnumFromPropertyType_NoEnum_Should_Skip()
        {
            var sut = new OptionSetAttributeEnumPostFixRemover(null);

            var property = new CodeMemberProperty();
            try
            {
                sut.RemoveIncorrectlyPostfixedEnumFromPropertyType(null, property, null);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void RemoveIncorrectlyPostfixedEnumFromPropertyType_NonOptionSetProperty_Should_Skip()
        {
            var sut = new OptionSetAttributeEnumPostFixRemover(null);
            var attributeLogicalName = "NotAnOptionSet".ToLower();
            var entityMetadata = CreateEntityMetadata(attributeLogicalName, AttributeTypeCode.Integer);
            var property = new CodeMemberProperty
            {
                CustomAttributes = new CodeAttributeDeclarationCollection(new[] {new CodeAttributeDeclaration(new CodeTypeReference
                        {
                            BaseType = Extensions.XrmAttributeLogicalName,
                        },
                        new CodeAttributeArgument(new CodePrimitiveExpression(attributeLogicalName))
                    )
                }),
                Type = new CodeTypeReference
                {
                    BaseType = "intEnum?"
                }
            };

            try
            {
                sut.RemoveIncorrectlyPostfixedEnumFromPropertyType(null, property, entityMetadata);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        private static EntityMetadata CreateEntityMetadata(string attributeLogicalName, AttributeTypeCode attributeType)
        {
            var entityMetadata = new EntityMetadata();
            var attributesField = typeof(EntityMetadata).GetField("_attributes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (attributesField != null)
            {
                var attributeMetadata = new AttributeMetadata
                {
                    LogicalName = attributeLogicalName
                };

                var attributeTypeProperty = typeof(AttributeMetadata).GetField("_attributeType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (attributeTypeProperty != null)
                {
                    attributeTypeProperty.SetValue(attributeMetadata, attributeType);
                }
                attributesField.SetValue(entityMetadata, new [] { attributeMetadata });
            }

            return entityMetadata;
        }

        [TestMethod]
        public void RemoveIncorrectlyPostfixedEnumFromPropertyType_NonProperty_Should_Skip()
        {
            var sut = new OptionSetAttributeEnumPostFixRemover(null);

            var field = new CodeMemberField();
            try
            {
                sut.RemoveIncorrectlyPostfixedEnumFromPropertyType(null, field, null);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void RemoveIncorrectlyPostfixedEnumFromPropertyType_WithEnumPostFix_Should_Remove()
        {
            var sut = new OptionSetAttributeEnumPostFixRemover(null)
            {
                ServiceCache = new ServiceCache(null)
                {
                    MetadataForEnumsByName = new Dictionary<string, OptionSetMetadataBase>
                    {
                        { "MyOptionSet", new OptionSetMetadata() }
                    }
                }
            };
            
            var attributeLogicalName = "NotAnOptionSet".ToLower();
            var entityMetadata = CreateEntityMetadata(attributeLogicalName, AttributeTypeCode.Picklist);
            var property = new CodeMemberProperty
            {
                CustomAttributes = new CodeAttributeDeclarationCollection(new[] {new CodeAttributeDeclaration(new CodeTypeReference
                        {
                            BaseType = Extensions.XrmAttributeLogicalName,
                        },
                        new CodeAttributeArgument(new CodePrimitiveExpression(attributeLogicalName))
                    )
                }),
                Type = new CodeTypeReference
                {
                    BaseType = "MyOptionSetEnum?"
                }
            };

            sut.RemoveIncorrectlyPostfixedEnumFromPropertyType(null, property, entityMetadata);
            
            Assert.AreEqual(property.Type.BaseType, "MyOptionSet");
        }
    }
}
