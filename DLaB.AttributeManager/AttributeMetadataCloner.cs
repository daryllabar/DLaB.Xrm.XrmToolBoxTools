using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Messages;

// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBeMadeStatic.Local

namespace DLaB.AttributeManager
{
    public partial class Logic
    {
        #region Clone Attributes

        private AttributeMetadata CloneAttributes(AttributeMetadata att, string schemaName, AttributeMetadata newAttributeType)
        {
            var clone = CloneAttributes((dynamic)(newAttributeType ?? att));

            clone.CanModifyAdditionalSettings = att.CanModifyAdditionalSettings;
            clone.Description = att.Description;
            clone.DisplayName = att.DisplayName;
            clone.ExtensionData = att.ExtensionData;
            clone.IsAuditEnabled = att.IsAuditEnabled;
            clone.IsCustomizable = att.IsCustomizable;
            clone.IsRenameable = att.IsRenameable;
            clone.IsSecured = att.IsSecured;
            clone.IsValidForAdvancedFind = att.IsValidForAdvancedFind;
            clone.LinkedAttributeId = att.LinkedAttributeId;
            clone.RequiredLevel = att.RequiredLevel;


            // Fix for issue 1468 Inactive Language Causing Error
            RemoveInvalidLanguageLocalizedLabels(att.Description);
            RemoveInvalidLanguageLocalizedLabels(att.DisplayName);

            clone.LogicalName = schemaName.ToLower();
            clone.SchemaName = schemaName;

            // Update EntityLogicalName for other methods to use
            SetEntityLogicalName(clone, att.EntityLogicalName);
            return clone;
        }

        private AttributeMetadata CloneAttributes(object att)
        {
            throw new NotSupportedException("Unexpected AttributeMetadata Type " + att.GetType());
        }

        // ReSharper disable once UnusedParameter.Local
        private AttributeMetadata CloneAttributes(EntityNameAttributeMetadata att)
        {
            return new EntityNameAttributeMetadata();
        }

        private AttributeMetadata CloneAttributes(PicklistAttributeMetadata att)
        {
            var picklist =  new PicklistAttributeMetadata
            {
                FormulaDefinition = att.FormulaDefinition,
                DefaultFormValue = att.DefaultFormValue,
                OptionSet = att.OptionSet
            };

            if (!picklist.OptionSet.IsGlobal.Value)
            {
                // Can't reuse an existing local Option Set
                var optionSet = picklist.OptionSet;
                if (optionSet.Name.EndsWith(TempPostfix))
                {
                    optionSet.Name = optionSet.Name.Remove(optionSet.Name.Length - TempPostfix.Length);
                }
                else
                {
                    optionSet.Name = optionSet.Name + TempPostfix;
                }
                optionSet.MetadataId = null;
            }
            return picklist;
        }

        private AttributeMetadata CloneAttributes(DateTimeAttributeMetadata att)
        {
            return new DateTimeAttributeMetadata
            {
                Format = att.Format,
                ImeMode = att.ImeMode,
                FormulaDefinition = att.FormulaDefinition
            };
        }

        private AttributeMetadata CloneAttributes(BooleanAttributeMetadata att)
        {
            return new BooleanAttributeMetadata
            {
                DefaultValue = att.DefaultValue,
                FormulaDefinition = att.FormulaDefinition,
                OptionSet = new BooleanOptionSetMetadata(att.OptionSet.TrueOption, att.OptionSet.FalseOption)
            };
        }

        private AttributeMetadata CloneAttributes(IntegerAttributeMetadata att)
        {
            return new IntegerAttributeMetadata
            {
                Format = att.Format,
                MaxValue = att.MaxValue,
                MinValue = att.MinValue,
                FormulaDefinition = att.FormulaDefinition
            };
        }

        private AttributeMetadata CloneAttributes(DecimalAttributeMetadata att)
        {
            return new DecimalAttributeMetadata
            {
                MaxValue = att.MaxValue,
                MinValue = att.MinValue,
                Precision = att.Precision,
                ImeMode = att.ImeMode,
                FormulaDefinition = att.FormulaDefinition
            };
        }

        private AttributeMetadata CloneAttributes(StringAttributeMetadata att)
        {
            return new StringAttributeMetadata
            {
                Format = att.Format,
                FormatName = att.FormatName,
                ImeMode = att.ImeMode,
                MaxLength = att.MaxLength,
                YomiOf = att.YomiOf,
                FormulaDefinition = att.FormulaDefinition
            };
        }

        // ReSharper disable once UnusedParameter.Local
        private AttributeMetadata CloneAttributes(StatusAttributeMetadata att)
        {
            return new StatusAttributeMetadata();
        }

        // ReSharper disable once UnusedParameter.Local
        private AttributeMetadata CloneAttributes(BigIntAttributeMetadata att)
        {
            return new BigIntAttributeMetadata();
        }

        private AttributeMetadata CloneAttributes(LookupAttributeMetadata att)
        {
            return new LookupAttributeMetadata
            {
                Targets = att.Targets
            };
        }

        private AttributeMetadata CloneAttributes(DoubleAttributeMetadata att)
        {
            return new DoubleAttributeMetadata
            {
                ImeMode = att.ImeMode,
                MaxValue = att.MaxValue,
                MinValue = att.MinValue,
                Precision = att.Precision
            };
        }

        private AttributeMetadata CloneAttributes(ImageAttributeMetadata att)
        {
            return new ImageAttributeMetadata
            {
                IsPrimaryImage = att.IsPrimaryImage
            };
        }

        private AttributeMetadata CloneAttributes(MoneyAttributeMetadata att)
        {
            return new MoneyAttributeMetadata
            {
                ImeMode = att.ImeMode,
                MaxValue = att.MaxValue,
                MinValue = att.MinValue,
                Precision = att.Precision,
                PrecisionSource = att.PrecisionSource,
                CalculationOf = att.CalculationOf,
                FormulaDefinition = att.FormulaDefinition
            };
        }

        // ReSharper disable once UnusedParameter.Local
        private AttributeMetadata CloneAttributes(StateAttributeMetadata att)
        {
            return new StateAttributeMetadata();
        }

        private AttributeMetadata CloneAttributes(MemoAttributeMetadata att)
        {
            return new MemoAttributeMetadata
            {
                Format = att.Format,
                ImeMode = att.ImeMode,
                MaxLength = att.MaxLength
            };
        }

        // ReSharper disable once UnusedParameter.Local
        private AttributeMetadata CloneAttributes(ManagedPropertyAttributeMetadata att)
        {
            return new ManagedPropertyAttributeMetadata();
        }

        #endregion Clone Attributes

        private AttributeMetadata CreateAttributeWithDifferentNameInternal(IOrganizationService service, AttributeMetadata existingAtt, string newSchemaName, AttributeMetadata newAttributeType)
        {
            var newAttribute = CloneAttributes(existingAtt, newSchemaName, newAttributeType);
            var response = ((CreateAttributeResponse)service.Execute(new CreateAttributeRequest
            {
                Attribute = newAttribute,
                EntityName = newAttribute.EntityLogicalName
            }));
            newAttribute.MetadataId = response.AttributeId;
            return newAttribute;
        }

        private AttributeMetadata CreateAttributeWithDifferentNameInternal(IOrganizationService service, LookupAttributeMetadata existingAtt, string newSchemaName, AttributeMetadata newAttributeType)
        {
            if (newAttributeType != null)
            {
                throw new NotImplementedException("Updating that attribute type for Lookup Attributes to a different type is not implemented!");
            }
            var clone = (LookupAttributeMetadata)CloneAttributes(existingAtt, newSchemaName, null);
            foreach (var relationship in Metadata.ManyToOneRelationships.Where(r => r.ReferencingAttribute == existingAtt.LogicalName && r.ReferencingEntity == existingAtt.EntityLogicalName))
            {
                UpdateRelationshipSchemaName(relationship, newSchemaName);

                relationship.ReferencingAttribute = null;
                relationship.ReferencedAttribute = null;
                Trace("Creating Relationship " + relationship.SchemaName);
                service.Execute(new CreateOneToManyRequest { OneToManyRelationship = relationship, Lookup = clone });
            }

            foreach (var relationship in Metadata.OneToManyRelationships.Where(r => r.ReferencedAttribute == existingAtt.LogicalName && r.ReferencedEntity == existingAtt.EntityLogicalName))
            {
                UpdateRelationshipSchemaName(relationship, newSchemaName);

                relationship.ReferencingAttribute = null;
                relationship.ReferencedAttribute = null;
                Trace("Creating Relationship " + relationship.SchemaName);
                service.Execute(new CreateOneToManyRequest { OneToManyRelationship = relationship, Lookup = clone });
            }

            return clone;
        }

        private void UpdateRelationshipSchemaName(OneToManyRelationshipMetadata relationship, string newSchemaName)
        {
            // Add or Remove Temp Post Fix or update last index of existing Att Schema to newSchema
            if (relationship.SchemaName.EndsWith(TempPostfix))
            {
                relationship.SchemaName = relationship.SchemaName.Remove(relationship.SchemaName.LastIndexOf(TempPostfix, StringComparison.Ordinal));
            }
            else if (newSchemaName.EndsWith(TempPostfix))
            {
                relationship.SchemaName = relationship.SchemaName + TempPostfix;
            }
            else
            {
                // Format {Prefix of SchemaName} + {Referenced Entity} + "_" + {Referencing Entity} + "_" + {End of SchemaName}
                var index = newSchemaName.IndexOf("_", StringComparison.Ordinal) + 1;
                var prefix = newSchemaName.Substring(0, index);
                var postFix = newSchemaName.Substring(index, newSchemaName.Length - index);

                relationship.SchemaName = string.Format("{0}{1}_{2}_{3}", prefix, relationship.ReferencedEntity, relationship.ReferencingEntity, postFix);
            }
        }
    }
}
