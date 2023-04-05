using System;
using System.Diagnostics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
#if DLAB_UNROOT_COMMON_NAMESPACE
using DLaB.Common.Exceptions;
#else
using Source.DLaB.Common.Exceptions;
#endif

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
{
    /// <summary>
    /// Type of Active Attribute
    /// </summary>
    public enum ActiveAttributeType
    {
        /// <summary>
        /// Entity does not support an Active Attribute
        /// </summary>
        None,
        /// <summary>
        /// Entity uses an IsDisabled Attribute
        /// </summary>
        IsDisabled,
        /// <summary>
        /// Entity uses a StateCode Attribute
        /// </summary>
        StateCode,
    }

    /// <summary>
    /// Determines the Active Attribute for the Entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if !DLAB_XRM_DEBUG
    [DebuggerNonUserCode]
#endif
    public class ActivePropertyInfo<T> where T : Entity
    {
        #region Properties

        /// <summary>
        /// Gets or sets the active attribute type.
        /// </summary>
        /// <value>
        /// The active attribute type.
        /// </value>
        public ActiveAttributeType ActiveAttribute { get; set; }
        /// <summary>
        /// Gets or sets the name of the active attribute.
        /// </summary>
        /// <value>
        /// The name of the active attribute.
        /// </value>
        public string AttributeName { get; set; }
        /// <summary>
        /// Gets or sets the state of the active.
        /// </summary>
        /// <value>
        /// The state of the active.
        /// </value>
        public int? ActiveState { get; set; }
        /// <summary>
        /// Gets or sets the not active state code integer value of the entity.
        /// </summary>
        /// <value>
        /// The state of the not active.
        /// </value>
        public int? NotActiveState { get; set; }

        #endregion Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivePropertyInfo{T}"/> class.
        /// </summary>
        /// <exception cref="TypeArgumentException">'Entity' is an invalid type for T.  Please use the LateBoundActivePropertyInfo.</exception>
        public ActivePropertyInfo()
        {
            if (typeof(T) == typeof(Entity))
            {
                throw new TypeArgumentException("'Entity' is an invalid type for T.  Please use the LateBoundActivePropertyInfo.");
            }
            SetAttributeNameAndType(EntityHelper.GetEntityLogicalName<T>());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivePropertyInfo{T}"/> class.
        /// </summary>
        /// <param name="logicalName">Name of the logical.</param>
        /// <exception cref="System.ArgumentNullException">logicalName</exception>
        protected ActivePropertyInfo(string logicalName)
        {
            if (logicalName == null)
            {
                throw new ArgumentNullException(nameof(logicalName));
            }
            SetAttributeNameAndType(logicalName);
        }

        private void SetAttributeNameAndType(string logicalName)
        {
            switch (logicalName)
            {
                case "businessunit":
                case "equipment":
                case "organization":
                case "resource":
                case "systemuser":
                    ActiveAttribute = ActiveAttributeType.IsDisabled;
                    AttributeName = "isdisabled";
                    break;

                #region Default CRM Entites with no active flag

                case "accountleads":
                case "actioncard":
                case "actioncardusersettings":
                case "actioncarduserstate":
                case "activitymimeattachment":
                case "activityparty":
                case "annotation":
                case "annualfiscalcalendar":
                case "appconfiginstance":
                case "appconfigmaster":
                case "appmodule":
                case "appmodulecomponent":
                case "appmoduleroles":
                case "attributemap":
                case "audit":
                case "bookableresourcebookingexchangesyncidmapping":
                case "bulkdeletefailure":
                case "bulkoperationlog":
                case "businessunitnewsarticle":
                case "calendar":
                case "calendarrule":
                case "campaignactivityitem":
                case "campaignitem":
                case "canvasapp":
                case "cardtype":
                case "category":
                case "channelaccessprofileentityaccesslevel":
                case "channelaccessprofileruleitem":
                case "competitor":
                case "competitorproduct":
                case "competitorsalesliterature":
                case "connectionroleassociation":
                case "connectionroleobjecttypecode":
                case "constraintbasedgroup":
                case "contactinvoices":
                case "contactleads":
                case "contactorders":
                case "contactquotes":
                case "contracttemplate":
                case "convertruleitem":
                case "customcontrol":
                case "customcontroldefaultconfig":
                case "customcontrolresource":
                case "customeraddress":
                case "customeropportunityrole":
                case "customerrelationship":
                case "dataperformance":
                case "dependency":
                case "discount":
                case "displaystring":
                case "documenttemplate":
                case "duplicaterecord":
                case "duplicaterulecondition":
                case "dynamicpropertyassociation":
                case "dynamicpropertyinstance":
                case "dynamicpropertyoptionsetitem":
                case "emailsignature":
                case "entitlementchannel":
                case "entitlementcontacts":
                case "entitlementproducts":
                case "entitlementtemplate":
                case "entitlementtemplatechannel":
                case "entitlementtemplateproducts":
                case "entitydataprovider":
                case "entitydatasource":
                case "entitymap":
                case "exchangesyncidmapping":
                case "expanderevent":
                case "fieldpermission":
                case "fieldsecurityprofile":
                case "fixedmonthlyfiscalcalendar":
                case "hierarchyrule":
                case "hierarchysecurityconfiguration":
                case "importjob":
                case "incidentknowledgebaserecord":
                case "invaliddependency":
                case "invoicedetail":
                case "isvconfig":
                case "kbarticlecomment":
                case "kbarticletemplate":
                case "knowledgearticlescategories":
                case "knowledgebaserecord":
                case "leadaddress":
                case "leadcompetitors":
                case "leadproduct":
                case "license":
                case "listmember":
                case "mailboxstatistics":
                case "mailboxtrackingcategory":
                case "mailboxtrackingfolder":
                case "mobileofflineprofile":
                case "mobileofflineprofileitem":
                case "mobileofflineprofileitemassociation":
                case "monthlyfiscalcalendar":
                case "msdyn_odatav4ds":
                case "msdyn_solutioncomponentdatasource":
                case "msdyn_solutioncomponentsummary":
                case "navigationsetting":
                case "officegraphdocument":
                case "offlinecommanddefinition":
                case "opportunitycompetitors":
                case "opportunityproduct":
                case "organizationui":
                case "orginsightsmetric":
                case "orginsightsnotification":
                case "personaldocumenttemplate":
                case "pluginassembly":
                case "plugintracelog":
                case "plugintype":
                case "plugintypestatistic":
                case "post":
                case "postcomment":
                case "postfollow":
                case "postlike":
                case "principalentitymap":
                case "principalobjectattributeaccess":
                case "privilege":
                case "processstage":
                case "processtrigger":
                case "productassociation":
                case "productpricelevel":
                case "productsalesliterature":
                case "productsubstitute":
                case "publisher":
                case "publisheraddress":
                case "quarterlyfiscalcalendar":
                case "queuemembership":
                case "quotedetail":
                case "recommendationcache":
                case "recommendationmodelmapping":
                case "recommendationmodelversion":
                case "recommendationmodelversionhistory":
                case "recommendeddocument":
                case "recurrencerule":
                case "relationshiprolemap":
                case "report":
                case "reportcategory":
                case "reportentity":
                case "reportlink":
                case "reportvisibility":
                case "resourcegroup":
                case "resourcespec":
                case "ribboncustomization":
                case "role":
                case "roleprivileges":
                case "roletemplateprivileges":
                case "rollupfield":
                case "routingruleitem":
                case "salesliterature":
                case "salesliteratureitem":
                case "salesorderdetail":
                case "savedorginsightsconfiguration":
                case "savedqueryvisualization":
                case "sdkmessage":
                case "sdkmessagefilter":
                case "sdkmessagepair":
                case "sdkmessageprocessingstepimage":
                case "sdkmessageprocessingstepsecureconfig":
                case "sdkmessagerequest":
                case "sdkmessagerequestfield":
                case "sdkmessageresponse":
                case "sdkmessageresponsefield":
                case "semiannualfiscalcalendar":
                case "service":
                case "servicecontractcontacts":
                case "serviceendpoint":
                case "sharepointdata":
                case "sharepointdocument":
                case "site":
                case "sitemap":
                case "slaitem":
                case "slakpiinstance":
                case "socialinsightsconfiguration":
                case "solution":
                case "solutioncomponent":
                case "subject":
                case "subscriptionmanuallytrackedobject":
                case "subscriptiontrackingdeletedobject":
                case "suggestioncardtemplate":
                case "syncerror":
                case "systemform":
                case "systemuserlicenses":
                case "systemuserprofiles":
                case "systemuserroles":
                case "systemusersyncmappingprofiles":
                case "team":
                case "teammembership":
                case "teamprofiles":
                case "teamroles":
                case "teamsyncattributemappingprofiles":
                case "teamtemplate":
                case "template":
                case "territory":
                case "textanalyticsentitymapping":
                case "timezonedefinition":
                case "timezonelocalizedname":
                case "timezonerule":
                case "topic":
                case "topichistory":
                case "topicmodelconfiguration":
                case "topicmodelexecutionhistory":
                case "tracelog":
                case "transformationparametermapping":
                case "uom":
                case "userentityinstancedata":
                case "userentityuisettings":
                case "userform":
                case "usermapping":
                case "userqueryvisualization":
                case "usersettings":
                case "webresource":
                case "workflowdependency":
                case "workflowlog":

                    #endregion Default CRM Entites with no active flag

                    ActiveAttribute = ActiveAttributeType.None;
                    break;

                default:
                    if (logicalName.Length > 4 && logicalName[3] == '_')
                    {
                        var prefix = logicalName.ToLower().Substring(0, 3);
                        if (logicalName.ToLower().Split(new [] { prefix }, StringSplitOptions.None).Length >= 3 || logicalName.ToLower().EndsWith("_association"))
                        {
                            // N:N Joins or association entities do not contain active flags
                            ActiveAttribute = ActiveAttributeType.None;
                            break;
                        }
                    }
                    SetStateAttributesAndValue(logicalName);
                    break;
            }
        }

        /// <summary>
        /// Sets the state attributes and value.
        /// </summary>
        /// <param name="logicalName">Name of the logical.</param>
        protected void SetStateAttributesAndValue(string logicalName)
        {
            ActiveAttribute = ActiveAttributeType.StateCode;
            AttributeName = "statecode";

            switch (logicalName)
            {
                // Entities with a Canceled State
                case "activitypointer":
                case "appointment":
                case "bulkoperation":
                case "campaignactivity":
                case "contractdetail":
                case "email":
                case "fax":
                case "letter":
                case "orderclose":
                case "phonecall":
                case "quoteclose":
                case "recurringappointmentmaster":
                case "serviceappointment":
                case "taskstate":
                    NotActiveState = 2;
                    break;

                case "duplicaterule": // don't ask me why, but this one is flipped
                    ActiveState = 1;
                    NotActiveState = 0;
                    break;

                // Entities with states that can't be grouped into separate all inclusive active and inactive states
                case "asyncoperation":
                case "bulkdeleteoperation":
                case "contract":
                case "lead":
                case "opportunity":
                case "processsession":
                case "quote":
                case "sdkmessageprocessingstep":
                case "workflow":
                    ActiveAttribute = ActiveAttributeType.None;
                    break;

                default:
                    if (IsJoinEntity(logicalName))
                    {
                        ActiveAttribute = ActiveAttributeType.None;
                    }
                    else
                    {
                        ActiveState = 0;
                        NotActiveState = 1;
                    }
                    break;
            }
        }

        /// <summary>
        /// Determines whether [is join entity] [the specified logical name].
        /// </summary>
        /// <param name="logicalName">Name of the logical.</param>
        /// <returns></returns>
        private bool IsJoinEntity(string logicalName)
        {
            // Entities of the type new_Foo_Bar are usually Join Entities that don't have a state
            return logicalName.Split('_').Length >= 3;
        }

        /// <summary>
        /// Determines whether the specified service is active.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <returns></returns>
        public static bool? IsActive(IOrganizationService service, Guid entityId)
        {
            var info = new ActivePropertyInfo<T>();
            var entity = service.GetEntity<T>(entityId, new ColumnSet(info.AttributeName));
            return IsActive(info, entity);
        }

        /// <summary>
        /// Determines whether the specified information is active.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">ActivePropertyInfo defines Attribute StateCode, but neither ActiveState or NotActiveState is popualted</exception>
        /// <exception cref="EnumCaseUndefinedException{ActiveAttributeType}"></exception>
        protected static bool? IsActive(ActivePropertyInfo<T> info, T entity)
        {
            bool? active;
            switch (info.ActiveAttribute)
            {
                case ActiveAttributeType.None:
                    // Unable to determine
                    active = null;
                    break;
                case ActiveAttributeType.IsDisabled:
                    active = !entity.GetAttributeValue<bool>(info.AttributeName);
                    break;
                case ActiveAttributeType.StateCode:
                    var state = entity.GetAttributeValue<OptionSetValue>(info.AttributeName).Value;
                    if (info.ActiveState.HasValue)
                    {
                        active = state == info.ActiveState;
                    }
                    else if (info.NotActiveState.HasValue)
                    {
                        active = state != info.NotActiveState;
                    }
                    else
                    {
                        throw new Exception("ActivePropertyInfo defines Attribute StateCode, but neither ActiveState or NotActiveState is popualted");
                    }
                    break;
                default:
                    throw new EnumCaseUndefinedException<ActiveAttributeType>(info.ActiveAttribute);
            }

            return active;
        }
    }
}
