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
                ? GetDisplayCount(context, value, collection.Count) 
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
                    return GetDisplayCountForCollectionOfKvp(context, value);
                }
            }

            return GetDisplayCount(context, value, ((dynamic) value).Count);

        }

        private static string GetDisplayCountForCollectionOfKvp(ITypeDescriptorContext context, object value)
        {
            var count = (from object pairs in (IEnumerable) value
                    select ((dynamic) pairs)?.Value?.Count)
                .Aggregate(0, (current, c) => current + (c == 0 ? 1 : c));
            return GetDisplayCount(context, value, count);
        }

        private static string GetDisplayCount(ITypeDescriptorContext context, object value, int count)
        {
            var info = (CollectionCountAttribute)context?.PropertyDescriptor?.Attributes.Cast<Attribute>().FirstOrDefault(a => a is CollectionCountAttribute)
                       ?? new CollectionCountAttribute();

            string text;
            switch (count)
            {
                case 0:
                    text = info.GetDisplayCountForNone();
                    break;
                case 1:
                    text = info.GetDisplayCountForOne(((IEnumerable) value).Cast<object>().First());
                    break;
                default:
                    text = "Count: " + count;
                    break;
            }

            return text;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CollectionCountAttribute : Attribute
    {
        public string None { get; set; }
        public string OneValueLongFormat { get; set; }
        public string OneValueShortFormat { get; set; }
        public string CountFormat { get; set; }
        public int OneValueMaxDisplayLength { get; set; }

        public CollectionCountAttribute(string none = "(None)", string oneValueLongFormat = "({0})", string oneValueShortFormat = "({0}...)", string countFormat = "Count: {0}", int oneValueMaxDisplayLength = 30)
        {
            None = none;
            OneValueLongFormat = oneValueLongFormat;
            OneValueShortFormat = oneValueShortFormat;
            CountFormat = countFormat;
            OneValueMaxDisplayLength = oneValueMaxDisplayLength;
        }

        public virtual string GetDisplayCountForNone()
        {
            return None;
        }

        public virtual string GetDisplayCountForOne(object value)
        {
            var first = value.ToString();

            var longLabel = string.Format(OneValueLongFormat, first);
            if (longLabel.Length <= OneValueMaxDisplayLength)
            {
                return longLabel;
            }

            var shortLabel = string.Format(OneValueShortFormat, first);
            var excessCharacters = OneValueMaxDisplayLength - shortLabel.Length;
            if (excessCharacters <= 0)
            {
                return shortLabel;
            }

            return first.Length <= excessCharacters 
                ? GetDisplayCount(1) 
                : string.Format(OneValueShortFormat, first.Substring(0, first.Length - excessCharacters));
        }

        public virtual string GetDisplayCount(int count)
        {
            return string.Format(CountFormat, count);
        }
    }
}
