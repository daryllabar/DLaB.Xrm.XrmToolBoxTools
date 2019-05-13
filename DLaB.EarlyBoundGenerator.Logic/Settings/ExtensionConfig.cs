using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace DLaB.EarlyBoundGenerator.Settings
{
    /// <summary>
    /// Serializable class containing all settings that will get written to the CrmSrvUtil.exe.config
    /// </summary>
    [Serializable]
    public class ExtensionConfig
    {
        /// <summary>
        /// Pipe Delimited String containing the prefixes of Actions to be generated.
        /// </summary>
        public string ActionPrefixesWhitelist { get; set; }
        /// <summary>
        /// Pipe Delimited String containing the logical names of Actions to be generated.
        /// </summary>
        public string ActionsWhitelist { get; set; }
        /// <summary>
        /// Pipe Delimited String containing the logical names of Actions to not generate
        /// </summary>
        public string ActionsToSkip { get; set; }
        /// <summary>
        /// Pipe Delimited String containing the prefixes of Actions to not generate.
        /// </summary>
        public string ActionPrefixesToSkip { get; set; }
        /// <summary>
        /// Specifies that the debugger should skip stepping into generated entity files.
        /// </summary>
        public bool AddDebuggerNonUserCode { get; set; }
        /// <summary>
        /// Adds newly created files to Project File
        /// </summary>
        public bool AddNewFilesToProject { get; set; }
        /// <summary>
        /// Specifies that each Action class should be outputted to it's own file
        /// </summary>
        public bool CreateOneFilePerAction { get; set; }
        /// <summary>
        /// Specifies that each Entity class should be outputted to it's own file
        /// </summary>
        public bool CreateOneFilePerEntity { get; set; }
        /// <summary>
        /// Specifies that each OptionSet class should be outputted to it's own file
        /// </summary>
        public bool CreateOneFilePerOptionSet { get; set; }
        /// <summary>
        /// Pipe Delimited String containing the logical names of Entities to not generate
        /// </summary>
        public string EntitiesToSkip { get; set; }
        /// <summary>
        /// Pipe Delimited String containing the logical names of Entities to generate.  If empty, all Entities will be included.
        /// </summary>
        public string EntitiesWhitelist { get; set; }
        /// <summary>
        /// Formatted as a single string in the format of "EntityName1:firstAttributeName, ... ,lastAttributeName|EntityName2:firstAttributeName, ... ,lastAttributeName|..."
        /// Basically split each entity by pipe, use then split by colon, then comma, with the first value being the entityName
        /// Allows for the ability to specify the capitalization of an attribute on an entity
        /// </summary>
        public string EntityAttributeSpecifiedNames { get; set; }
        /// <summary>
        /// Pipe delimited string containing prefixes of entities to not generate.
        /// </summary>
        public string EntityPrefixesToSkip { get; set; }
        /// <summary>
        /// Pipe delimited string containing prefixes of entities to be generated.
        /// </summary>
        public string EntityPrefixesWhitelist { get; set; }
        /// <summary>
        /// Specifies the generation of an Attributes Struct containing logical names for all attributes for the Action
        /// </summary>
        public bool GenerateActionAttributeNameConsts { get; set; }        
        /// <summary>
        /// Specifies the generation of an Attributes Struct containing logical names for all attributes for the Entity
        /// </summary>
        public bool GenerateAttributeNameConsts { get; set; }
        /// <summary>
        /// Specifies the generation of AnonymousType Constructors for entities
        /// </summary>
        public bool GenerateAnonymousTypeConstructor { get; set; }
        /// <summary>
        /// Specifies the generation of Relationships properties for Entities
        /// </summary>
        public bool GenerateEntityRelationships { get; set; }
        /// <summary>
        /// Specifies the generation of Enum properties for option sets
        /// </summary>                                          
        public bool GenerateEnumProperties { get; set; }
        /// <summary>
        /// Specifies that only option sets that are referenced by Entities that are generated.
        /// </summary>                                          
        public bool GenerateOnlyReferencedOptionSets { get; set; }
        /// <summary>
        /// Specifies the Prefix to be used for OptionSets that would normally start with an invalid first character ie "1st"
        /// </summary>
        public string InvalidCSharpNamePrefix { get; set; }
        /// <summary>
        /// Defines that Entities should be created with all atributes as editable.
        /// Helpful for writing linq statements where those attributes are wanting to be returned in the select.
        /// </summary>
        public bool MakeAllFieldsEditable { get; set; }
        /// <summary>
        /// Defines that Entities should be created with editable createdby, createdon, modifiedby, modifiedon, owningbusinessunit, owningteam, and owninguser properties.
        /// Helpful for writing linq statements where those attributes are wanting to be returned in the select
        /// </summary>
        public bool MakeReadonlyFieldsEditable { get; set; }
        /// <summary>
        /// Specifies that the properties of Response Actions should be editable.
        /// </summary>
        public bool MakeResponseActionsEditable { get; set; }
        /// <summary>
        /// Defines the format of Local Option Sets where {0} is the Entity Schema Name, and {1} is the Attribute Schema Name.  
        /// The format Specified in the SDK is {0}{1}, but the default is {0}_{1}, but used to be prefix_{0}_{1}(all lower case)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
        /// </summary>
        public string LocalOptionSetFormat { get; set; }
        /// <summary>
        /// Pipe delimited string containing prefixes of entities to not generate.
        /// </summary>
        public string OptionSetPrefixesToSkip { get; set; }
        /// <summary>
        /// Pipe Delimited String containing the logical names of Option Set Names to not generate
        /// </summary>
        public string OptionSetsToSkip { get; set; }
        /// <summary>
        /// Overrides the default (English:1033) language code used for generating Option Set Value names (the value, not the option set)
        /// </summary>
        public int? OptionSetLanguageCodeOverride { get; set; }
        /// <summary>
        /// Remove the Runtime Version in the header comment
        /// </summary>
        public bool RemoveRuntimeVersionComment { get; set; }
        /// <summary>
        /// Used to manually specify an enum mapping for an OptionSetValue Property on an entity 
        /// Format: EntityName.PropertyName,EnumName|
        /// </summary>
        public string PropertyEnumMappings { get; set; }
        /// <summary>
        /// Used to manually specify an OptionSetValue Property of an entity that doesn't have an enum mapping 
        /// Format: EntityName.PropertyName|
        /// </summary>
        public string UnmappedProperties { get; set; }
        /// <summary>
        /// Creates Local OptionSets Using the Deprecated Naming Convention. prefix_oobentityname_prefix_attribute
        /// </summary>
        /// <value>
        /// <c>true</c> if [use Deprecated Option Set Naming]; otherwise, <c>false</c>.
        /// </value>
        public bool UseDeprecatedOptionSetNaming { get; set; }
        /// <summary>
        /// Uses TFS to checkout files
        /// </summary>
        public bool UseTfsToCheckoutFiles { get; set; }
        /// <summary>
        /// Specifies that the Service Context should inherit from CrmOrganizationServiceContext, and conversly, Entities from Xrm.Client.Entity  --> 
        /// </summary>
        public bool UseXrmClient { get; set; }

        #region NonSerialized Properties

        /// <summary>
        /// If populated, used as the Command Line text to insert into generated header for Actions
        /// </summary>
        [XmlIgnore]
        public string ActionCommandLineText { get; set; }

        /// <summary>
        /// If populated, used as the Command Line text to insert into generated header for Entities
        /// </summary>
        [XmlIgnore]
        public string EntityCommandLineText { get; set; }

        /// <summary>
        /// If populated, used as the Command Line text to insert into generated header for OptionSets
        /// </summary>
        [XmlIgnore]
        public string OptionSetCommandLineText { get; set; }

        #endregion // NonSerialized Properties

        public static ExtensionConfig GetDefault()
        {
            return new ExtensionConfig
            {
                AddDebuggerNonUserCode = true,
                AddNewFilesToProject = true,
                CreateOneFilePerAction = false,
                CreateOneFilePerEntity = false,
                CreateOneFilePerOptionSet = false,
                GenerateActionAttributeNameConsts = false,
                GenerateAttributeNameConsts = false,
                GenerateAnonymousTypeConstructor = true,
                GenerateEntityRelationships = true,
                GenerateEnumProperties = true,
                GenerateOnlyReferencedOptionSets = true,
                ActionPrefixesToSkip = null,
                ActionPrefixesWhitelist = null,
                ActionsWhitelist = null,
                ActionsToSkip = null,
                EntitiesToSkip = null,
                EntitiesWhitelist = null,
                EntityAttributeSpecifiedNames = null,
                EntityPrefixesToSkip = null,
                EntityPrefixesWhitelist = null,
                InvalidCSharpNamePrefix = "_",
                MakeAllFieldsEditable = false,
                MakeReadonlyFieldsEditable = false,
                MakeResponseActionsEditable = false,
                LocalOptionSetFormat = "{0}_{1}",
                OptionSetPrefixesToSkip = null,
                OptionSetsToSkip = null,
                OptionSetLanguageCodeOverride = null,
                PropertyEnumMappings = string.Empty,
                RemoveRuntimeVersionComment = true,
                UnmappedProperties =
                    "DuplicateRule:BaseEntityTypeCode,MatchingEntityTypeCode|" +
                    "InvoiceDetail:InvoiceStateCode|" + 
                    "LeadAddress:AddressTypeCode,ShippingMethodCode|" + 
                    "Organization:CurrencyFormatCode,DateFormatCode,TimeFormatCode,WeekStartDayCode|" +
                    "Quote:StatusCode|" + 
                    "QuoteDetail:QuoteStateCode|" + 
                    "SalesOrderDetail:SalesOrderStateCode|",
                UseDeprecatedOptionSetNaming = false,
                UseTfsToCheckoutFiles = false,
                UseXrmClient = false
            };
        }
    }
}

namespace DLaB.EarlyBoundGenerator.Settings.POCO
{
    /// <summary>
    /// Serializable Class with Nullable types to be able to tell if they are populated or not
    /// </summary>
    public class ExtensionConfig
    {
        public bool? CreateOneFilePerEntity { get; set; }
        public bool? CreateOneFilePerOptionSet { get; set; }
        public bool? GenerateActionAttributeNameConsts { get; set; }
        public bool? GenerateAttributeNameConsts { get; set; }
        public bool? GenerateAnonymousTypeConstructor { get; set; }
        public bool? GenerateEntityRelationships { get; set; }
        public bool? GenerateEnumProperties { get; set; }
        public bool? AddDebuggerNonUserCode { get; set; }
        public bool? CreateOneFilePerAction { get; set; }
        public string ActionPrefixesToSkip { get; set; }
        public string ActionPrefixesWhitelist { get; set; }
        public string ActionsToSkip { get; set; }
        public string ActionsWhitelist { get; set; }
        public string EntitiesToSkip { get; set; }
        public string EntitiesWhitelist { get; set; }
        public string EntityAttributeSpecifiedNames { get; set; }
        public string EntityPrefixesToSkip { get; set; }
        public string EntityPrefixesWhitelist { get; set; }
        public bool? GenerateOnlyReferencedOptionSets { get; set; }
        public string InvalidCSharpNamePrefix { get; set; }
        public bool? MakeAllFieldsEditable { get; set; }
        public bool? MakeReadonlyFieldsEditable { get; set; }
        public bool? MakeResponseActionsEditable { get; set; }
        public string LocalOptionSetFormat { get; set; }
        public string OptionSetPrefixesToSkip { get; set; }
        public string OptionSetsToSkip { get; set; }
        public int? OptionSetLanguageCodeOverride { get; set; }
        public string PropertyEnumMappings { get; set; }
        public string UnmappedProperties { get; set; }
        public bool? AddNewFilesToProject { get; set; }
        public bool? RemoveRuntimeVersionComment { get; set; }
        public bool? UseDeprecatedOptionSetNaming { get; set; }
        public bool? UseTfsToCheckoutFiles { get; set; }
        public bool? UseXrmClient { get; set; }
    }
}