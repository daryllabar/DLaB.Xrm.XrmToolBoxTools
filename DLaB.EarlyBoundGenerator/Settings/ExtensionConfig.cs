using System;
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
        /// Pipe Delimited String containing the logical names of Actions to not generate
        /// </summary>
        public string ActionsToSkip { get; set; }
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
        /// Formated as a single string in the format of "EntityName1,firstAttributeName, ... ,lastAttributeName|EntityName2,firstAttributeName, ... ,lastAttributeName|..."
        /// Basically split each entity by pipe, use then split by comma, with the first value being the entityName
        /// Allows for the ability to specify the capitalization of an attribute on an entity
        /// </summary>
        public string EntityAttributeSpecifiedNames { get; set; }
        /// <summary>
        /// Specifies the generation of an Attributes Struct containing logical names for all attributes for the Entity
        /// </summary>
        public bool GenerateAttributeNameConsts { get; set; }
        /// <summary>
        /// Specifies the generation of AnonymousType Constructors for entities
        /// </summary>
        public bool GenerateAnonymousTypeConstructor { get; set; }
        /// <summary>
        /// Specifies the generation of Enum properties for option sets
        /// </summary>
        public bool GenerateEnumProperties { get; set; }
        /// <summary>
        /// Specifies the Prefix to be used for OptionSets that would normally start with an invalid first character ie "1st"
        /// </summary>
        public string InvalidCSharpNamePrefix { get; set; }
        /// <summary>
        /// Pipe Delimited String containing the logical names of Option Set Names to not generate
        /// </summary>
        public string OptionSetsToSkip { get; set; }
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
                GenerateAttributeNameConsts = false,
                GenerateAnonymousTypeConstructor = true,
                GenerateEnumProperties = true,
                ActionsToSkip = null,
                EntitiesToSkip = null,
                EntityAttributeSpecifiedNames = null,
                InvalidCSharpNamePrefix = "_",
                OptionSetsToSkip = "organization_currencyformatcode|quote_statuscode",
                PropertyEnumMappings =
                    "OpportunityProduct.OpportunityStateCode,opportunity_statuscode|" +
                    "OpportunityProduct.PricingErrorCode,qooi_pricingerrorcode|" +
                    "ResourceGroup.GroupTypeCode,constraintbasedgroup_grouptypecode|",
                RemoveRuntimeVersionComment = true,
                UnmappedProperties =
                    "DuplicateRule,BaseEntityTypeCode,MatchingEntityTypeCode|" +
                    "InvoiceDetail,InvoiceStateCode|" + 
                    "LeadAddress,AddressTypeCode,ShippingMethodCode|" + 
                    "Organization,CurrencyFormatCode,DateFormatCode,TimeFormatCode,WeekStartDayCode|" +
                    "Quote,StatusCode|" + 
                    "QuoteDetail,QuoteStateCode|" + 
                    "SalesOrderDetail,SalesOrderStateCode|",
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
        public bool? GenerateAttributeNameConsts { get; set; }
        public bool? GenerateAnonymousTypeConstructor { get; set; }
        public bool? GenerateEnumProperties { get; set; }
        public bool? AddDebuggerNonUserCode { get; set; }
        public bool? CreateOneFilePerAction { get; set; }
        public string ActionsToSkip { get; set; }
        public string EntitiesToSkip { get; set; }
        public string EntityAttributeSpecifiedNames { get; set; }
        public string InvalidCSharpNamePrefix { get; set; }
        public string OptionSetsToSkip { get; set; }
        public string PropertyEnumMappings { get; set; }
        public string UnmappedProperties { get; set; }
        public bool? AddNewFilesToProject { get; set; }
        public bool? RemoveRuntimeVersionComment { get; set; }
        public bool? UseTfsToCheckoutFiles { get; set; }
        public bool? UseXrmClient { get; set; }
    }
}