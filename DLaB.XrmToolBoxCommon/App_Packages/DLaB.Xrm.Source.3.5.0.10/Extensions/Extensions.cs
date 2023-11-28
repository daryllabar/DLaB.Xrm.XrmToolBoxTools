using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
#if !DLAB_XRM_DEBUG
using System.Diagnostics;
#endif
using System.Globalization;
using System.Linq.Expressions;
using System.Xml;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
#if DLAB_UNROOT_COMMON_NAMESPACE
using DLaB.Common;
#else
using Source.DLaB.Common;
#endif
using System.Collections.Concurrent;
using System.Runtime.Caching;
#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
using DLaB.Xrm.Exceptions;

namespace DLaB.Xrm
#else
using Source.DLaB.Xrm.Exceptions;

namespace Source.DLaB.Xrm
#endif
{
    /// <summary>
    /// Extension class for Xrm
    /// </summary>

#if !DLAB_XRM_DEBUG
    [DebuggerNonUserCode]
#endif
    public static partial class Extensions
    {
#region AttributeMetadata
        /// <summary>
        /// Determines whether the attribute is a local option set.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns></returns>
        public static bool IsLocalOptionSetAttribute(this AttributeMetadata attribute)
        {
            if (attribute.AttributeType != AttributeTypeCode.Picklist
                && attribute.AttributeType != AttributeTypeCode.State
                && attribute.AttributeType != AttributeTypeCode.Status)
            {
                return false;
            }

            if (attribute is EnumAttributeMetadata picklist)
            {
                return picklist.OptionSet.IsGlobal.HasValue && !picklist.OptionSet.IsGlobal.Value;
            }
            return false;
        }

#endregion AttributeMetadata

#region ColumnSet

        /// <summary>
        /// Allows for adding column names in an early bound manner:
        /// columnSet.AddColumns&lt;Opportunity&gt;(i => new { i.OpportunityId, i.SalesStage, i.SalesStageCode });
        /// </summary>
        /// <typeparam name="T">An Entity Type</typeparam>
        /// <param name="columnSet">The ColumnSet</param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <returns></returns>
        public static ColumnSet AddColumns<T>(this ColumnSet columnSet, Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
        {
            columnSet.AddColumns(GetAttributeNamesArray(anonymousTypeInitializer));
            return columnSet;
        }

#endregion ColumnSet

#region DataCollection<string,Entity>

        /// <summary>
        /// If the imageName is populated, then if images collection contains the given imageName Key, the Value is cast to the Entity type T, else null is returned
        /// If the imageName is not populated but the default name is, then the defaultName is searched for in the images collection and if it has a value, it is cast to the Entity type T.
        /// Else, the first non-null value in the images collection is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="images"></param>
        /// <param name="imageName">The name to Search For</param>
        /// <param name="defaultName">The Default Name to use</param>
        /// <returns></returns>
        public static T GetEntity<T>(this DataCollection<string, Entity> images, string imageName = null, string defaultName = null) where T : Entity
        {
            if (images.Count == 0)
            {
                return null;
            }

            if (images.Count == 1 && imageName == null)
            {
                return images.Values.FirstOrDefault().AsEntity<T>();
            }

            Entity entity;

            if (imageName != null)
            {
                return images.TryGetValue(imageName, out entity) ? entity.AsEntity<T>() : null;
            }

            return defaultName != null && images.TryGetValue(defaultName, out entity)
                ? entity.AsEntity<T>()
                : images.Values.FirstOrDefault(v => v != null).AsEntity<T>();
        }

#endregion DataCollection<string,Entity>

#region Entity


        /// <summary>
        /// Checks to see if the entity is already of the given type.
        /// If it is, it just returns the entity cast as the type, else ToEntity is called.
        ///  the entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static T AsEntity<T>(this Entity entity) where T : Entity
        {
            var tEntity = entity as T;
            return tEntity ?? entity.ToEntity<T>();
        }

#region AssertContainsAllNonNull

        /// <summary>
        /// Checks to see if the Entity Contains the attribute names, and the value is not null.  Any missing/null attributes will result in an exception, listing the missing attributes.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="attributeNames"></param>
        /// <returns></returns>
        public static void AssertContainsAllNonNull(this Entity entity, params string[] attributeNames)
        {
            if (entity.ContainsAllNonNull(out List<string> missingAttributes, attributeNames)) { return; }

            if (missingAttributes.Count == 1)
            {
                throw new MissingAttributeException("Missing Required Field: " + missingAttributes[0]);
            }

            throw new MissingAttributeException("Missing Required Fields: " + missingAttributes.ToCsv());
        }

        /// <summary>
        /// Checks to see if the Entity Contains the attribute names, and the value is not null.  Any missing/null attributes will result in an exception, listing the missing attributes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="anonymousTypeInitializer"></param>
        /// <returns></returns>
        public static void AssertContainsAllNonNull<T>(this T entity, Expression<Func<T, object>> anonymousTypeInitializer)
            where T : Entity
        {
            AssertContainsAllNonNull(entity, GetAttributeNamesArray(anonymousTypeInitializer));
        }

#endregion AssertContainsAllNonNull

        /// <summary>
        /// Clears the id of an entity so it can be saved as a new entity
        /// </summary>
        /// <param name="entity"></param>
        public static void ClearId(this Entity entity)
        {
            entity.Id = Guid.Empty;
            entity.Attributes.Remove(EntityHelper.GetIdAttributeName(entity.LogicalName));
        }

        /// <summary>
        /// Adds the attributes formatted values, and key attributes from the given entity if they do not exist in the current
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="baseEntity"></param>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static T CoalesceEntity<T>(this T baseEntity, Entity entity) where T : Entity
        {
            if (entity == null)
            {
                return baseEntity;
            }

            if (baseEntity.LogicalName == null)
            {
                baseEntity.LogicalName = entity.LogicalName;
            }

            if (baseEntity.Id == Guid.Empty && baseEntity.LogicalName == entity.LogicalName)
            {
                baseEntity.Id = entity.Id;
                baseEntity.LogicalName = entity.LogicalName;
            }

            foreach (var attribute in entity.Attributes.Where(a => !baseEntity.Contains(a.Key)))
            {
                baseEntity[attribute.Key] = attribute.Value;
            }

            foreach (var formattedAtt in entity.FormattedValues.Where(a =>
                !baseEntity.FormattedValues.Contains(a.Key)
                && baseEntity.GetAttributeValue<object>(a.Key)?.Equals(entity.GetAttributeValue<object>(a.Key)) == true))
            {
                baseEntity.FormattedValues[formattedAtt.Key] = formattedAtt.Value;
            }
#if !PRE_KEYATTRIBUTE
            foreach (var keyAtt in entity.KeyAttributes.Where(k => !baseEntity.KeyAttributes.Contains(k.Key)))
            {
                baseEntity.KeyAttributes[keyAtt.Key] = keyAtt.Value;
            }
#endif

            return baseEntity;
        }

#region ContainsAllNonNull

        /// <summary>
        /// Checks to see if the Entity Contains the attribute names, and the value is not null
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="attributeNames"></param>
        /// <returns></returns>
        public static bool ContainsAllNonNull(this Entity entity, params string[] attributeNames)
        {
            return attributeNames.All(name => entity.Contains(name) && entity[name] != null);
        }

        /// <summary>
        /// Checks to see if the Entity Contains the attribute names, and the value is not null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="anonymousTypeInitializer"></param>
        /// <returns></returns>
        public static bool ContainsAllNonNull<T>(this T entity, Expression<Func<T, object>> anonymousTypeInitializer)
            where T : Entity
        {
            return ContainsAllNonNull(entity, GetAttributeNamesArray(anonymousTypeInitializer));
        }

        /// <summary>
        /// Checks to see if the Entity Contains the attribute names, and the value is not null
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="nullAttributeNames"></param>
        /// <param name="attributeNames"></param>
        /// <returns></returns>
        public static bool ContainsAllNonNull(this Entity entity, out List<string> nullAttributeNames, params string[] attributeNames)
        {
            nullAttributeNames = attributeNames.Where(name => !entity.Contains(name) || entity[name] == null).ToList();

            return !nullAttributeNames.Any();
        }

        /// <summary>
        /// Checks to see if the Entity Contains the attribute names, and the value is not null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="nullAttributeNames"></param>
        /// <param name="anonymousTypeInitializer"></param>
        /// <returns></returns>
        public static bool ContainsAllNonNull<T>(this T entity, out List<string> nullAttributeNames, Expression<Func<T, object>> anonymousTypeInitializer)
            where T : Entity
        {
            return ContainsAllNonNull(entity, out nullAttributeNames, GetAttributeNamesArray(anonymousTypeInitializer));
        }

#endregion ContainsAllNonNull

#region ContainsAnyNonNull

        /// <summary>
        /// Checks to see if the Entity Contains any of the attribute names, and the value is not null
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="attributeNames"></param>
        /// <returns></returns>
        public static bool ContainsAnyNonNull(this Entity entity, params string[] attributeNames)
        {
            return attributeNames.Any(name => entity.Contains(name) && entity[name] != null);
        }

        /// <summary>
        /// Checks to see if the Entity Contains any of the attribute names, and the value is not null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="anonymousTypeInitializer"></param>
        /// <returns></returns>
        public static bool ContainsAnyNonNull<T>(this T entity, Expression<Func<T, object>> anonymousTypeInitializer)
            where T : Entity
        {
            return ContainsAnyNonNull(entity, GetAttributeNamesArray(anonymousTypeInitializer));
        }

#endregion ContainsAnyNonNull

        /// <summary>
        /// Gets the url of the form.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public static string GetFormUrl(this Entity entity, Uri uri)
        {
            string parameters = $"etn={entity.LogicalName}&pagetype=entityrecord&id={entity.Id:D}";
            string url;
            if (uri == null)
            {
                url = @"http://localhost/main.aspx?" + parameters;
            }
            else
            {
                var host = uri.Host;
                var orgName = uri.Segments.Length > 1 ? uri.Segments[1] : string.Empty;
                // Handle Online Url
                if (host.EndsWith("dynamics.com") && host.Contains(".api."))
                {
                    host = host.Remove(host.LastIndexOf(".api.", StringComparison.Ordinal), 4);
                    orgName = string.Empty;
                }
                url = $@"http://{host}/{orgName}main.aspx?{parameters}";
            }
            return url;
        }

        /// <summary>
        /// Gets the formatted attribute value or null.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="attributeLogicalName">Name of the attribute logical.</param>
        /// <returns></returns>
        public static string GetFormattedAttributeValueOrNull(this Entity entity, string attributeLogicalName)
        {
            if (string.IsNullOrWhiteSpace(attributeLogicalName))
            {
                throw new ArgumentNullException(nameof(attributeLogicalName));
            }
            // Check for unaliased value first
            if (entity.FormattedValues.Contains(attributeLogicalName))
            {
                return entity.FormattedValues[attributeLogicalName];
            }
            // Check for Aliased values second
            foreach (var attribute in entity.FormattedValues.Keys)
            {
                var attributeName = attribute;
                var aliasedEntityName = SplitAliasedAttributeEntityName(ref attributeName);
                if (!String.IsNullOrWhiteSpace(aliasedEntityName) && attributeName == attributeLogicalName)
                {
                    return entity.FormattedValues[attribute];
                }
            }

            return null;
        }

        /// <summary>
        /// Returns Local Option Set text Only.  Not sure what else it does...
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="attributeLogicalName"></param>
        /// <returns></returns>
        public static string GetFormattedAttributeValue(this Entity entity, string attributeLogicalName)
        {
            var value = entity.GetFormattedAttributeValueOrNull(attributeLogicalName);

            if (value == null)
            {
                throw new Exception("Formatted attribute value with attribute " + attributeLogicalName +
                " was not found!  Only these attributes were found: " + entity.Attributes.Keys.ToCsv());
            }

            return value;
        }

        /// <summary>
        /// Gets the name of the id attribute logical name.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static string GetIdAttributeName(this Entity entity)
        {
            return EntityHelper.GetIdAttributeName(entity.LogicalName);
        }

        /// <summary>
        /// Returns the Id of the entity or Guid.Empty if it is null"
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Guid GetIdOrDefault(this Entity entity)
        {
            return entity?.Id ?? Guid.Empty;
        }

        /// <summary>
        /// Returns the name of the Attribute that contains the default name of the entity
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static string GetNameAttribute(this Entity entity)
        {
            var value = string.Empty;
            var index = entity.LogicalName.IndexOf("_", StringComparison.Ordinal);
            var prefix = index > 0 ? entity.LogicalName.Substring(0, index) : string.Empty;
            if (entity.Contains(prefix + "_name"))
            {
                value = prefix + "_name";
            }
            else if (entity.Contains("fullname"))
            {
                value = "fullname";
            }
            else if (entity.Contains("name"))
            {
                value = "name";
            }
            else if (entity.LogicalName.Contains('_'))
            {
                // Get prefix of entity
                prefix = entity.LogicalName.Substring(0, entity.LogicalName.IndexOf('_'));
                if (entity.Contains(prefix + "_name"))
                {
                    value = prefix + "_name";
                }
            }

            return value;
        }

        /// <summary>
        /// Returns the Name and Id of the Current Entity in this format "Name (id)"
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static string GetNameId(this Entity entity)
        {
            var value = string.Empty;
            var nameAttribute = entity.GetNameAttribute();
            if (nameAttribute != string.Empty && entity.Contains(nameAttribute))
            {
                value = entity.Attributes[nameAttribute] as string ?? "NULL";
            }

            return value + $" ({entity.Id})";
        }

        /// <summary>
        /// Returns the value for the attribute, or the default value for the type if it does not exist in the entity.
        /// </summary>
        /// <typeparam name="T">The attribute type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public static T GetOrDefault<T>(this Entity entity, string attributeName)
        {
            return entity.GetOrDefault(attributeName, default(T));
        }

        /// <summary>
        /// Returns the value for the attribute, or the default value for the type if it does not exist in the entity.
        /// </summary>
        /// <typeparam name="T">The attribute type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="defaultValue">The value to use if the entity does not contain the attribute.</param>
        /// <returns></returns>
        public static T GetOrDefault<T>(this Entity entity, string attributeName, T defaultValue)
        {
            T value;
            if (entity.Contains(attributeName))
            {
                value = (T)entity[attributeName];
            }
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Calls the Entity.ToEntity() method generically based on the logical name.  Useful for determining if an early bound entity implements a given type.
        /// </summary>
        /// <typeparam name="TEntityContext">The Context to use to determine the entity type.</typeparam>
        /// <param name="entity">The Entity</param>
        /// <returns></returns>
        public static Entity ToEarlyBoundEntity<TEntityContext>(this Entity entity) where TEntityContext : OrganizationServiceContext
        {
            return entity.ToEarlyBoundEntity(EntityHelper.GetType<TEntityContext>(entity.LogicalName));
        }

        /// <summary>
        /// Calls the Entity.ToEntity() method generically based on the logical name.  Useful for determining if an early bound entity implements a given type.
        /// </summary>
        /// <param name="entity">The Entity</param>
        /// <param name="earlyBoundAssembly">The assembly to search</param>
        /// <param name="namespace">The namespace to search</param>
        /// <returns></returns>
        public static Entity ToEarlyBoundEntity(this Entity entity, Assembly earlyBoundAssembly, string @namespace)
        {
            return entity.ToEarlyBoundEntity(EntityHelper.GetType(earlyBoundAssembly, @namespace, entity.LogicalName));
        }

        /// <summary>
        /// Calls the Entity.ToEntity() method generically based on the logical name.  Useful for determining if an early bound entity implements a given type.
        /// </summary>
        /// <param name="entity">The Entity</param>
        /// <param name="earlyBoundType">The Type to call the ToEntity generic method with.</param>
        /// <returns></returns>
        public static Entity ToEarlyBoundEntity(this Entity entity, Type earlyBoundType)
        {
            if (entity.GetType() == earlyBoundType)
            {
                return entity;
            }
            var toEntity = typeof(Entity).GetMethod(nameof(Entity.ToEntity));
            return (Entity) toEntity?.MakeGenericMethod(earlyBoundType).Invoke(entity, null);
        }

        /// <summary>
        /// Attempts to cast the given entity to specified interface by first converting it to it's early bound entity type.
        /// </summary>
        /// <typeparam name="TInterface">The Interface to cast the early bound entity to</typeparam>
        /// <typeparam name="TEntityContext">The EntityContext To Use to lookup the early bound type.</typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static TInterface ToEntityInterface<TInterface, TEntityContext>(this Entity entity) where TEntityContext : OrganizationServiceContext
        {
            return ((object)entity) is TInterface type
                ? type
                : entity.ToEntityInterface<TInterface>(EntityHelper.GetType<TEntityContext>(entity.LogicalName));
        }

        /// <summary>
        /// Attempts to cast the given entity to specified interface by first converting it to it's early bound entity type.
        /// </summary>
        /// <typeparam name="TInterface">The Interface to cast the early bound entity to</typeparam>
        /// <param name="earlyBoundAssembly">The early bound assembly.</param>
        /// <param name="namespace">The namespace.</param>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static TInterface ToEntityInterface<TInterface>(this Entity entity, Assembly earlyBoundAssembly, string @namespace)
        {
            return ((object)entity) is TInterface type
                ? type
                : entity.ToEntityInterface<TInterface>(EntityHelper.GetType(earlyBoundAssembly, @namespace, entity.LogicalName));
        }

        private static TInterface ToEntityInterface<TInterface>(this Entity entity, Type earlyBoundType)
        {
            var typedEntity = ToEarlyBoundEntity(entity, earlyBoundType);
            if (((object)typedEntity) is TInterface type) {
                return type;
            }
            throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture, "Cannot cast entity {0} to {1}!", earlyBoundType.FullName, typeof(TInterface).FullName));
        }

        /// <summary>
        /// Creates the EntityReference from Entity, settings it's name
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static EntityReference ToEntityReference(this Entity entity, string name)
        {
            var reference = entity.ToEntityReference();
            reference.Name = name;
            return reference;
        }

        /// <summary>
        /// Creates an array of attribute names array from an Anonymous Type Initializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonymousTypeInitializer">The anonymous type initializer.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">lambda must return an object initializer</exception>
        public static string[] GetAttributeNamesArray<T>(this Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
        {
            var initializer = anonymousTypeInitializer.Body as NewExpression;
            if (initializer?.Members == null)
            {
                throw new ArgumentException("lambda must return an object initializer");
            }

            // Search for and replace any occurrence of Id with the actual Entity's Id
            return initializer.Members.Select(GetLogicalAttributeName<T>).ToArray();
        }

        /// <summary>
        /// Normally just returns the name of the property, in lowercase.  But Id must be looked up via reflection.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static string GetLogicalAttributeName<T>(MemberInfo property) where T : Entity
        {
            var name = property.Name.ToLower();
            if (name == "id"
                || name.Substring(name.Length - 1) == "1" && name.StartsWith(typeof(T).GetClassAttribute<EntityLogicalNameAttribute>().LogicalName)) // If an attribute is the same value as the name of the entity, it is created with a 1 post fix to allow for it to compile
            {
                var attribute = typeof(T).GetProperty(property.Name)?.GetCustomAttributes<AttributeLogicalNameAttribute>().FirstOrDefault();
                if (attribute == null)
                {
                    throw new ArgumentException(property.Name + " does not contain an AttributeLogicalNameAttribute.  Unable to determine logical name of " + name);
                }

                name = attribute.LogicalName;
            }

            return name;
        }

        /// <summary>
        /// Converts an early bound entity to the SDK entity, as well as all child entities in EntityCollection Attributes
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Entity ToSdkEntity(this Entity entity)
        {
            var sdkEntity = new Entity(entity.LogicalName);
            sdkEntity.Attributes.AddRange(entity.Attributes);
            sdkEntity.EntityState = entity.EntityState;
            sdkEntity.FormattedValues.AddRange(entity.FormattedValues);
#if !PRE_KEYATTRIBUTE
            sdkEntity.KeyAttributes.AddRange(entity.KeyAttributes);
            sdkEntity.RowVersion = entity.RowVersion;
#endif
            if (entity.Id != Guid.Empty)
            {
                sdkEntity.Id = entity.Id;
            }
            sdkEntity.RelatedEntities.AddRange(entity.RelatedEntities);

            ConvertEntitiesInEntityCollectionAttributesToSdkEntities(sdkEntity);
            return sdkEntity;
        }

        private static void ConvertEntitiesInEntityCollectionAttributesToSdkEntities(Entity entity)
        {
            foreach (var att in entity.Attributes.Select(a => new
            {
                a.Key,
                Value = a.Value as EntityCollection
            }).Where(a => a.Value != null).ToList())
            {
                var sdkEntities = att.Value.Entities.Select(e => e.ToSdkEntity()).ToList();

                att.Value.Entities.Clear();
                att.Value.Entities.AddRange(sdkEntities);
            }
        }

//#if NET
        /// <summary>
        /// Clone Entity (deep copy)
        /// </summary>
        /// <param name="source">source entity.</param>
        /// <param name="deepClone">Also clones Attributes so Option Sets and Entity References are cloned.</param>
        /// <returns>new cloned entity</returns>
        public static T Clone<T>(this T source, bool deepClone = false) where T : Entity
        {
            return source?.CloneInternal(deepClone);
        }
//#else
//        /// <summary>
//        /// Clone Entity (deep copy)
//        /// </summary>
//        /// <param name="source">source entity.</param>
//        /// <param name="serialize">Clone by serializing and deserializing the Entity.  False will return a shallow clone</param>
//        /// <returns>new cloned entity</returns>
//        public static T Clone<T>(this T source, bool serialize = true) where T : Entity
//        {
//            return serialize
//                ? source?.Serialize().DeserializeEntity<T>()
//                : source?.CloneInternal();
//        }
//#endif
        private static T CloneInternal<T>(this T source, bool deepClone = false) where T : Entity
        {
            if (source == null)
            {
                return null;
            }

            var entity = (T)Activator.CreateInstance(source.GetType());
            entity.Id = source.Id;
            entity.LogicalName = source.LogicalName;
            entity.EntityState = source.EntityState;
#if !PRE_KEYATTRIBUTE
            entity.RowVersion = source.RowVersion;
            foreach (var keyAtt in source.KeyAttributes)
            {
                entity.KeyAttributes[keyAtt.Key] = keyAtt.Value;
            }
#endif
            foreach (var attribute in source.Attributes)
            {
                if (deepClone)
                {
                    switch (attribute.Value)
                    {
                        case AliasedValue av:
                            entity[attribute.Key] = new AliasedValue(av.EntityLogicalName, av.AttributeLogicalName, av.Value);
                            break;
                        case Entity e:
#if NET
                            entity[attribute.Key] = e.Clone(true);
#else
                            entity[attribute.Key] = e.Clone();
#endif
                            break;
                        case EntityCollection ec:
#if NET
                            entity[attribute.Key] = ec.Clone(true);
#else
                            entity[attribute.Key] = ec.Clone();
#endif
                            break;
                        case OptionSetValue osv:
                            entity[attribute.Key] = new OptionSetValue(osv.Value);
                            break;
                        case EntityReference er:
                            entity[attribute.Key] = er.Clone();
                            break;
                        case Money m:
                            entity[attribute.Key] = new Money(m.Value);
                            break;
#if !XRM_2013 && !XRM_2015 && !XRM_2016
                        case OptionSetValueCollection osvc:
                            entity[attribute.Key] = new OptionSetValueCollection(osvc.Select(o => new OptionSetValue(o.Value)).ToList());
                            break;
#endif
                        default:
                            entity[attribute.Key] = attribute.Value;
                            break;
                    }
                }
                else
                {
                    entity[attribute.Key] = attribute.Value;
                }
            }
            foreach (var formatted in source.FormattedValues)
            {
                entity.FormattedValues[formatted.Key] = formatted.Value;
            }
            foreach (var related in source.RelatedEntities)
            {
#if NET
                entity.RelatedEntities[related.Key] = related.Value.Clone(deepClone);
#else
                entity.RelatedEntities[related.Key] = related.Value.Clone();
#endif
            }
            return entity;
        }

#endregion Entity

#region EntityCollection

#if NET
        /// <summary>
        /// Clone EntityCollection (deep copy)
        /// </summary>
        /// <param name="source">source EntityCollection.</param>
        /// <param name="deepClone">Also clones Attributes so Option Sets and Entity References are cloned.</param>
        /// <returns>new cloned EntityCollection</returns>
        public static EntityCollection Clone(this EntityCollection source, bool deepClone = false)
        {
            return source.CloneInternal(deepClone);
        }
#else
        /// <summary>
        /// Clone EntityCollection (deep copy)
        /// </summary>
        /// <param name="source">source EntityCollection.</param>
        /// <param name="serialize">Clone by serializing and deserializing the Entity.  False will return a shallow clone</param>
        /// <returns>new cloned EntityCollection</returns>
        public static EntityCollection Clone(this EntityCollection source, bool serialize = true)
        {
            return source.CloneInternal(serialize);
        }
#endif

        private static EntityCollection CloneInternal(this EntityCollection source, bool deepClone = true)
        {
            if (source == null)
            {
                return null;
            }

            var clone = new EntityCollection
            {
                EntityName = source.EntityName,
                MinActiveRowVersion = source.MinActiveRowVersion,
                ExtensionData = source.ExtensionData,
                MoreRecords = source.MoreRecords,
                PagingCookie = source.PagingCookie,
                TotalRecordCount = source.TotalRecordCount,
                TotalRecordCountLimitExceeded = source.TotalRecordCountLimitExceeded
            };

            clone.Entities.AddRange(source.Entities.Select(e => e.Clone(deepClone)));
            return clone;
        }

        /// <summary>
            /// Converts the entity collection into a list, casting each entity.
            /// </summary>
            /// <typeparam name="T">The type of Entity</typeparam>
            /// <param name="col">The collection to convert</param>
            /// <returns></returns>
            public static List<T> ToEntityList<T>(this EntityCollection col) where T : Entity
        {
            if (typeof(T) == typeof(Entity))
            {
                // T is Entity.  No need to cast, just convert.
                return (List<T>)(object)col.Entities.ToList();
            }

            return col.Entities.Select(e => e.AsEntity<T>()).ToList();
        }

#endregion EntityCollection

#region EntityMetadata

        /// <summary>
        /// Gets the text value of the di.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static string GetDisplayNameWithLogical(this EntityMetadata entity)
        {
            return entity.DisplayName.GetLocalOrDefaultText(entity.SchemaName) + " (" + entity.LogicalName + ")";
        }

#endregion EntityMetadata

#region EntityReference

        /// <summary>
        /// Clone Entity (deep copy)
        /// </summary>
        /// <param name="source">source entity.</param>
        /// <returns>new cloned entity</returns>
        public static EntityReference Clone(this EntityReference source)
        {
            if (source == null)
            {
                return null;
            }

            var clone = new EntityReference
            {
                ExtensionData = source.ExtensionData,
                Id = source.Id,
                LogicalName = source.LogicalName,
                Name = source.Name,
#if !PRE_KEYATTRIBUTE
                RowVersion = source.RowVersion,
#endif
            };

#if !PRE_KEYATTRIBUTE
            foreach (var kvp in source.KeyAttributes)
            {
                clone.KeyAttributes[kvp.Key] = kvp.Value;
            }

#endif
            return clone;
        }

        /// <summary>
            /// Returns the Name and Id of an entity reference in this format "Name (id)"
            /// </summary>
            /// <param name="entity"></param>
            /// <returns></returns>
            public static string GetNameId(this EntityReference entity)
        {
            return $"{entity.Name} ({entity.Id})";
        }

        /// <summary>
        /// Returns the Id of the entity reference or Guid.Empty if it is null"
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Guid GetIdOrDefault(this EntityReference entity)
        {
            return entity?.Id ?? Guid.Empty;
        }

        /// <summary>
        /// Returns the Name of the entity reference or null if it is null"
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string GetNameOrDefault(this EntityReference entity)
        {
            return entity?.Name;
        }
        /// <summary>
        /// Allows for Null safe Equals Comparison for more concise code.  i.e.<para />
        /// if(contact.NullSafeEquals(entity))<para />
        /// vs.<para />
        /// if(contact == value || contact != null amps;amps; contact.Equals(entity))
        /// </summary>
        /// <param name="entityReference"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool NullSafeEquals(this EntityReference entityReference, EntityReference value)
        {
            // ReSharper disable once PossibleUnintendedReferenceComparison If EntityReference and Value are both null, or actually both the same reference, then return true
            return entityReference == value || entityReference != null && entityReference.Equals(value);
        }

#endregion EntityReference

#region FetchExpression

        /// <summary>
        /// Get's the logical name of the primary entity for the fetch expression.
        /// </summary>
        /// <param name="fe"></param>
        /// <returns></returns>
        public static string GetEntityName(this FetchExpression fe)
        {
            var xml = new XmlDocument();
            xml.LoadXml(fe.Query);
            return xml.SelectSingleNode("/fetch/entity/@name")?.Value;
        }

#endregion FetchExpression

#region FilterExpression

        /// <summary>
        /// Depending on the Type of T, adds the correct is active criteria Statement
        /// Note: Use AddLink&lt;T&gt; for Linked Entities
        /// </summary>
        /// <typeparam name="T">The Entity type.</typeparam>
        /// <param name="fe"></param>
        public static FilterExpression ActiveOnly<T>(this FilterExpression fe) where T : Entity
        {
            return ActiveOnlyCore(fe, new ActivePropertyInfo<T>());
        }

        /// <summary>
        /// Depending on the Type of T, adds the correct is active criteria Statement
        /// Note: Use AddLink&lt;T&gt; for Linked Entities
        /// </summary>
        /// <param name="fe"></param>
        /// <param name="logicalName">The logical name of the entity to have the active only enforced.</param>
        public static FilterExpression ActiveOnly(this FilterExpression fe, string logicalName)
        {
            return ActiveOnlyCore(fe, new LateBoundActivePropertyInfo(logicalName));
        }

        private static FilterExpression ActiveOnlyCore<T>(FilterExpression fe, ActivePropertyInfo<T> activeInfo) where T : Entity
        {
            switch (activeInfo.ActiveAttribute)
            {
                case ActiveAttributeType.IsDisabled:
                    fe.AddConditionEnforceAndFilterOperator(new ConditionExpression(
                            activeInfo.AttributeName, ConditionOperator.Equal, false));
                    break;
                case ActiveAttributeType.StateCode:
                    if (activeInfo.ActiveState.HasValue)
                    {
                        fe.StateIs(activeInfo.ActiveState.Value);
                    }
                    else
                    {
                        fe.AddConditionEnforceAndFilterOperator(new ConditionExpression(
                                activeInfo.AttributeName, ConditionOperator.NotEqual, activeInfo.NotActiveState.GetValueOrDefault(int.MinValue)));
                    }
                    break;
                case ActiveAttributeType.None:
                    break;
                default:
                    throw new EnumCaseUndefinedException<ActiveAttributeType>(activeInfo.ActiveAttribute);
            }
            return fe;
        }

        /// <summary>
        /// Adds the condition to the Filter Expression, and if the current filter expression's logical operator
        /// is an Or, it will move all of the current conditions into a child filter and create a new
        /// top level and filter
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="ce"></param>
        public static void AddConditionEnforceAndFilterOperator(this FilterExpression criteria, ConditionExpression ce)
        {
            if (criteria.FilterOperator == LogicalOperator.Or)
            {
                // Move the current filter criteria down and add the is active as an and filter join
                var fe = new FilterExpression(LogicalOperator.Or);

                // Move Conditions
                foreach (var condition in criteria.Conditions.ToList())
                {
                    fe.AddCondition(condition);
                    criteria.Conditions.Remove(condition);
                }

                // Move Filters
                foreach (var filter in criteria.Filters.ToList())
                {
                    fe.AddFilter(filter);
                    criteria.Filters.Remove(filter);
                }

                criteria.FilterOperator = LogicalOperator.And;
                criteria.AddCondition(ce);
                criteria.Filters.Add(fe);
            }
            else
            {
                criteria.AddCondition(ce);
            }
        }

        /// <summary>
        /// Adds the conditions to the FilterExpression as normal if the FilterOperator is an And.
        /// If it is an Or, adds the conditions to a child FilterExpression, with a FilterOperator of And.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="conditions"></param>
        public static void AddConditionsAnded(this FilterExpression criteria, params ConditionExpression[] conditions)
        {
            var fe = criteria;
            if (fe.FilterOperator == LogicalOperator.Or)
            {
                fe = new FilterExpression(LogicalOperator.And);
                criteria.AddFilter(fe);
            }

            foreach (var condition in conditions)
            {
                fe.AddCondition(condition);
            }
        }

        /// <summary>
        /// Adds a Condition expression to the filter expression to force the statecode to be a specific value.
        /// </summary>
        /// <param name="fe">The Filter Expression.</param>
        /// <param name="entityStateEnum">The entity state enum value.</param>
        /// <returns>The Filter expression with the ConditionExpression added</returns>
        public static FilterExpression StateIs(this FilterExpression fe, object entityStateEnum)
        {
            fe.AddConditionEnforceAndFilterOperator(new ConditionExpression("statecode", ConditionOperator.Equal, (int)entityStateEnum));
            return fe;
        }

#endregion FilterExpression

#region IExecutionContext

#region ContainsAllNonNull

        /// <summary>
        /// Checks to see if the PluginExecutionContext.InputParameters Contains the attribute names, and the value is not null
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="parameterNames">The parameter names.</param>
        /// <returns></returns>
        public static bool InputContainsAllNonNull(this IExecutionContext context, params string[] parameterNames)
        {
            return context.InputParameters.ContainsAllNonNull(parameterNames);
        }

        /// <summary>
        /// Checks to see if the PluginExecutionContext.OutputParameters Contains the attribute names, and the value is not null
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="parameterNames">The parameter names.</param>
        /// <returns></returns>
        public static bool OutputContainsAllNonNull(this IExecutionContext context, params string[] parameterNames)
        {
            return context.OutputParameters.ContainsAllNonNull(parameterNames);
        }

        /// <summary>
        /// Checks to see if the PluginExecutionContext.SharedVariables Contains the attribute names, and the value is not null
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="parameterNames">The parameter names.</param>
        /// <returns></returns>
        public static bool SharedContainsAllNonNull(this IExecutionContext context, params string[] parameterNames)
        {
            return context.SharedVariables.ContainsAllNonNull(parameterNames);
        }

#endregion ContainsAllNonNull

#region GetParameterValue

        /// <summary>
        /// Gets the parameter value from the PluginExecutionContext.InputParameters collection, cast to type 'T', or default(T) if the collection doesn't contain a parameter with the given name.
        /// </summary>
        /// <typeparam name="T">Type of the parameter to be returned</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns></returns>
        public static T GetInputParameterValue<T>(this IExecutionContext context, string parameterName)
        {
            return context.InputParameters.GetParameterValue<T>(parameterName);
        }

        /// <summary>
        /// Gets the parameter value from the ExecutionContext.InputParameters collection, or null if the collection doesn't contain a parameter with the given name.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns></returns>
        public static object GetInputParameterValue(this IExecutionContext context, string parameterName)
        {
            return context.InputParameters.GetParameterValue(parameterName);
        }

        /// <summary>
        /// Gets the parameter value from the OutputParameters collection, cast to type 'T', or default(T) if the collection doesn't contain a parameter with the given name.
        /// </summary>
        /// <typeparam name="T">Type of the parameter to be returned</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns></returns>
        public static T GetOutputParameterValue<T>(this IExecutionContext context, string parameterName)
        {
            return context.OutputParameters.GetParameterValue<T>(parameterName);
        }

        /// <summary>
        /// Gets the parameter value from the OutputParameters collection, or null if the collection doesn't contain a parameter with the given name.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns></returns>
        public static object GetOutputParameterValue(this IExecutionContext context, string parameterName)
        {
            return context.OutputParameters.GetParameterValue(parameterName);
        }

        /// <summary>
        /// Populates a local version of the request using the parameters from the context.  This exposes (most of) the parameters of that particular request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static T GetRequestParameters<T>(this IExecutionContext context) where T : OrganizationRequest
        {
            var request = Activator.CreateInstance<T>();
            request.Parameters = context.InputParameters;
            return request;
        }

        /// <summary>
        /// Populates a local version of the response using the parameters from the context.  This exposes (most of) the parameters of that particular response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static T GetResponseParameters<T>(this IExecutionContext context) where T : OrganizationResponse
        {
            var response = Activator.CreateInstance<T>();
            response.Results = context.OutputParameters;
            return response;
        }

        /// <summary>
        /// Gets the variable value from the SharedVariables collection, cast to type 'T', or default(T) if the collection doesn't contain a variable with the given name.
        /// </summary>
        /// <typeparam name="T">Type of the variable to be returned</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="variableName">Name of the variable.</param>
        /// <returns></returns>
        public static T GetSharedVariable<T>(this IExecutionContext context, string variableName)
        {
            return context.SharedVariables.GetParameterValue<T>(variableName);
        }

        /// <summary>
        /// Gets the variable value from the PluginExecutionContext.SharedVariables collection, or null if the collection doesn't contain a variable with the given name.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="variableName">Name of the variable.</param>
        /// <returns></returns>
        public static object GetSharedVariable(this IExecutionContext context, string variableName)
        {
            return context.SharedVariables.GetParameterValue(variableName);
        }

#endregion GetParameterValue

#endregion IExecutionContext

#region IServiceProvider

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static T GetService<T>(this IServiceProvider provider)
        {
            return (T)provider.GetService(typeof(T));
        }

        /// <summary>
        /// Creates an IOrganization Service using the default IOrganizationServiceFactory
        /// </summary>
        /// <param name="provider">The Provider.</param>
        /// <param name="userId">The UserId to create the service in context of.</param>
        /// <returns></returns>
        public static IOrganizationService CreateOrganizationService(this IServiceProvider provider, Guid? userId = null)
        {
            return provider.GetService<IOrganizationServiceFactory>().CreateOrganizationService(userId);
        }

#endregion IServiceProvider

#region Label

        /// <summary>
        /// Gets the local or default text.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="defaultIfNull">The default if null.</param>
        /// <returns></returns>
        public static string GetLocalOrDefaultText(this Label label, string defaultIfNull = null)
        {
            var local = label.UserLocalizedLabel ?? label.LocalizedLabels.FirstOrDefault();

            if (local == null)
            {
                return defaultIfNull;
            }

            return local.Label ?? defaultIfNull;
        }

#endregion Label

#region LinkEntity

        /// <summary>
        /// Adds a Condition expression to the LinkCriteria of the LinkEntity to force the statecode to be a specific value.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <param name="entityStateEnum">The entity state enum.</param>
        /// <returns></returns>
        public static LinkEntity StateIs(this LinkEntity link, object entityStateEnum)
        {
            link.LinkCriteria.StateIs(entityStateEnum);
            return link;
        }

        #endregion LinkEntity

#region Money

        /// <summary>
        /// Returns the value of the Money, or 0 if it is null
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        public static decimal GetValueOrDefault(this Money money)
        {
            return GetValueOrDefault(money, 0m);
        }

        /// <summary>
        /// Returns the value of the Money, or the default value if it is null
        /// </summary>
        /// <param name="money">The Money.</param>
        /// <param name="defaultValue">The value to default the Money's Value to if it is null.</param>
        /// <returns></returns>
        public static decimal GetValueOrDefault(this Money money, decimal defaultValue)
        {
            return money?.Value ?? defaultValue;
        }

        /// <summary>
        /// Allows for Null safe Equals Comparison for more concise code.  i.e.<para />
        /// if(contact.Salary.NullSafeEquals(1m))<para />
        /// vs.<para />
        /// if(contact.Salary != null &amp;&amp; contact.gendercode.Value == 1m)
        /// </summary>
        /// <param name="money"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool NullSafeEquals(this Money money, decimal value)
        {
            return money != null && money.Value == value;
        }

        /// <summary>
        /// Allows for Null safe Equals Comparison for more concise code.  i.e.<para />
        /// if(contact.Salary.NullSafeEquals(salary))<para />
        /// vs.<para />
        /// if(contact.Salary == salary || contact.Salary != null amps;amps; contact.Salary.Value == salary.Value)
        /// </summary>
        /// <param name="money"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool NullSafeEquals(this Money money, Money value)
        {
            // ReSharper disable once PossibleUnintendedReferenceComparison If Money and Value are both null, or actually both the same reference, then return true
            return money == value || money != null && money.Equals(value);
        }

#endregion Money

#region ObjectCache

        private static readonly ConcurrentDictionary<string, object> LocksByKey = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// Gets the item from the cache or adds it using the factory functions to both get the value and set the expiration time.
        /// </summary>
        /// <param name="cache">The Cache</param>
        /// <param name="key">The key</param>
        /// <param name="getValue">The getValue factory</param>
        /// <param name="getExpirationTime">The getExpirationTime factory</param>
        /// <returns></returns>
        public static T GetOrAdd<T>(this ObjectCache cache, string key, Func<string, T> getValue, Func<string, T, DateTime> getExpirationTime)
        {
            var value = (T)cache.Get(key);
            if (value != null)
            {
                return value;
            }

            var lockForKey = LocksByKey.GetOrAdd(key, k => new object());
            lock (lockForKey)
            {
                value = (T)cache.Get(key);
                if (value != null)
                {
                    return value;
                }

                value = getValue(key);
                cache.Set(key, value, new CacheItemPolicy
                {
                    AbsoluteExpiration = new DateTimeOffset(getExpirationTime(key, value))
                });
            }

            return value;
        }

#endregion ObjectCache

#region OptionSetValue

        /// <summary>
        /// Returns the value of the OptionSetValue, or int.MinValue if it is null
        /// </summary>
        /// <param name="osv"></param>
        /// <returns></returns>
        public static int GetValueOrDefault(this OptionSetValue osv)
        {
            return GetValueOrDefault(osv, int.MinValue);
        }

        /// <summary>
        /// Returns the value of the OptionSetValue, or the defaultValue if it is null
        /// </summary>
        /// <param name="osv">The OptionSetValue.</param>
        /// <param name="defaultValue">The value to default the OptionSetValue's Value to if it is null.</param>
        /// <returns></returns>
        public static int GetValueOrDefault(this OptionSetValue osv, int defaultValue)
        {
            return osv?.Value ?? defaultValue;
        }

        /// <summary>
        /// Allows for Null safe Equals Comparison for more concise code.  i.e.
        /// if(contact.GenderCode.NullSafeEquals(1))
        /// vs.
        /// if(contact.GenderCode != null &amp;&amp; contact.GenderCode.Value == 1)
        /// </summary>
        /// <param name="osv"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool NullSafeEquals(this OptionSetValue osv, int value)
        {
            return osv != null && osv.Value == value;
        }

        /// <summary>
        /// Allows for Null safe Equals Comparison for more concise code.  i.e.
        /// if(contact.GenderCode.NullSafeEquals(genderCode))
        /// vs.
        /// if(contact.GenderCode == genderCode || contact.GenderCode != null amps;amps; contact.GenderCode.Value == genderCode.Value))
        /// </summary>
        /// <param name="osv"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool NullSafeEquals(this OptionSetValue osv, OptionSetValue value)
        {
            // ReSharper disable once PossibleUnintendedReferenceComparison If Osv and Value are both null, or actually both the same reference, then return true
            return osv == value || osv != null && osv.Equals(value);
        }

#endregion OptionSetValue

#region OrganizationRequestCollection

        /// <summary>
        /// Adds a CreateRequest to the OrganizationRequestCollection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requests">The requests.</param>
        /// <param name="entity">The entity.</param>
        public static void AddCreate<T>(this OrganizationRequestCollection requests, T entity) where T : Entity
        {
            requests.Add(new CreateRequest { Target = entity });
        }

        /// <summary>
        /// Adds a DeleteRequest to the OrganizationRequestCollection.
        /// </summary>
        /// <param name="requests">The requests.</param>
        /// <param name="entity">The entity.</param>
        public static void AddDelete(this OrganizationRequestCollection requests, EntityReference entity)
        {
            requests.Add(new DeleteRequest { Target = entity });
        }

        /// <summary>
        /// Adds a DeleteRequest to the OrganizationRequestCollection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requests">The requests.</param>
        /// <param name="entity">The entity.</param>
        public static void AddDelete<T>(this OrganizationRequestCollection requests, T entity) where T : Entity
        {
            requests.Add(new DeleteRequest { Target = entity.ToEntityReference() });
        }

        /// <summary>
        /// Adds a RetrieveRequest to the OrganizationRequestCollection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requests">The requests.</param>
        /// <param name="id">The identifier.</param>
        public static void AddRetrieve<T>(this OrganizationRequestCollection requests, Guid id) where T : Entity
        {
            requests.AddRetrieve<T>(id, SolutionCheckerAvoider.CreateColumnSetWithAllColumns());
        }

        /// <summary>
        /// Adds a RetrieveRequest to the OrganizationRequestCollection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requests">The requests.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="cs">The cs.</param>
        public static void AddRetrieve<T>(this OrganizationRequestCollection requests, Guid id, ColumnSet cs) where T : Entity
        {
            requests.Add(new RetrieveRequest { Target = new EntityReference(EntityHelper.GetEntityLogicalName<T>(), id), ColumnSet = cs });
        }

        /// <summary>
        /// Adds a RetrieveRequest to the OrganizationRequestCollection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requests">The requests.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="anonymousTypeInitializer">The anonymous type initializer.</param>
        public static void AddRetrieve<T>(this OrganizationRequestCollection requests, Guid id, Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
        {
            var cs = AddColumns(new ColumnSet(), anonymousTypeInitializer);
            requests.Add(new RetrieveRequest { Target = new EntityReference(EntityHelper.GetEntityLogicalName<T>(), id), ColumnSet = cs });
        }

        /// <summary>
        /// Adds a retrieve multiple request to the OrganizationRequestCollection.
        /// </summary>
        /// <param name="requests">The requests.</param>
        /// <param name="query">The query.</param>
        public static void AddRetrieveMultiple(this OrganizationRequestCollection requests, QueryBase query)
        {
            requests.Add(new RetrieveMultipleRequest { Query = query });
        }

        /// <summary>
        /// Adds an update request to the OrganizationRequestCollection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requests">The requests.</param>
        /// <param name="entity">The entity.</param>
        public static void AddUpdate<T>(this OrganizationRequestCollection requests, T entity) where T : Entity
        {
            requests.Add(new UpdateRequest { Target = entity });
        }

#endregion OrganizationRequestCollection

#region ParameterCollection

        /// <summary>
        /// Checks to see if the ParameterCollection Contains the attribute names, and the value is not null
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="attributeNames"></param>
        /// <returns></returns>
        public static bool ContainsAllNonNull(this ParameterCollection parameters, params string[] attributeNames)
        {
            return attributeNames.All(name => parameters.Contains(name) && parameters[name] != null);
        }

        /// <summary>
        /// Gets the parameter value from the collection, cast to type 'T', or default(T) if the collection doesn't contain a parameter with the given name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The ParameterCollection.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns></returns>
        public static T GetParameterValue<T>(this ParameterCollection parameters, string parameterName)
        {
            var attributeValue = parameters.GetParameterValue(parameterName);
            return (T)(attributeValue ?? default(T));
        }

        /// <summary>
        /// Gets the parameter value from the collection, or null if the collection doesn't contain a parameter with the given name.
        /// </summary>
        /// <param name="parameters">The ParameterCollection.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">parameterName</exception>
        public static object GetParameterValue(this ParameterCollection parameters, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentNullException(nameof(parameterName));
            }
            return parameters.Contains(parameterName) ? parameters[parameterName] : null;
        }

#endregion ParameterCollection

#region PropertyInfo

        /// <summary>
        /// Gets the logical attribute name of the given property.  Assumes that the property contains an AttributeLogicalNameAttribute
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="throwIfNotFound">Throws an error if the property does not contain an AttributeLogicalNameAttribute</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static string GetAttributeLogicalName(this PropertyInfo property, bool throwIfNotFound = true)
        {
            var attribute = property.GetCustomAttribute<AttributeLogicalNameAttribute>();
            if (attribute == null && throwIfNotFound)
            {
                throw new Exception($"Property \"{property.Name}\" does not contain an AttributeLogicalNameAttribute.  Unable to determine the Attribute Logical Name.");
            }
            return attribute?.LogicalName;
        }

#endregion PropertyInfo

#region QueryByAttribute

        /// <summary>
        /// Sets the Count and Page number of the query to return just the first entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public static QueryByAttribute First(this QueryByAttribute query)
        {
            var p = GetPageInfo(query);
            p.Count = 1;
            p.PageNumber = 1;

            return query;
        }

        /// <summary>
        /// Updates the Query to only return only the first entity that matches the query criteria.
        /// Shortcut for setting the Query's PageInfo.Count and PageInfo.PageNumber to 1.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="count">The count of entities to restrict the result of the query to.</param>
        public static QueryByAttribute Take(this QueryByAttribute query, int count)
        {
            if (count > 5000)
            {
                throw new ArgumentException("Count must be 5000 or less", nameof(count));
            }

            var p = GetPageInfo(query);
            p.Count = count;
            p.PageNumber = 1;

            return query;
        }

#endregion QueryByAttribute

#region QueryBase

        /// <summary>
        /// Sets the Count and Page number of the query to return just the first entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public static QueryBase First(this QueryBase query)
        {
            var p = GetPageInfo(query);
            p.Count = 1;
            p.PageNumber = 1;

            return query;
        }

        private static PagingInfo GetPageInfo<T>(T qb) where T : QueryBase
        {
            PagingInfo p;
            switch ((QueryBase)qb)
            {
                case QueryExpression qe:
                    p = qe.PageInfo;
                    break;
                case QueryByAttribute qa:
                    p = qa.PageInfo;
                    break;
                default:
                    throw new NotSupportedException("QueryBase of type " + qb.GetType().FullName + " not supported for Getting PageInfo");
            }

            return p;
        }

        /// <summary>
        /// Updates the Query to only return only the first entity that matches the query criteria.
        /// Shortcut for setting the Query's PageInfo.Count and PageInfo.PageNumber to 1.
        /// </summary>
        /// <param name="qb">The query.</param>
        /// <param name="count">The count of entities to restrict the result of the query to.</param>
        public static QueryBase Take(this QueryBase qb, int count)
        {
            if (count > 5000)
            {
                throw new ArgumentException("Count must be 5000 or less", nameof(count));
            }

            var p = GetPageInfo(qb);
            p.Count = count;
            p.PageNumber = 1;

            return qb;
        }

#endregion QueryBase

#region QueryExpression

        /// <summary>
        /// Depending on the Type of T, adds the correct is active criteria Statement
        /// Note: Does not work for Linked Entities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="qe"></param>
        public static QueryExpression ActiveOnly<T>(this QueryExpression qe) where T : Entity
        {
            qe.Criteria.ActiveOnly<T>();
            return qe;
        }

        /// <summary>
        /// Depending on the Type of T, adds the correct is active criteria Statement
        /// Note: Does not work for Linked Entities
        /// </summary>
        /// <param name="qe"></param>
        /// <param name="logicalName">The logical name of the entity to have the active only enforced.</param>
        public static QueryExpression ActiveOnly(this QueryExpression qe, string logicalName)
        {
            qe.Criteria.ActiveOnly(logicalName);
            return qe;
        }

        /// <summary>
        /// Sets the Count and Page number of the query to return just the first entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public static QueryExpression First(this QueryExpression query)
        {
            var p = GetPageInfo(query);
            p.Count = 1;
            p.PageNumber = 1;

            return query;
        }

        /// <summary>
        /// Updates the QueryExpression to only return entities with the given state.
        /// </summary>
        /// <param name="qe">The qe.</param>
        /// <param name="entityStateEnum">The entity state enum.</param>
        /// <returns></returns>
        public static QueryExpression StateIs(this QueryExpression qe, object entityStateEnum)
        {
            qe.Criteria.StateIs(entityStateEnum);
            return qe;
        }

        /// <summary>
        /// Updates the Query to only return only the first entity that matches the query criteria.
        /// Shortcut for setting the Query's PageInfo.Count and PageInfo.PageNumber to 1.
        /// </summary>
        /// <param name="qe">The query.</param>
        /// <param name="count">The count of entities to restrict the result of the query to.</param>
        public static QueryExpression Take(this QueryExpression qe, int count)
        {
            if (count > 5000)
            {
                throw new ArgumentException("Count must be 5000 or less", nameof(count));
            }

            var p = GetPageInfo(qe);
            p.Count = count;
            p.PageNumber = 1;

            return qe;
        }

#endregion QueryExpression

#region String

#if !NET
        /// <summary>
        /// Deserializes the string xml value to a specific entity type.
        /// </summary>
        /// <typeparam name="T">The type of entity to deserialize the xml to.</typeparam>
        /// <param name="xml">The xml to deserialize.</param>
        /// <returns></returns>
        public static T DeserializeEntity<T>(this string xml) where T : Entity
        {
            var entity = xml.DeserializeEntity();
            return entity?.AsEntity<T>();
        }

        /// <summary>
        /// Deserializes the string xml value to an Entity
        /// </summary>
        /// <param name="xml">The xml to deserialize.</param>
        /// <returns></returns>
        public static Entity DeserializeEntity(this string xml)
        {
            return xml.DeserializeDataObject<Entity>();
        }

        /// <summary>
        /// Deserializes the string xml value to an IExtensibleDataObject
        /// </summary>
        /// <param name="xml"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T DeserializeDataObject<T>(this string xml) where T : IExtensibleDataObject
        {
            var serializer = new NetDataContractSerializer();
            return (T)(serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(xml))));
        }

#endif
#endregion String
    }
}
