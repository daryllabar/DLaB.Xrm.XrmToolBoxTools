using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace DLaB.XrmToolBoxCommon.Editors
{
    public class CollectionCountConverter : TypeConverter
    {
        public const string Name = "DLaB.XrmToolBoxCommon.Editors.CollectionCountConverter";

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            var type = value.GetType();

            if (type.IsGenericType)
            {
                return ConvertGenericTypeToString(context, culture, value, destinationType);
            }

            return ConvertCollectionToString(context, culture, value, destinationType);
        }

        private string ConvertCollectionToString(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value is ICollection collection 
                ? GetDisplayCount(value, collection.Count) 
                : (string)base.ConvertTo(context, culture, value, destinationType);
        }

        private string ConvertGenericTypeToString(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var type = value.GetType();
            var collectionType = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));
            if(collectionType == null)
            {
                return ConvertCollectionToString(context, culture, value, destinationType);
            }

            // Handle Collection<KeyValuePair<KeyType,ValueType>>
            var firstGenericType = collectionType.GenericTypeArguments[0];
            if (firstGenericType.IsGenericType
                && firstGenericType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                // Check if ValueType is ICollection
                var kvpValueType = firstGenericType.GenericTypeArguments[1];
                if (kvpValueType.IsGenericType
                    && kvpValueType.GetInterfaces().Any(i => 
                        i.IsGenericType 
                        && (i.GetGenericTypeDefinition() == typeof(ICollection<>) 
                            || i.GetGenericTypeDefinition() == typeof(ICollection))))
                {
                    return GetDisplayCountForCollectionOfKvp(value);
                }
            }

            return GetDisplayCount(value, ((dynamic) value).Count);

        }

        private static string GetDisplayCountForCollectionOfKvp(object value)
        {
            var count = (from object pairs in (IEnumerable) value
                    select ((dynamic) pairs)?.Value?.Count)
                .Aggregate(0, (current, c) => current + (c == 0 ? 1 : c));
            return GetDisplayCount(value, count);
        }

        private static string GetDisplayCount(object value, int count)
        {
            string text;
            switch (count)
            {
                case 0:
                    text = "(None)";
                    break;
                case 1:
                    var first = ((IEnumerable) value).Cast<object>().First().ToString();
                    text = first.Length > 20
                        ? $"({first.Substring(0, Math.Min(first.Length, 15))}...)"
                        : $"({first})";
                    break;
                default:
                    text = "Count: " + count;
                    break;
            }

            return text;
        }
    }
}
