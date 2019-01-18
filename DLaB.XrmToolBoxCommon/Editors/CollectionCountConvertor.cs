using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DLaB.XrmToolBoxCommon.Editors
{
    public class CollectionCountConverter : TypeConverter
    {
        public const string Name = "DLaB.XrmToolBoxCommon.Editors.CollectionCountConverter";

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            var type = value.GetType();

            if (type.IsGenericType 
                && type.GenericTypeArguments.Length >= 1
                && type.GenericTypeArguments[0] == typeof(string)
                && value is ICollection<string> genericCollection)
            {
                return DisplayCount(value, genericCollection.Count);
            }

            return value is ICollection collection 
                ? DisplayCount(value, collection.Count) 
                : base.ConvertTo(context, culture, value, destinationType);
        }

        private static object DisplayCount(object value, int count)
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
