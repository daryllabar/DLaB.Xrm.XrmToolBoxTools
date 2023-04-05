using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Xrm.Sdk;
#if DLAB_UNROOT_COMMON_NAMESPACE
using DLaB.Common;
#else
using Source.DLaB.Common;
#endif

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
{
    /// <summary>
    /// Utility class to get an Entity Type from a name, and visa-versa, and for determining Entity Id Attribute Name
    /// </summary>
#if !DLAB_XRM_DEBUG
    [DebuggerNonUserCode]
#endif
    public class EntityHelper
    {
        /// <summary>
        /// Gets the entity logical name for the given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetEntityLogicalName<T>() where T : Entity
        {
            return GetEntityLogicalName(typeof(T));
        }

        /// <summary>
        /// Gets the entity logical name for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Type  + type.FullName +  does not contain an EntityLogicalName Field</exception>
        public static string GetEntityLogicalName(Type type)
        {
            if (type.IsGenericParameter && type.BaseType != null)
            {
                // Handle SomeType<TEntity> where TEntity : Entity
                return GetEntityLogicalName(type.BaseType);
            }

            var att = type.GetCustomAttributes<Microsoft.Xrm.Sdk.Client.EntityLogicalNameAttribute>(true).FirstOrDefault();
            if (att != null)
            {
                return att.LogicalName;
            }

            var field = type.GetField("EntityLogicalName");
            if (field != null)
            {
                return (string) field.GetValue(null);
            }
            if (type == typeof(Entity))
            {
                return "entity";
            }
            throw new Exception("Type " + type.FullName + " does not contain an EntityLogicalName Field");
        }

        #region Determine Type

        /// <summary>
        /// Gets the type of the given entity, using the OrganizationServiceContext to determine the assembly to look for the Entity Type in.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityLogicalName">Name of the entity logical.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Must pass in a derived type from Microsoft.Xrm.Sdk.Client.OrganizationServiceContext</exception>
        public static Type GetType<T>(string entityLogicalName) where T : Microsoft.Xrm.Sdk.Client.OrganizationServiceContext
        {
            var contextType = typeof(T);
            if (contextType.Name == "OrganizationServiceContext")
            {
                throw new Exception("Must pass in a derived type from Microsoft.Xrm.Sdk.Client.OrganizationServiceContext");
            }

            return GetType(contextType.Assembly, contextType.Namespace, entityLogicalName);
        }

        private static readonly ConcurrentDictionary<string, Dictionary<string, Type>> Cache = new ConcurrentDictionary<string, Dictionary<String, Type>>();
        /// <summary>
        /// Gets the type for the given entity logical name.
        /// </summary>
        /// <param name="earlyBoundAssembly">The early bound assembly.</param>
        /// <param name="namespace">The namespace.</param>
        /// <param name="entityLogicalName">Name of the entity logical.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static Type GetType(Assembly earlyBoundAssembly, string @namespace, string entityLogicalName)
        {
            var key = "LogicalNameToEntityMapping|" + earlyBoundAssembly.FullName + "|" + @namespace;
            var mappings = Cache.GetOrAdd(key, k =>
            {
                var entityType = typeof(Entity);
                return earlyBoundAssembly.GetTypes().
                    Where(t => t.Namespace == @namespace && entityType.IsAssignableFrom(t)).
                    Select(t => new
                    {
                        Key = GetEntityLogicalName(t),
                        Value = t
                    }).
                    ToDictionary(v => v.Key, v => v.Value);
            });

            if (!mappings.TryGetValue(entityLogicalName, out Type otherType))
            {
                throw new Exception($"Unable to find a Type in assembly \"{earlyBoundAssembly.FullName}\", namespace \"{@namespace}\", with a logical name of \"{entityLogicalName}\"");
            }

            return otherType;
        }

        /// <summary>
        /// Determines whether entity defined by the logical name is defined as an early bound type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityLogicalName">Name of the entity logical.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Must pass in a derived type from Microsoft.Xrm.Sdk.Client.OrganizationServiceContext</exception>
        public static bool IsTypeDefined<T>(string entityLogicalName) where T : Microsoft.Xrm.Sdk.Client.OrganizationServiceContext
        {
            var contextType = typeof(T);
            if (contextType.Name == "OrganizationServiceContext")
            {
                throw new Exception("Must pass in a derived type from Microsoft.Xrm.Sdk.Client.OrganizationServiceContext");
            }

            return IsTypeDefined(contextType.Assembly, contextType.Namespace, entityLogicalName);
        }

        /// <summary>
        /// Determines whether entity defined by the logical name is defined as an early bound type.
        /// </summary>
        /// <param name="earlyBoundAssembly">The early bound assembly.</param>
        /// <param name="namespace">The namespace.</param>
        /// <param name="entityLogicalName">Name of the entity logical.</param>
        /// <returns></returns>
        public static bool IsTypeDefined(Assembly earlyBoundAssembly, string @namespace, string entityLogicalName)
        {
            var key = "LogicalNameToEntityMapping|" + earlyBoundAssembly.FullName + "|" + @namespace;
            var mappings = Cache.GetOrAdd(key, k =>
            {
                var entityType = typeof(Entity);
                return earlyBoundAssembly.GetTypes().
                    Where(t => t.Namespace == @namespace && entityType.IsAssignableFrom(t)).
                    Select(t => new
                    {
                        Key = GetEntityLogicalName(t),
                        Value = t
                    }).
                    ToDictionary(v => v.Key, v => v.Value);
            });

            return mappings.ContainsKey(entityLogicalName);
        }

        #endregion Determine Type

        #region Determine Id Attribute Name

        /// <summary>
        /// Returns the attribute name of the id of the entity using late bound approach
        /// </summary>
        /// <param name="logicalName"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string GetIdAttributeName(string logicalName, IEntityHelperConfig config = null)
        {
            return GetIrregularIdAttributeName(logicalName, config) ?? logicalName + "id";
        }

        /// <summary>
        /// Returns the attribute name of the id of the entity
        /// </summary>
        /// <typeparam name="T">Entity Type to use Reflection to lookup the entity logical name for</typeparam>
        /// <returns></returns>
        public static string GetIdAttributeName<T>() where T : Entity
        {
            return GetIdAttributeName(typeof(T));
        }

        /// <summary>
        /// Returns the attribute name of the id of the entity
        /// </summary>
        /// <param name="type">The Type of the Entity.</param>
        /// <returns></returns>
        public static string GetIdAttributeName(Type type)
        {
            if (type.IsGenericParameter && type.BaseType != null)
            {
                // Handle SomeType<TEntity> where TEntity : Entity
                return GetIdAttributeName(type.BaseType);
            }

            var idName = type.GetProperty("Id", BindingFlags.Instance | BindingFlags.Public)
                          ?.GetCustomAttribute<AttributeLogicalNameAttribute>()?.LogicalName;
            if (!string.IsNullOrWhiteSpace(idName))
            {
                return idName;
            }

            var field = type.GetField("EntityLogicalName");
            if (field != null)
            {
                return (string)field.GetValue(null);
            }
            if (type == typeof(Entity))
            {
                return "entity";
            }

            //[Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("activityid")]
            //typeof(T).GetProperty("Id")
            return GetIdAttributeName(GetEntityLogicalName(type));
        }

        /// <summary>
        /// Returns the attribute name of the id of the entity if it doesn't follow the standard (logicalName + id) rule, or null
        /// </summary>
        /// <param name="logicalName"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string GetIrregularIdAttributeName(string logicalName, IEntityHelperConfig config = null)
        {
            string name;

            switch (logicalName)
            {
                case "activitypointer":
                case "appointment":
                case "bulkoperation":
                case "campaignactivity":
                case "campaignresponse":
                case "email":
                case "fax":
                case "incidentresolution":
                case "letter":
                case "opportunityclose":
                case "phonecall":
                case "quoteclose":
                case "recurringappointmentmaster":
                case "serviceappointment":
                case "task":
                    name = "activityid";
                    break;
                default:
                    name = (config ?? DLaBXrmConfig.EntityHelperConfig).GetIrregularIdAttributeName(logicalName);
                    break;
            }

            return name;
        }

        /// <summary>
        /// Returns the attribute name of the id of the entity if it doesn't follow the standard (logicalName + id) rule, or null
        /// </summary>
        /// <typeparam name="T">Entity Type to use Reflection to lookup the entity logical name for</typeparam>
        /// <param name="config">Interface for handling irregular primary attribute names.</param>
        /// <returns></returns>
        public static string GetIrregularIdAttributeName<T>(IEntityHelperConfig config = null) where T : Entity
        {
            return GetIrregularIdAttributeName(GetEntityLogicalName<T>(), config);
        }

        #endregion Determine Id Attribute Name

        #region Determine Entity Attribute Name Name

        /// <summary>
        /// Gets the Primary Field (name) info. 
        /// </summary>
        /// <param name="logicalName">Name of the logical.</param>
        /// <param name="config">Interface for handling irregular primary attribute names.</param>
        /// <returns></returns>
        public static PrimaryFieldInfo GetPrimaryFieldInfo(string logicalName, IEntityHelperConfig config = null)
        {
            var info = new PrimaryFieldInfo();
                switch (logicalName)
                {
                    case "businessunitnewsarticle":
                        info.AttributeName = "articletitle";
                        info.MaximumLength = 300;
                        break;

                    case "transactioncurrency":
                        info.AttributeName = "currencyname";
                        break;

                    case "customerrelationship":
                        info.AttributeName = "customerroleidname";
                        info.ReadOnly = true;
                        info.IsAttributeOf = true;
                        break;

                    case "importjob":
                        info.AttributeName = "data";
                        info.MaximumLength = 1073741823;
                        break;

                    case "transformationparametermapping":
                        info.AttributeName = "data";
                        info.MaximumLength = 500;
                        break;

                    case "activitymimeattachment":
                        info.AttributeName = "fileName";
                        info.MaximumLength = 255;
                        break;

                    case "contact":
                    case "systemuser":
                    case "lead":
                        info.AttributeName = "fullname";
                        info.BaseAttributes.Add("firstname");
                        info.BaseAttributes.Add("lastname");
                        if (logicalName == "lead")
                        {
                            info.BaseAttributes.Add("companyname"); 
                        }
                        info.ReadOnly = true;
                        break;

                    case "solution":
                    case "publisher":
                        info.AttributeName = "friendlyname";
                        break;

                    case "account":
                    case "asyncoperation":
                    case "bulkdeleteoperation":
                    case "businessunit":
                    case "calendar":
                    case "calendarrule":
                    case "campaign":
                    case "competitor":
                    case "connection":
                    case "connectionrole":
                    case "constraintbasedgroup":
                    case "contracttemplate":
                    case "convertrule":
                    case "convertruleitem":
                    case "customeraddress":
                    case "discounttype":
                    case "duplicaterule":
                    case "emailserverprofile":
                    case "entitlement":
                    case "entitlementchannel":
                    case "entitlementtemplate":
                    case "entitlementtemplatechannel":
                    case "equipment":
                    case "fieldsecurityprofile":
                    case "goalrollupquery":
                    case "import":
                    case "importfile":
                    case "importmap":
                    case "invoice":
                    case "mailbox":
                    case "mailmergetemplate":
                    case "metric":
                    case "opportunity":
                    case "organization":
                    case "pluginassembly":
                    case "plugintype":
                    case "pricelevel":
                    case "privilege":
                    case "processsession":
                    case "product":
                    case "publisheraddress":
                    case "queue":
                    case "quote":
                    case "relationshiprole":
                    case "report":
                    case "resource":
                    case "resourcegroup":
                    case "resourcespec":
                    case "role":
                    case "routingrule":
                    case "routingruleitem":
                    case "salesliterature":
                    case "salesorder":
                    case "savedquery":
                    case "savedqueryvisualization":
                    case "sdkmessage":
                    case "sdkmessageprocessingstep":
                    case "sdkmessageprocessingstepimage":
                    case "service":
                    case "serviceendpoint":
                    case "sharepointdocumentlocation":
                    case "sharepointsite":
                    case "site":
                    case "sla":
                    case "systemform":
                    case "team":
                    case "territory":
                    case "uom":
                    case "uomschedule":
                    case "userform":
                    case "userquery":
                    case "userqueryvisualization":
                    case "webresource":
                        info.AttributeName = "name";
                        break;

                    case "list":
                        info.AttributeName = "listname";
                        info.MaximumLength = 128;
                        break;

                    case "activityparty":
                        info.AttributeName = "partyidname";
                        info.MaximumLength = 400;
                        info.ReadOnly = true;
                        info.IsAttributeOf = true;
                        break;

                    case "invoicedetail":
                    case "opportunityproduct":
                    case "productpricelevel":
                    case "quotedetail":
                    case "salesorderdetail":
                        info.AttributeName = "productidname";
                        info.ReadOnly = true;
                        info.IsAttributeOf = true;
                        break;

                    case "socialprofile":
                        info.AttributeName = "profilename";
                        break;

                    case "postfollow":
                        info.AttributeName = "regardingobjectidname";
                        info.ReadOnly = true;
                        info.IsAttributeOf = true;
                        info.MaximumLength = 4000;
                        break;

                    case "columnmapping":
                        info.AttributeName = "sourceattributename";
                        info.MaximumLength = 160;
                        break;

                    case "processstage":
                        info.AttributeName = "stagename";
                        break;

                    case "activitypointer":
                    case "annotation":
                    case "appointment":
                    case "bulkoperation":
                    case "campaignactivity":
                    case "campaignresponse":
                    case "email":
                    case "fax":
                    case "incidentresolution":
                    case "letter":
                    case "opportunityclose":
                    case "orderclose":
                    case "phonecall":
                    case "quoteclose":
                    case "recurringappointmentmaster":
                    case "serviceappointment":
                    case "socialactivity":
                    case "task":
                        info.AttributeName = "subject";
                        info.MaximumLength = 200;
                        break;

                    case "teamtemplate":
                        info.AttributeName = "teamtemplatename";
                        break;

                    case "post":
                    case "postcomment":
                    case "tracelog":
                        info.AttributeName = "text";
                        info.MaximumLength = 1000;
                        break;

                    case "contract":
                    case "contractdetail":
                    case "goal":
                    case "incident":
                    case "kbarticle":
                    case "kbarticlecomment":
                    case "kbarticletemplate":
                    case "queueitem":
                    case "salesliteratureitem":
                    case "subject":
                    case "template":
                        info.AttributeName = "title";
                        break;

                    default:
                        if (logicalName.Contains('_'))
                        {
                            info.AttributeName = logicalName.SubstringByString(0, "_") + "_name";
                        }
                        else
                        {
                            info.AttributeName = null;
                        }

                        info = (config ?? DLaBXrmConfig.EntityHelperConfig).GetIrregularPrimaryFieldInfo(logicalName, info) ?? info;
                        break;
                }

            return info;
        }

        #endregion Determine Entity Attribute Name Name

        #region Determine Parent Attribute Name

        /// <summary>
        /// Gets the attribute name of the first attribute with the given Parent Type.
        /// If any of the possible attributes are CustomerId, customerId will be returned.
        /// </summary>
        /// <param name="childType">Type of the child.</param>
        /// <param name="parentType">Type of the parent.</param>
        /// <returns></returns>
        public static string GetParentEntityAttributeName(Type childType, Type parentType)
        {
            var possibleProperties = childType.GetProperties().Where(p => p.PropertyType == parentType).ToList();
            var customerId = possibleProperties.Count <= 1
                ? null
                : possibleProperties.FirstOrDefault(p => p.GetCustomAttributes(typeof(AttributeLogicalNameAttribute))
                                                          .Cast<AttributeLogicalNameAttribute>()
                                                          .FirstOrDefault(a => a.LogicalName == "customerid") != null);
            if (customerId != null)
            {
                return customerId.GetAttributeLogicalName();
            }
            if (possibleProperties.Count == 0)
            {
                throw new Exception($"No Property found for type {childType.FullName}, of type {parentType.FullName}.");
            }
            return possibleProperties.First().GetAttributeLogicalName();
        }

        /// <summary>
        /// Gets the attribute name of the first attribute with the given parent type.
        /// If any of the possible attributes are CustomerId, customerId will be returned.
        /// </summary>
        /// <typeparam name="TChild">The type of the child.</typeparam>
        /// <typeparam name="TParent">The type of the parent.</typeparam>
        /// <returns></returns>
        public static string GetParentEntityAttributeName<TChild,TParent>() where TChild : Entity where TParent : Entity
        {
            return GetParentEntityAttributeName(typeof (TChild), typeof (TParent));
        }

        #endregion Determine Parent Attribute Name
    }
}
