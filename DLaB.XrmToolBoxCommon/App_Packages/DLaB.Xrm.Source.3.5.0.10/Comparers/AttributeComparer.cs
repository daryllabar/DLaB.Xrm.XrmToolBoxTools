using System.Collections;
using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using System.Linq;
using Microsoft.Xrm.Sdk.Query;
#if DLAB_UNROOT_COMMON_NAMESPACE
using DLaB.Common;
#else
using Source.DLaB.Common;
#endif

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Comparers
#else
namespace Source.DLaB.Xrm.Comparers
#endif
{
    /// <summary>
    /// Comparer for Attributes
    /// </summary>
    public class AttributeComparer
    {
        /// <summary>
        /// Returns true if the given objects are (value) equal.  Assumed to work for any Dataverse value types ie EntityReference, OptionSetValue, etc
        /// </summary>
        /// <param name="service">Service for looking up attribute keys</param>
        /// <param name="value">The value</param>
        /// <param name="preValue">The value to Compare</param>
        /// <returns></returns>
        public static bool ValuesAreEqual(IOrganizationService service, object value, object preValue)
        {
            if (preValue == null)
            {
                return value == null;
            }

            switch (value)
            {
                case null:
                    return false;

                case ColumnSet cs:
                    throw new NotImplementedException("ColumnSet is not Implemented!");

                //case Entity entity:
                //    value = entity.ToStringAttributes(info);
                //    break;

                case EntityReference entityRef:
                    return EntityReferencesAreEqual(service, preValue, entityRef);

                case EntityCollection entities:
                    throw new NotImplementedException("EntityCollection is not Implemented!");

                case EntityReferenceCollection entityRefCollection:
                    throw new NotImplementedException("EntityReferenceCollection is not Implemented!");

                case Dictionary<string, string> dict:
                    return StringDictionariesAreEqual(preValue, dict);

                case FetchExpression fetch:
                    throw new NotImplementedException("FetchExpression is not Implemented!");

                case byte[] imageArray:
                    return imageArray.SequenceEqual((byte[])preValue);

                case IEnumerable enumerable when !(enumerable is string):
                    return NonStringIEnumerablesAreEqual(service, preValue, enumerable);

                case OptionSetValue optionSet:
                    return optionSet.Value == ((OptionSetValue)preValue).Value;

                case Money money:
                    return money.Value == ((Money)preValue).Value;

                case QueryExpression qe:
                    throw new NotImplementedException("QueryExpression is not Implemented!");

                default:
                    return value.Equals(preValue);
            }
        }

        private static bool EntityReferencesAreEqual(IOrganizationService service, object preValue, EntityReference entityRef)
        {
            var preEntityRef = (EntityReference)preValue;
#if !PRE_KEYATTRIBUTE
            var entityRefHasAttributes = entityRef.Id == Guid.Empty
                                         && entityRef.KeyAttributes?.Any() == true;
            var preEntityRefHasAttributes = preEntityRef.Id == Guid.Empty
                                            && preEntityRef.KeyAttributes?.Any() == true;
            if (entityRefHasAttributes != preEntityRefHasAttributes)
            {
                if (entityRefHasAttributes)
                {
                    entityRef = GetEntityReferenceWithoutKeyAttributes(service, entityRef);
                }
                else
                {
                    preEntityRef = GetEntityReferenceWithoutKeyAttributes(service, preEntityRef);
                }
            }
#endif
            return entityRef.Equals(preEntityRef);
        }

        private static bool StringDictionariesAreEqual(object preValue, Dictionary<string, string> dict)
        {
            var preDict = (Dictionary<string, string>)preValue;
            return dict.Keys.OrderBy(k => k).SequenceEqual(preDict.Keys.OrderBy(k => k))
                   && dict.OrderBy(k => k.Key).Select(kvp => kvp.Value).SequenceEqual(preDict.OrderBy(k => k.Key).Select(kvp => kvp.Value));
        }

        private static bool NonStringIEnumerablesAreEqual(IOrganizationService service, object preValue, IEnumerable enumerable)
        {
            var preItems = new Dictionary<Type, List<object>>();
            var preCount = 0;
            foreach (var item in ((IEnumerable)preValue))
            {
                preItems.AddOrAppend(item.GetType(), item);
                preCount++;
            }

            var items = new List<object>();
            foreach (var item in enumerable)
            {
                if (!preItems.TryGetValue(item.GetType(), out var typedValues))
                {
                    return false;
                }

                var match = typedValues.FirstOrDefault(v => ValuesAreEqual(service, item, v));
                if (match == null)
                {
                    return false;
                }

                typedValues.Remove(match);
                items.Add(item);
            }

            return items.Count == preCount;
        }

#if !PRE_KEYATTRIBUTE
        private static EntityReference GetEntityReferenceWithoutKeyAttributes(IOrganizationService service, EntityReference entityRef)
        {
            if (entityRef.Id == Guid.Empty
                && entityRef.KeyAttributes?.Any() == true)
            {
                var entity = service.GetEntityOrDefault(entityRef.LogicalName, entityRef.KeyAttributes, new ColumnSet(false)) ?? new Entity(entityRef.LogicalName);
                entityRef = entity.ToEntityReference();
            }

            return entityRef;
        }
#endif
    }
}
