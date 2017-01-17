using System;
using Microsoft.Xrm.Sdk.Metadata;
// ReSharper disable UnusedParameter.Local

namespace DLaB.AttributeManager
{
    public partial class Logic
    {
        public object CopyValue(dynamic oldAttribute, dynamic newAttribute, object value)
        {
            return value == null ? null : CopyValueInternal(oldAttribute, newAttribute, value);
        }

        private void CopyValueInternal(object oldAttribute, object newAttribute, object value)
        {
            throw new NotImplementedException("Not Implemented Value Copy From type: " + oldAttribute.GetType().FullName + " to type: " + newAttribute.GetType().FullName);
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, BooleanAttributeMetadata newAttribute, object value)
        {
            bool output;
            if (bool.TryParse(value.ToString(), out output))
            {
                return output;
            }
            try
            {
                return Convert.ToBoolean(value);
            }
            catch
            {
                // Failed to convert.  Give Up
            }
            Trace("Unable to convert value \"" + value + "\" of type \"" + value.GetType().Name + "\" to Double");
            return null;
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, DateTimeAttributeMetadata newAttribute, object value)
        {
            DateTime output;
            if (DateTime.TryParse(value.ToString(), out output))
            {
                return output;
            }
            Trace("Unable to convert value \"" + value + "\" of type \"" + value.GetType().Name + "\" to DateTime");
            return null;
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, DecimalAttributeMetadata newAttribute, object value)
        {
            decimal output;
            if (decimal.TryParse(value.ToString(), out output))
            {
                return output;
            }
            Trace("Unable to convert value \"" + value + "\" of type \"" + value.GetType().Name + "\" to Decimal");
            return null;
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, DoubleAttributeMetadata newAttribute, object value)
        {
            double output;
            if (double.TryParse(value.ToString(), out output))
            {
                return output;
            }
            Trace("Unable to convert value \"" + value + "\" of type \"" + value.GetType().Name + "\" to Double");
            return null;
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, ImageAttributeMetadata newAttribute, object value)
        {
            CopyValueInternal((object)oldAttribute, newAttribute, value);
            return null;
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, IntegerAttributeMetadata newAttribute, object value)
        {
            int output;
            var unformatted = value.ToString();
            // Handle 1.0000
            if (unformatted.Contains("."))
            {
                while (unformatted.EndsWith("0") || unformatted.EndsWith("."))
                {
                    unformatted = unformatted.Substring(0, unformatted.Length - 1);
                }
            }

            if (int.TryParse(unformatted, out output))
            {
                return output;
            }
            Trace("Unable to convert value \"" + value + "\" of type \"" + value.GetType().Name + "\" to Integer");
            return null;
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, LookupAttributeMetadata newAttribute, object value)
        {
            CopyValueInternal((object)oldAttribute, newAttribute, value);
            return null;
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, MoneyAttributeMetadata newAttribute, object value)
        {
            return CopyValueInternal(oldAttribute, new DecimalAttributeMetadata(), value);
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, PicklistAttributeMetadata newAttribute, object value)
        {
            CopyValueInternal((object)oldAttribute, newAttribute, value);
            return null;
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, StringAttributeMetadata newAttribute, object value)
        {
            return value.ToString();
        }
    }
}
