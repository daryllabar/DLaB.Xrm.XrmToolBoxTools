using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
	
{
    public static partial class Extensions
    {
        #region Entity

        #region AddAliased

        /// <summary>
        /// Adds the aliased entity to the current entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="entityToAdd">The entity to add.</param>
        public static void AddAliasedEntity(this Entity entity, Entity entityToAdd)
        {
            foreach (var attribute in entityToAdd.Attributes.Where(a => !(a.Value is AliasedValue)))
            {
                var formattedValue = entityToAdd.FormattedValues.TryGetValue(attribute.Key, out var formatted) ? formatted : null;
                entity.AddAliasedValue(entityToAdd.LogicalName, attribute.Key, attribute.Value, formattedValue);
            }
        }

        /// <summary>
        /// Adds the value to the entity as an Aliased Value.  Helpful when you need to add attributes
        /// that are for other entities locally, in such a way that it looks like it was added by a link on a query
        /// expression
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="logicalName">The logical name from which the aliased value would have come from</param>
        /// <param name="attributeName">The logical name of the attribute of the aliased value</param>
        /// <param name="value">The Value to store in the aliased value</param>
        /// <param name="formattedValue">The formatted value</param>
        public static void AddAliasedValue(this Entity entity, string logicalName, string attributeName, object value, string formattedValue = null)
        {
            entity.AddAliasedValue(null, logicalName, attributeName, value, formattedValue);
        }

        /// <summary>
        /// Adds the value to the entity as an Aliased Value.  Helpful when you need to add attributes
        /// that are for other entities locally, in such a way that it looks like it was added by a link on a query
        /// expression
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="aliasName">Aliased Name of the Attribute</param>
        /// <param name="logicalName">The logical name from which the aliased value would have come from</param>
        /// <param name="attributeName">The logical name of the attribute of the aliased value</param>
        /// <param name="value">The Value to store in the aliased value</param>
        /// <param name="formattedValue">The formatted value</param>
        public static void AddAliasedValue(this Entity entity, string aliasName, string logicalName, string attributeName, object value, string formattedValue = null)
        {
            aliasName = aliasName ?? logicalName;
            var name = aliasName + "." + attributeName;
            entity.Attributes.Add(name, new AliasedValue(logicalName, attributeName, value));

            if (formattedValue != null)
            {
                entity.FormattedValues.Add(name, formattedValue);
            }
        }

        /// <summary>
        /// Adds the value to the entity as an Aliased Value, or replaces an already existing value.  Helpful when you need to add attributes
        /// that are for other entities locally, in such a way that it looks like it was added by a link on a query
        /// expression
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="logicalName">The logical name from which the aliased value would have come from</param>
        /// <param name="attributeName">The logical name of the attribute of the aliased value</param>
        /// <param name="value">The Value to store in the aliased value</param>
        /// <param name="formattedValue">The formatted value</param>
        public static void AddOrReplaceAliasedValue(this Entity entity, string logicalName, string attributeName, object value, string formattedValue = null)
        {
            // Check for value already existing
            foreach (var attribute in entity.Attributes.Where(a => a.Value is AliasedValue))
            {
                var aliasedValue = ((AliasedValue)attribute.Value);
                if (aliasedValue.EntityLogicalName != logicalName || aliasedValue.AttributeLogicalName != attributeName) { continue; }

                entity[attribute.Key] = new AliasedValue(aliasedValue.EntityLogicalName, aliasedValue.AttributeLogicalName, value);
                if (formattedValue != null)
                {
                    entity.FormattedValues[attribute.Key] = formattedValue;
                }
                return;
            }

            entity.AddAliasedValue(logicalName, attributeName, value, formattedValue);
        }

        #endregion AddAliased

        #region GetAliasedEntity

        /// <summary>
        /// Gets the aliased entity from the current entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static T GetAliasedEntity<T>(this Entity entity) where T : Entity, new()
        {
            return entity.GetAliasedEntity<T>(null);
        }

        /// <summary>
        /// Gets the aliased entity from the current entity with the given entity name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="aliasedEntityName">Name of the aliased entity.</param>
        /// <returns></returns>
        public static T GetAliasedEntity<T>(this Entity entity, string aliasedEntityName) where T : Entity, new()
        {
            var entityLogicalName = EntityHelper.GetEntityLogicalName<T>();
            var aliasedEntity = new T();
            var idAttribute = GetIdAttributeName(aliasedEntity);

            foreach (var entry in entity.Attributes)
            {
                if (entry.Value is AliasedValue aliased && aliased.EntityLogicalName == entityLogicalName &&
                    (
                        (aliasedEntityName == null && // No Entity Attribute Name Specified
                            (entry.Key.Contains(".") || entry.Key == aliased.AttributeLogicalName || aliased.AttributeLogicalName == idAttribute))
                    // And the key contains a "." or is an exact match on the aliased attribute logical name.  This keeps groupings that may not be the same type (Date Group by Day) from populating the aliased entity
                    // The one exception for this is the Id. which we want to include if we can
                    ||
                        (aliasedEntityName != null && // Or an Entity Attribute Name is specified, and 
                            entry.Key.StartsWith(aliasedEntityName + ".")) // it starts with the aliasedEntityName + ".
                    ))
                {
                    aliasedEntity[aliased.AttributeLogicalName] = aliased.Value;
                    if (entity.FormattedValues.TryGetValue(entry.Key, out string formattedValue))
                    {
                        aliasedEntity.FormattedValues[aliased.AttributeLogicalName] = formattedValue;
                    }
                }
            }

            // Remove names for Entity References.  
            foreach (var entry in aliasedEntity.Attributes.Where(a => a.Key.EndsWith("name")).ToList())
            {
                var nonNameAttribute = entry.Key.Substring(0, entry.Key.Length - "name".Length);
                if (aliasedEntity.Contains(nonNameAttribute))
                {
                    if (aliasedEntity[nonNameAttribute] is EntityReference entityRef && entityRef.Name == (string)entry.Value)
                    {
                        aliasedEntity.Attributes.Remove(entry.Key);
                    }
                }
            }


            if (aliasedEntity.Attributes.Contains(idAttribute))
            {
                aliasedEntity.Id = (Guid)aliasedEntity[idAttribute];
            }

            return aliasedEntity;
        }

        #endregion GetAliasedEntity

        #region GetAliasedValue

        /// <summary>
        /// Returns the Aliased Value for a column specified in a Linked entity, returning the default value for 
        /// the type if it wasn't found
        /// </summary>
        /// <typeparam name="T">The type of the aliased attribute form the linked entity</typeparam>
        /// <param name="entity"></param>
        /// <param name="attributeName">The aliased attribute from the linked entity.  Can be prepended with the
        /// linked entities logical name and a period. ie "Contact.LastName"</param>
        /// <returns></returns>
        public static T GetAliasedValue<T>(this Entity entity, string attributeName)
        {
            var aliasedEntityName = SplitAliasedAttributeEntityName(ref attributeName);

            var value = (from entry in entity.Attributes
                         where entry.IsSpecifiedAliasedValue(aliasedEntityName, attributeName)
                         select entry.Value).FirstOrDefault();

            if (value == null)
            {
                return default(T);
            }

            if (!(value is AliasedValue aliased))
            {
                throw new InvalidCastException($"Attribute {attributeName} was of type {value.GetType().FullName}, not AliasedValue");
            }

            try
            {
                // If the primary key of an entity is returned, it is returned as a Guid.  If it is a FK, it is returned as an Entity Reference
                // Handle that here
                if (typeof(T) == typeof(EntityReference) && aliased.Value is Guid guid)
                {
                    return (T)(object)new EntityReference(aliased.EntityLogicalName, guid);
                }

                if (typeof(T) == typeof(Guid) && aliased.Value is EntityReference reference)
                {
                    return (T)(object)reference.Id;
                }

                return (T)aliased.Value;
            }
            catch (InvalidCastException)
            {
                throw new InvalidCastException($"Unable to cast attribute {aliased.EntityLogicalName}.{aliased.AttributeLogicalName} from type {aliased.Value.GetType().Name} to type {typeof(T).Name}");
            }
        }

        #endregion GetAliasedValue

        #region HasAliasedAttribute

        /// <summary>
        /// Returns the Aliased Value for a column specified in a Linked entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="attributeName">The aliased attribute from the linked entity.  Can be prepended with the
        /// linked entities logical name and a period. ie "Contact.LastName"</param>
        /// <returns></returns>
        public static bool HasAliasedAttribute(this Entity entity, string attributeName)
        {
            var aliasedEntityName = SplitAliasedAttributeEntityName(ref attributeName);
            return entity.Attributes.Any(e =>
                e.IsSpecifiedAliasedValue(aliasedEntityName, attributeName));
        }

        #endregion HasAliasedAttribute

        #region Helpers

        private static bool IsSpecifiedAliasedValue(this KeyValuePair<string, object> entry, string aliasedEntityName, string attributeName)
        {
            if (!(entry.Value is AliasedValue aliased))
            {
                return false;
            }

            // There are two ways to define an Aliased name that need to be handled
            //   1. At the Entity level in Query Expression or Fetch Xml.  This makes the key in the format AliasedEntityName.AttributeName
            //   2. At the attribute level in FetchXml Group.   This makes the key the Attribute Name.  The aliased Entity Name should always be null in this case

            var value = false;
            if (aliasedEntityName == null)
            {
                // No aliased entity name specified.  If attribute name matches, assume it's correct, or, 
                //     if the key is the attribute name.  This covers the 2nd possibility above
                value = aliased.AttributeLogicalName == attributeName || entry.Key == attributeName;
            }
            else if (aliased.AttributeLogicalName == attributeName)
            {
                // The Aliased Entity Name has been defined.  Check to see if the attribute name join is valid
                value = entry.Key == aliasedEntityName + "." + attributeName;
            }
            return value;
        }

        /// <summary>
        /// Handles splitting the attributeName if it is formatted as "EntityAliasedName.AttributeName",
        /// updating the attribute name and returning the aliased EntityName
        /// </summary>
        /// <param name="attributeName"></param>
        private static string SplitAliasedAttributeEntityName(ref string attributeName)
        {
            string aliasedEntityName = null;
            if (attributeName.Contains('.'))
            {
                var split = attributeName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 2)
                {
                    throw new Exception("Attribute Name was specified for an Aliased Value with " + split.Length +
                    " split parts, and two were expected.  Attribute Name = " + attributeName);
                }
                aliasedEntityName = split[0];
                attributeName = split[1];
            }

            return aliasedEntityName;
        }

        #endregion Helpers

        #endregion Entity
    }
}
