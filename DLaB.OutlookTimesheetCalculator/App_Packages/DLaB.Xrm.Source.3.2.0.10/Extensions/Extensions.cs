using System;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Xml;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
#if DLAB_UNROOT_COMMON_NAMESPACE
using DLaB.Common;
#else
using Source.DLaB.Common;
#endif
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
        public static T GetEntity<T>(this DataCollection<string, Entity> images, string imageName = null, string defaultName = null) where T: Entity
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
        public static T AsEntity<T>(this Entity entity) where T: Entity
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
        /// Adds the attributes from the given entity if they do not exist in the current
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

            // Search for and replace any occurence of Id with the actual Entity's Id
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
                || name.Substring(name.Length-1) == "1" && name.StartsWith(typeof(T).GetClassAttribute<EntityLogicalNameAttribute>().LogicalName)) // If an attribute is the same value as the name of the entity, it is created with a 1 post fix to allow for it to compile
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

        /// <summary>
        /// Clone Entity (deep copy)
        /// </summary>
        /// <param name="source">source entity.</param>
        /// <returns>new cloned entity</returns>
        public static T Clone<T>(this T source) where T : Entity
        {
#if NETCOREAPP
            var cloned = new Entity(source.LogicalName)
            {
                Id = source.Id,
                LogicalName = source.LogicalName
            };

            foreach (var kvp in source.FormattedValues)
            {
                cloned.FormattedValues.Add(kvp.Key, kvp.Value);
            }

#if !PRE_KEYATTRIBUTE
            foreach (var kvp in source.KeyAttributes)
            {
                cloned.KeyAttributes.Add(kvp.Key, CloneAttribute(kvp.Value));
            }
#endif

            foreach (var kvp in source.Attributes)
            {
                cloned[kvp.Key] = CloneAttribute(kvp.Value);
            }

            foreach (var related in source.RelatedEntities)
            {
                var sourceCollection = related.Value;
                var collection = new EntityCollection(sourceCollection.Entities.Select(e => e.CloneToEntity()).ToList())
                {
                    EntityName = sourceCollection.EntityName,
                    MinActiveRowVersion = sourceCollection.MinActiveRowVersion,
                    PagingCookie = sourceCollection.PagingCookie,
                    MoreRecords = sourceCollection.MoreRecords,
                    TotalRecordCount = sourceCollection.TotalRecordCount,
                    TotalRecordCountLimitExceeded = sourceCollection.TotalRecordCountLimitExceeded
                };

                cloned.RelatedEntities.Add(related.Key, collection);
            }
            return cloned.ToEntity<T>();
#else
            return source?.Serialize().DeserializeEntity<T>();
#endif
        }

        /// <summary>
        /// Clones the entity, then converts it to the same type as the entity
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static Entity CloneToEntity(this Entity source)
        {
            var clone = source.Clone();
            if (source.GetType() == typeof(Entity))
            {
                return clone;
            }
            
            return (Entity)typeof(Entity)
                .GetMethod("ToEntity")
                .MakeGenericMethod(source.GetType())
                .Invoke(clone, new object[0]);
        }

        private static object CloneAttribute(this object value)
        {
            if (value == null)
                return null;

            switch (value)
            {
                case string stringValue:
                    return stringValue;
                case EntityReference entityRef:
                {
                    var clone = new EntityReference(entityRef.LogicalName, entityRef.Id)
                    {
                        Name = entityRef.Name,
                        RowVersion = entityRef.RowVersion
                    };
#if !PRE_KEYATTRIBUTE
                    clone.KeyAttributes.AddRange(entityRef.KeyAttributes.Select(kvp => new KeyValuePair<string,object>(kvp.Key, CloneAttribute(kvp.Value))));
#endif
                    return clone;
                }
                case BooleanManagedProperty boolManaged:
                    return new BooleanManagedProperty(boolManaged.Value);
                case AliasedValue aliased:
                    return new AliasedValue(aliased.EntityLogicalName, aliased.AttributeLogicalName, CloneAttribute(aliased.Value));
                case OptionSetValue optionSetValue:
                    return new OptionSetValue(optionSetValue.Value);
                case Money money:
                    return new Money(money.Value);
                case EntityCollection collection:
                    return new EntityCollection(collection.Entities.Select(e => e.Clone()).ToList());
                case IEnumerable<Entity> entities:
                    return entities.Select(e => e.Clone()).ToArray();
                case byte[] bytes:
                    return bytes.Select(b => b).ToArray();
                default:
                    var type = value.GetType();
                    if(type.GetInterfaces()
                         .Where(i =>  i.IsGenericType)
                         .Any(i => i.GetGenericTypeDefinition() == typeof(IList<>)))
                    {
                        dynamic clonedList;
                        try
                        {
                            clonedList = (dynamic) Activator.CreateInstance(type);
                        }
                        catch(Exception ex)
                        {
                            throw new NotImplementedException($"An attempted was made to clone an attribute of type {type.FullName}, but it does not contain an empty constructor, and therefore requires custom logic to clone.", ex);
                        }
                        foreach(var item in (IEnumerable)value)
                        {
                            clonedList.Add(item.CloneAttribute());
                        }

                        return clonedList;
                    }

                    return value;
            }
        }

#endregion Entity

#region EntityCollection

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

#region EntityReferenceCollection



#endregion EntityReferenceCollection

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

#region IOrganizationService

#region Assign

        /// <summary>
        /// Assigns the supplied entity to the supplied user
        /// </summary>
        /// <param name="service"></param>
        /// <param name="target"></param>
        /// <param name="systemUser"></param>
        /// <returns>AssignResponse</returns>
        public static AssignResponse Assign(this IOrganizationService service, EntityReference target, EntityReference systemUser)
        {
            return (AssignResponse)service.Execute(new AssignRequest
            {
                Assignee = systemUser,
                Target = target
            });
        }

        /// <summary>
        /// Reassigns the owner of the entity to the new owner
        /// </summary>
        /// <param name="service"></param>
        /// <param name="itemToChangeOwnershipOf">Must have Logical Name and Id Populated</param>
        /// <param name="newOwnerId"></param>
        /// <returns></returns>
        public static AssignResponse Assign(this IOrganizationService service, Entity itemToChangeOwnershipOf, Guid newOwnerId)
        {
            return Assign(service, itemToChangeOwnershipOf.ToEntityReference(), newOwnerId);
        }

        /// <summary>
        /// Reassigns the owner of the entity to the new owner
        /// </summary>
        /// <param name="service"></param>
        /// <param name="itemToChangeOwnershipOf">Must have Logical Name and Id Populated</param>
        /// <param name="newOwnerId"></param>
        /// <returns></returns>
        public static AssignResponse Assign(this IOrganizationService service, EntityReference itemToChangeOwnershipOf, Guid newOwnerId)
        {
            return (AssignResponse)service.Execute(new AssignRequest
            {
                Target = itemToChangeOwnershipOf,
                Assignee = new EntityReference("systemuser", newOwnerId),
            });
        }

#endregion Assign

#region Associate

        /// <summary>
        /// Associates one or more entities to an entity.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entity"></param>
        /// <param name="relationshipLogicalName"></param>
        /// <param name="entities"></param>
        public static void Associate(this IOrganizationService service, Entity entity, string relationshipLogicalName, params Entity[] entities)
        {
            var relationship = new Relationship(relationshipLogicalName);
            if (entity.LogicalName == entities.First().LogicalName)
            {
                relationship.PrimaryEntityRole = EntityRole.Referenced;
            }

            service.Associate(entity.LogicalName, entity.Id,
                relationship,
                new EntityReferenceCollection(entities.Select(e => e.ToEntityReference()).ToList()));
        }

        /// <summary>
        /// Associates one or more entities to an entity.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entity"></param>
        /// <param name="relationshipLogicalName"></param>
        /// <param name="entities"></param>
        public static void Associate(this IOrganizationService service, EntityReference entity, string relationshipLogicalName, params EntityReference[] entities)
        {
            var relationship = new Relationship(relationshipLogicalName);
            if (entity.LogicalName == entities.First().LogicalName)
            {
                relationship.PrimaryEntityRole = EntityRole.Referenced;
            }

            service.Associate(entity.LogicalName, entity.Id,
                relationship,
                new EntityReferenceCollection(entities.ToList()));
        }

#endregion Associate

#region CreateWithSuppressDuplicateDetection

        /// <summary>
        /// Creates a record with SuppressDuplicateDetection Enabled to Ignore any potential Duplicates Created
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Guid CreateWithSuppressDuplicateDetection(this IOrganizationService service, Entity entity)
        {
            var response = (CreateResponse)service.Execute(new CreateRequest
                                                           {
                                                               Target = entity,
                                                               ["SuppressDuplicateDetection"] = true
                                                           });
            return response.id;
        }

#endregion CreateWithSuppressDuplicateDetection

#region Delete

        /// <summary>
        /// Deletes the specified entity
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entity">The entity to be deleted.</param>
        public static void Delete(this IOrganizationService service, Entity entity)
        {
            service.Delete(entity.LogicalName, entity.Id);
        }

        /// <summary>
        /// Deletes the specified entity  
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static void Delete(this IOrganizationService service, EntityReference entity)
        {
            service.Delete(entity.LogicalName, entity.Id);
        }

#endregion Delete

#region DeleteIfExists

        /// <summary>
        /// Attempts to delete the entity with the given id. If it doesn't exist, false is returned
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entity">The entity to delete if it exists.</param>
        public static void DeleteIfExists(this IOrganizationService service, Entity entity)
        {
            service.DeleteIfExists(entity.LogicalName, entity.Id);
        }

        /// <summary>
        /// Delete all active entities in the entity specified by the LogicalName and the Filter Expression
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="logicalName">The logical name of the entity that will be deleted.</param>
        /// <param name="fe">The filter expression to use to determine what records to delete.</param>
        /// <returns></returns>
        public static bool DeleteIfExists(this IOrganizationService service, string logicalName, FilterExpression fe)
        {
            var qe = new QueryExpression(logicalName) { Criteria = fe };
            return service.DeleteIfExists(qe);
        }

        /// <summary>
        /// Attempts to delete the entity with the given id. If it doesn't exist, false is returned
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="id">The id of the entity to search and potentially delete.</param>
        /// <returns></returns>
        public static bool DeleteIfExists(this IOrganizationService service, string entityName, Guid id)
        {
            return DeleteIfExistsWithRetry(service, entityName, id, 0);
        }

        /// <summary>
        /// Delete all entities that are returned by the Query Expression.
        /// </summary>
        /// <param name="service">The Service</param>
        /// <param name="qe">The query expression used to define the set of entities to delete</param>
        /// <returns></returns>
        public static bool DeleteIfExists(this IOrganizationService service, QueryExpression qe)
        {
            var exists = false;
            var idName = EntityHelper.GetIdAttributeName(qe.EntityName);
            qe.ColumnSet = new ColumnSet(idName);
            qe.NoLock = true;
            var entities = service.RetrieveMultiple(qe);
            if (entities.Entities.Count > 0)
            {
                exists = true;
                entities.Entities.ToList().ForEach(e => service.Delete(qe.EntityName, e.Id));
            }
            return exists;
        }

        private static bool DeleteIfExistsInternal(IOrganizationService service, string logicalName, Guid id)
        {
            var exists = false;
            var idName = EntityHelper.GetIdAttributeName(logicalName);
            var qe = new QueryExpression(logicalName) { ColumnSet = new ColumnSet(idName) };

            qe.WhereEqual(idName, id);
            qe.First();
            qe.NoLock = true;
            if (service.RetrieveMultiple(qe).Entities.Count > 0)
            {
                service.Delete(logicalName, id);
                exists = true;
            }
            return exists;
        }

        /// <summary>
        /// There have been Generic SQL errors caused with calling this while using multi-threading.  This hopefully
        /// will fix that
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="id">The id.</param>
        /// <param name="retryCount">The retry count.</param>
        /// <returns></returns>
        private static bool DeleteIfExistsWithRetry(IOrganizationService service, string entityName, Guid id,
                                                    int retryCount)
        {
            bool exists;
            try
            {
                exists = DeleteIfExistsInternal(service, entityName, id);
            }
            catch (System.ServiceModel.FaultException<OrganizationServiceFault> ex)
            {
                if (retryCount < 10 && ex.Message.Equals("Generic SQL error.", StringComparison.CurrentCultureIgnoreCase))
                { // This is normally caused by database deadlock issue.  
                    // Attempt to reprocess once after sleeping a random number of milliseconds
                    System.Threading.Thread.Sleep(new Random(System.Threading.Thread.CurrentThread.ManagedThreadId).
                        Next(1000, 5000));
                    exists = DeleteIfExistsWithRetry(service, entityName, id, retryCount + 1);
                }
                else if (ex.Message.EndsWith(id + " Does Not Exist"))
                {
                    exists = false;
                }
                else if (ex.Message == "The object you tried to delete is associated with another object and cannot be deleted.")
                {
                    throw new Exception("Entity " + entityName + " (" + id + ") is associated with another object and cannot be deleted.");
                }
                else
                {
                    throw;
                }
            }

            return exists;
        }

#endregion DeleteIfExists

        /// <summary>
        /// Executes a batch of requests against the CRM Web Service using the ExecuteMultipleRequest command.
        /// </summary>
        /// <remarks>
        /// ExecuteMultipleRequest allows for a maximum of 1000 messages to be processed in a single batch job.
        /// </remarks>
        /// <param name="service">Organization Service proxy for connecting to the relevant CRM instance.</param>
        /// <param name="requestCollection">Collection of organization requests to execute against the CRM Web Services.</param>
        /// <param name="returnResponses">Indicates if responses should be returned for the action taken on each entity in the bulk operation.</param>
        /// <param name="continueOnError">Indicates if the batch job should continue if an error occurs for any of the entities being processed. Default is true.</param>
        /// <returns>Returns the <see cref="ExecuteMultipleResponse"/> containing responses and faults from the operation if the returnResponses parameter is set to true; otherwise returns null. Default is true.</returns>
        public static ExecuteMultipleResponse ExecuteMultiple(this IOrganizationService service, OrganizationRequestCollection requestCollection, bool returnResponses = true, bool continueOnError = true)
        {
            // Validate required parameters.
            if (service == null)
                throw new ArgumentNullException(nameof(service), "A valid Organization Service Proxy must be specified.");
            // Validate the request collection.
            if (requestCollection == null)
                throw new ArgumentNullException(nameof(requestCollection), "The collection of requests to batch process cannot be null.");
            // Ensure the user is not attempting to pass in more than 1000 requests for the batch job, as this is the maximum number CRM allows within a single batch.
            if (requestCollection.Count > 1000)
                throw new ArgumentOutOfRangeException(nameof(requestCollection), "The Entity Collection cannot contain more than 1000 items, as that is the maximum number of messages that can be processed by the CRM web services in a single batch.");

            try
            {
                // Instantiate a new ExecuteMultipleRequest.
                var multipleRequest = new ExecuteMultipleRequest
                {
                    Settings = new ExecuteMultipleSettings { ContinueOnError = continueOnError, ReturnResponses = returnResponses },
                    Requests = requestCollection
                };

                return service.Execute(multipleRequest) as ExecuteMultipleResponse;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while executing an ExecuteMultipleRequest. See inner exception for details.", ex);
            }
        }

#region GetAllEntities

        /// <summary>
        /// Gets all entities using the Query Expression
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qe">Query Expression to Execute.</param>
        /// <param name="maxCount">The maximum number of entities to retrieve.  Use null for default.</param>
        /// <param name="pageSize">Number of records to return in each fetch.  Use null for default.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntities<T>(this IOrganizationService service, QueryExpression qe, int? maxCount = null, int? pageSize = null)
            where T : Entity
        {
            return RetrieveAllEntities<T>.GetAllEntities(service, qe, maxCount, pageSize);
        }

        /// <summary>
        /// Gets all entities using the Query Expression
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qe">Query Expression to Execute.</param>
        /// <param name="maxCount">The maximum number of entities to retrieve.  Use null for default.</param>
        /// <param name="pageSize">Number of records to return in each fetch.  Use null for default.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllEntities<T>(this IOrganizationService service, TypedQueryExpression<T> qe, int? maxCount = null, int? pageSize = null)
            where T : Entity
        {
            return RetrieveAllEntities<T>.GetAllEntities(service, qe, maxCount, pageSize);
        }

#endregion GetAllEntities

        /// <summary>
        /// Returns the WhoAmIResponse to determine the current user's UserId, BusinessUnitId, and OrganizationId
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static WhoAmIResponse GetCurrentlyExecutingUserInfo(this IOrganizationService service)
        {
            return (WhoAmIResponse)service.Execute(new WhoAmIRequest());
        }

#region GetEntity

        /// <summary>
        /// Retrieves the Entity of the given type with the given Id, with all columns
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type</typeparam>
        /// <param name="service">open IOrganizationService</param>
        /// <param name="id">Primary Key of Entity</param>
        /// <returns></returns>
        public static T GetEntity<T>(this IOrganizationService service, Guid id)
            where T : Entity
        {
            return service.GetEntity<T>(id, new ColumnSet(true));
        }

        /// <summary>
        /// Retrieves the Entity of the given type with the given Id, with the given columns
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type</typeparam>
        /// <param name="service">open IOrganizationService</param>
        /// <param name="id">Primary Key of Entity</param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <returns></returns>
        public static T GetEntity<T>(this IOrganizationService service, Guid id, Expression<Func<T, object>> anonymousTypeInitializer)
            where T : Entity
        {
            return service.GetEntity<T>(id, AddColumns(new ColumnSet(), anonymousTypeInitializer));
        }

        /// <summary>
        /// Retrieves the Entity of the given type with the given Id, with the given columns
        /// </summary>
        /// <typeparam name="T">An early bound Entity Type</typeparam>
        /// <param name="service">open IOrganizationService</param>
        /// <param name="id">Primary Key of Entity</param>
        /// <param name="columnSet">Columns to retrieve</param>
        /// <returns></returns>
        public static T GetEntity<T>(this IOrganizationService service, Guid id, ColumnSet columnSet)
            where T : Entity
        {
            return service.Retrieve(EntityHelper.GetEntityLogicalName<T>(), id, columnSet).AsEntity<T>();
        }

#endregion GetEntity

#region GetEntityLogicalName

        private static readonly ConcurrentDictionary<int, string> ObjectTypeToLogicalNameMapping = new ConcurrentDictionary<int, string>();
        private static readonly object ObjectTypeToLogicalNameMappingLock = new object();
        /// <summary>
        /// Gets the Entity Logical Name for the given object Type Code
        /// </summary>
        /// <param name="service"></param>
        /// <param name="objectTypeCode">The Object Type Code</param>
        /// <param name="useCache">Allows for caching the calls in a thread safe manner</param>
        /// <returns></returns>
        public static string GetEntityLogicalName(this IOrganizationService service, int objectTypeCode, bool useCache=true)
        {
            return useCache 
                ? ObjectTypeToLogicalNameMapping.GetOrAddSafe(ObjectTypeToLogicalNameMappingLock, objectTypeCode, c => GetEntityLogicalNameInternal(service, c)) 
                : GetEntityLogicalNameInternal(service, objectTypeCode);
        }

        private static string GetEntityLogicalNameInternal(this IOrganizationService service, int objectTypeCode)
        {
            var entityFilter = new MetadataFilterExpression(LogicalOperator.And); 
            entityFilter.Conditions.Add(new MetadataConditionExpression("ObjectTypeCode", MetadataConditionOperator.Equals, objectTypeCode)); 
            var propertyExpression = new MetadataPropertiesExpression { AllProperties = false }; 
            propertyExpression.PropertyNames.Add("LogicalName");

            var response = (RetrieveMetadataChangesResponse)service.Execute(new RetrieveMetadataChangesRequest 
            { 
                Query = new EntityQueryExpression 
                { 
                    Criteria = entityFilter, 
                    Properties = propertyExpression 
                } 
            }); 
 
            return response.EntityMetadata.Count >= 1 
                ? response.EntityMetadata[0].LogicalName 
                : null;
        }

#endregion GetEntityLogicalName

#region GetEntities

        /// <summary>
        /// Returns first 5000 entities using the Query Expression
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qb">Query to Execute.</param>
        /// <returns></returns>
        public static List<T> GetEntities<T>(this IOrganizationService service, QueryBase qb) where T : Entity
        {
            return service.RetrieveMultiple(qb).ToEntityList<T>();
        }

        /// <summary>
        /// Returns first 5000 entities using the Query Expression
        /// </summary>
        /// <typeparam name="T">Type of Entity List to return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qe">Query Expression to Execute.</param>
        /// <returns></returns>
        public static List<T> GetEntities<T>(this IOrganizationService service, TypedQueryExpression<T> qe) where T : Entity
        {
            return service.RetrieveMultiple(qe).ToEntityList<T>();
        }

#endregion GetEntities

#region GetFirst

        /// <summary>
        /// Gets the first entity that matches the query expression.  An exception is thrown if none are found.
        /// </summary>
        /// <typeparam name="T">The Entity Type.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qe">The query expression.</param>
        /// <returns></returns>
        public static T GetFirst<T>(this IOrganizationService service, QueryExpression qe) where T : Entity
        {
            var entity = service.GetFirstOrDefault<T>(qe);
            AssertExists(entity, qe);
            return entity;
        }

        /// <summary>
        /// Gets the first entity that matches the query expression.  An exception is thrown if none are found.
        /// </summary>
        /// <typeparam name="T">The Entity Type.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qe">The query expression.</param>
        /// <returns></returns>
        public static T GetFirst<T>(this IOrganizationService service, TypedQueryExpression<T> qe) where T : Entity
        {
            var entity = service.GetFirstOrDefault(qe);
            AssertExists(entity, qe);
            return entity;
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void AssertExists<T>(T entity, QueryExpression qe) where T : Entity
        {
            if (entity == null)
            {
                throw new InvalidOperationException("No " + EntityHelper.GetEntityLogicalName<T>() + " found for query " +
                                                    qe.GetSqlStatement());
            }
        }

#endregion GetFirst

#region GetFirstOrDefault

        /// <summary>
        /// Gets the first entity that matches the query expression.  Null is returned if none are found.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public static Entity GetFirstOrDefault(this IOrganizationService service, QueryBase query)
        {
            query.First();
            return service.RetrieveMultiple(query).Entities.FirstOrDefault();
        }

        /// <summary>
        /// Gets the first entity that is returned by the fetch expression.  Null is returned if none are found.
        /// </summary>
        /// <typeparam name="T">The Entity Type.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="fe">The fetch expression.</param>
        /// <returns></returns>
        public static T GetFirstOrDefault<T>(this IOrganizationService service, FetchExpression fe) where T : Entity
        {
            var entity = service.RetrieveMultiple(fe).Entities.FirstOrDefault();
            return entity?.AsEntity<T>();
        }

        /// <summary>
        /// Gets the first entity that matches the query.  Null is returned if none are found.
        /// </summary>
        /// <typeparam name="T">The Entity Type.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qb">The query.</param>
        /// <returns></returns>
        public static T GetFirstOrDefault<T>(this IOrganizationService service, QueryBase qb) where T : Entity
        {
            qb.First();
            return service.GetEntities<T>(qb).FirstOrDefault();
        }

        /// <summary>
        /// Gets the first entity that matches the query expression.  Null is returned if none are found.
        /// </summary>
        /// <typeparam name="T">The Entity Type.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="qe">The query expression.</param>
        /// <returns></returns>
        public static T GetFirstOrDefault<T>(this IOrganizationService service, TypedQueryExpression<T> qe) where T : Entity
        {
            return service.GetFirstOrDefault<T>(qe.Query);
        }

#endregion GetFirstOrDefault

        /// <summary>
        /// Gets the local time from the UTC time.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="userId">The id of the user to lookup the timezone code user settings</param>
        /// <param name="utcTime">The given UTC time to find the user's local time for.  Defaults to DateTime.UtcNow</param>
        /// <param name="defaultTimeZoneCode">Default TimeZoneCode if the user has no TimeZoneCode defined.  Defaults to EDT.</param>
        public static DateTime GetUserLocalTime(this IOrganizationService service, Guid userId, DateTime? utcTime, int defaultTimeZoneCode = 35)
        {
            var timeZoneCode = RetrieveUserSettingsTimeZoneCode(service, userId) ?? defaultTimeZoneCode;
            var request = new LocalTimeFromUtcTimeRequest
            {
                TimeZoneCode = timeZoneCode,
                UtcTime = utcTime ?? DateTime.UtcNow
            };

            var response = (LocalTimeFromUtcTimeResponse)service.Execute(request);

            return response.LocalTime;
        }

        /// <summary>
        /// Retrieves the current users TimeZoneCode
        /// </summary>
        private static int? RetrieveUserSettingsTimeZoneCode(IOrganizationService service, Guid userId)
        {
            // ReSharper disable StringLiteralTypo
            var setting = service.GetFirstOrDefault("usersettings", new ColumnSet("timezonecode"), "systemuserid", userId);
            return setting?.GetAttributeValue<int?>("timezonecode");
            // ReSharper restore StringLiteralTypo
        }

#region InitializeFrom

        /// <summary>
        /// Utilizes the standard OOB Mappings from CRM to hydrate fields on child record from a parent.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="parentEntity">The Parent Entity.</param>
        /// <param name="childLogicalName">The logical name of the child</param>
        /// <param name="targetFieldType">The Target Field Type</param>
        /// <returns></returns>
        public static Entity InitializeFrom(this IOrganizationService service, EntityReference parentEntity, string childLogicalName, TargetFieldType targetFieldType = TargetFieldType.All)
        {
            var initialize = new InitializeFromRequest
            {
                TargetEntityName = childLogicalName,
                EntityMoniker = parentEntity,
                TargetFieldType = targetFieldType
            };
            var initialized = (InitializeFromResponse)service.Execute(initialize);

            return initialized.Entity;
        }

        /// <summary>
        /// Utilizes the standard OOB Mappings from CRM to hydrate fields on child record from a parent.
        /// </summary>
        /// <typeparam name="T">The Entity Type to Return</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="parentEntity">The Parent Entity.</param>
        /// <param name="targetFieldType">The Target Field Type</param>
        /// <returns></returns>
        public static T InitializeFrom<T>(this IOrganizationService service, EntityReference parentEntity, TargetFieldType targetFieldType = TargetFieldType.All) where T: Entity
        {
            var initialize = new InitializeFromRequest
            {
                TargetEntityName = EntityHelper.GetEntityLogicalName<T>(),
                EntityMoniker = parentEntity,
                TargetFieldType = targetFieldType
            };
            var initialized = (InitializeFromResponse)service.Execute(initialize);

            return initialized.Entity.AsEntity<T>();
        }

#endregion InitializeFrom

        /// <summary>
        /// Currently only tested against System Users.  Not sure if it will work with other entities
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="entity">The entity to set the state of.</param>
        /// <param name="state">The state to change the entity to.</param>
        /// <param name="status">The status to change the entity to.</param>
        /// <returns></returns>
        public static SetStateResponse SetState(this IOrganizationService service, Entity entity, int state, int? status)
        {
            var setStateReq = new SetStateRequest
            {
                EntityMoniker = entity.ToEntityReference(),
                State = new OptionSetValue(state),
                Status = new OptionSetValue(status ?? -1)
            };

            return (SetStateResponse)service.Execute(setStateReq);
        }

        /// <summary>
        /// Currently only tested against System Users.  Not sure if it will work with other entities
        /// May need to rename this to SetSystemUserState
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="logicalName">logical name of the entity.</param>
        /// <param name="id">The id of the entity.</param>
        /// <param name="active">if set to <c>true</c> [active].</param>
        /// <returns></returns>
        public static SetStateResponse SetState(this IOrganizationService service, string logicalName, Guid id, bool active)
        {
            var info = new LateBoundActivePropertyInfo(logicalName);
            var state = active ?
                    info.ActiveState ?? 0 :
                    info.NotActiveState ?? (info.ActiveState == 1 ? 0 : 1);


            var setStateReq = new SetStateRequest
            {
                EntityMoniker = new EntityReference(logicalName, id),
                State = new OptionSetValue(state),
                Status = new OptionSetValue(-1)
            };

            return (SetStateResponse)service.Execute(setStateReq);
        }

        /// <summary>
        /// Attempts to delete the Entity, eating the error if it doesn't exist
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="logicalName">Logical name of the entity.</param>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static bool TryDelete(this IOrganizationService service, string logicalName, Guid id)
        {
            var exists = false;
            try
            {
                service.Delete(logicalName, id);
                exists = true;
            }
            catch (System.ServiceModel.FaultException<OrganizationServiceFault> ex)
            {
                if (!ex.Message.EndsWith(id + " Does Not Exist"))
                {
                    throw;
                }
            }

            return exists;
        }

#region UpdateWithSupressDuplicateDetection

        /// <summary>
        /// Creates a record with SuppressDuplicateDetection Enabled to Ignore any potential Duplicates Created
        /// </summary>
        /// <param name="service"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static void UpdateWithSuppressDuplicateDetection(this IOrganizationService service, Entity entity)
        {
            service.Execute(new UpdateRequest
            {
                Target = entity,
                ["SuppressDuplicateDetection"] = true
            });
        }

#endregion CreateWithSupressDuplicateDetection

#endregion IOrganizationService

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
            requests.AddRetrieve<T>(id, new ColumnSet(true));
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
            switch ((QueryBase) qb)
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

#if !NETCOREAPP
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
