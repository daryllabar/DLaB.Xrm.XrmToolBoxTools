using Microsoft.PowerPlatform.Dataverse.ModelBuilderLib;
using Microsoft.Xrm.Sdk;
using System;
using System.CodeDom;
using System.Collections.Generic;

namespace DLaB.ModelBuilderExtensions.Entity
{
    public class OptionSetToIntPropertyGenerator : TypedServiceBase<ICustomizeCodeDomService>
    {
        public bool SuppressINotifyPattern { get => Settings.SuppressINotifyPattern; set => Settings.SuppressINotifyPattern = value; }

        private readonly HashSet<string> _attributeLogicalNamesToConvert = new HashSet<string>(new [] { "record1objecttypecode", "record2objecttypecode" });

        #region Constructors

        public OptionSetToIntPropertyGenerator(ICustomizeCodeDomService defaultService, IDictionary<string, string> parameters) : base(defaultService, parameters)
        {
        }

        public OptionSetToIntPropertyGenerator(ICustomizeCodeDomService defaultService, DLaBModelBuilderSettings settings = null) : base(defaultService, settings)
        {
        }

        #endregion Constructors

        #region ICustomizeCodeDomService Members

        public void CustomizeCodeDom(CodeCompileUnit codeUnit, IServiceProvider services)
        {
            SetServiceCache(services);

            foreach (var type in codeUnit.GetEntityTypes())
            {
                foreach (var member in type.Members)
                {
                    if (!(member is CodeMemberProperty property)
                        || !_attributeLogicalNamesToConvert.Contains(property.GetLogicalName()))
                    {
                        continue;
                    }

                    UpdateEnumOptionSetPropertyToInt(property);
                }
            }
        }

        #endregion

        private void UpdateEnumOptionSetPropertyToInt(CodeMemberProperty enumProp)
        {
            enumProp.Type = new CodeTypeReference(typeof(int?));

            var logicalName = enumProp.GetLogicalName();
            UpdateGet(enumProp, logicalName);
            UpdateSet(enumProp, logicalName);
        }

        private static void UpdateGet(CodeMemberProperty enumProp, string logicalName)
        {
            // Generates the following:
            // Microsoft.Xrm.Sdk.OptionSetValue value = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>(attributeLogicalName);
            // if (value != null)
            // {
            //     return value.Value;
            // }
            // return null;

            if (!enumProp.HasSet)
            {
                return;
            }

            enumProp.GetStatements.Clear();
            var optionSetValueType = new CodeTypeReference(typeof(OptionSetValue));
            // Microsoft.Xrm.Sdk.OptionSetValue value = this.GetAttributeValue<Microsoft.Xrm.Sdk.OptionSetValue>("attributeLogicalName");
            enumProp.GetStatements.Add(new CodeVariableDeclarationStatement(
                optionSetValueType,
                "value",
                new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "GetAttributeValue", optionSetValueType),
                    new CodePrimitiveExpression(logicalName)
                )
            ));

            // if (value != null)
            // {
            //     return value.Value;
            // }
            enumProp.GetStatements.Add(new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeVariableReferenceExpression("value"),
                    CodeBinaryOperatorType.IdentityInequality,
                    new CodePrimitiveExpression(null)
                ),
                new CodeMethodReturnStatement(
                    new CodePropertyReferenceExpression(
                        new CodeVariableReferenceExpression("value"),
                        "Value"
                    )
                )
            ));

            // return null;
            enumProp.GetStatements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(null)));
        }

        private void UpdateSet(CodeMemberProperty property, string logicalName)
        {
            if (!property.HasSet)
            {
                return;
            }

            property.SetStatements.Clear();

            if (!SuppressINotifyPattern)
            {
                // this.OnPropertyChanging("PropName");
                property.SetStatements.Add(new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(), "OnPropertyChanging", new CodePrimitiveExpression(property.Name)
                ));
            }

            // Generates the following:
            // if (value.HasValue)
            // {
            //     this.SetAttributeValue("attributeLogicalName", new Microsoft.Xrm.Sdk.OptionSetValue(value));
            // }
            // else
            // {
            //     this.SetAttributeValue("attributeLogicalName", null);
            // }
            property.SetStatements.Add(
                new CodeConditionStatement(
                    // if (value.HasValue)
                    new CodePropertyReferenceExpression(new CodePropertySetValueReferenceExpression(), "HasValue"),

                    //     this.SetAttributeValue("attributeLogicalName", new Microsoft.Xrm.Sdk.OptionSetValue(value));        
                    new CodeStatement[]
                    {
                        new CodeExpressionStatement(
                            new CodeMethodInvokeExpression(
                                new CodeThisReferenceExpression(),
                                "SetAttributeValue",
                                new CodePrimitiveExpression(logicalName),
                                new CodeObjectCreateExpression(
                                    new CodeTypeReference(typeof(OptionSetValue)),
                                    new CodePropertySetValueReferenceExpression()
                                )
                            )
                        )
                    },

                    //     this.SetAttributeValue("attributeLogicalName", null);
                    new CodeStatement[]
                    {
                        new CodeExpressionStatement(
                            new CodeMethodInvokeExpression(
                                new CodeThisReferenceExpression(),
                                "SetAttributeValue",
                                new CodePrimitiveExpression(logicalName),
                                new CodePrimitiveExpression(null)
                            )
                        )
                    }
                )
            );

            if (!SuppressINotifyPattern)
            {
                // this.OnPropertyChanged("PropName");
                property.SetStatements.Add(new CodeMethodInvokeExpression(
                    new CodeThisReferenceExpression(), "OnPropertyChanged", new CodePrimitiveExpression(property.Name)
                ));
            }
        }
    }
}
