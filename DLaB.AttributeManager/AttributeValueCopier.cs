using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Source.DLaB.Common;

// ReSharper disable UnusedParameter.Local

namespace DLaB.AttributeManager
{
    public partial class Logic
    {
        public object CopyValue(dynamic oldAttribute, dynamic newAttribute, object value, Dictionary<string,string> migrationMapping)
        {
            return value == null ? null : CopyValueInternal(oldAttribute, newAttribute, value, migrationMapping);
        }

        private object CopyValueInternal(object oldAttribute, object newAttribute, object value, Dictionary<string, string> migrationMapping)
        {
            throw new NotImplementedException("Not Implemented Value Copy From type: " + oldAttribute.GetType().FullName + " to type: " + newAttribute.GetType().FullName);
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, BooleanAttributeMetadata newAttribute, object value, Dictionary<string, string> migrationMapping)
        {
            var copy = value.ToString();
            copy = migrationMapping.TryGetValue(copy, out var mappedValue) ? mappedValue : copy;

            return GetBooleanValue(value, copy);
        }

        private object GetBooleanValue(object value, string stringValue)
        {
            if (bool.TryParse(stringValue, out var output))
            {
                return output;
            }
            try
            {
                return Convert.ToBoolean(stringValue);
            }
            catch
            {
                // Failed to convert.  Give Up
            }
            Trace("Unable to convert value \"" + stringValue + "\" of type \"" + value.GetType().Name + "\" to Boolean");
            return null;
        }

        private object CopyValueInternal(PicklistAttributeMetadata oldAttribute, BooleanAttributeMetadata newAttribute, object value, Dictionary<string, string> migrationMapping)
        {
            var copy = ((OptionSetValue)value).Value.ToString();
            copy = migrationMapping.TryGetValue(copy, out var mappedValue) ? mappedValue : copy;

            return GetBooleanValue(value, copy);
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, DateTimeAttributeMetadata newAttribute, object value, Dictionary<string, string> migrationMapping)
        {
            var copy = value.ToString();
            copy = migrationMapping.TryGetValue(copy, out var mappedValue) ? mappedValue : copy;
            if (DateTime.TryParse(copy, out var output))
            {
                return output;
            }
            Trace("Unable to convert value \"" + value + "\" of type \"" + value.GetType().Name + "\" to DateTime");
            return null;
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, DecimalAttributeMetadata newAttribute, object value, Dictionary<string, string> migrationMapping)
        {
            var copy = value.ToString();
            copy = migrationMapping.TryGetValue(copy, out var mappedValue) ? mappedValue : copy;
            if (decimal.TryParse(copy, out var output))
            {
                return output;
            }
            Trace("Unable to convert value \"" + value + "\" of type \"" + value.GetType().Name + "\" to Decimal");
            return null;
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, DoubleAttributeMetadata newAttribute, object value, Dictionary<string, string> migrationMapping)
        {
            var copy = value.ToString();
            copy = migrationMapping.TryGetValue(copy, out var mappedValue) ? mappedValue : copy;

            if (double.TryParse(copy, out var output))
            {
                return output;
            }
            Trace("Unable to convert value \"" + value + "\" of type \"" + value.GetType().Name + "\" to Double");
            return null;
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, ImageAttributeMetadata newAttribute, object value, Dictionary<string, string> migrationMapping)
        {
            CopyValueInternal((object)oldAttribute, newAttribute, value, migrationMapping);
            return null;
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, IntegerAttributeMetadata newAttribute, object value, Dictionary<string, string> migrationMapping)
        {
            var unformatted = value.ToString();
            unformatted = migrationMapping.TryGetValue(unformatted, out var mappedValue) ? mappedValue : unformatted;

            // Handle 1.0000
            if (unformatted.Contains("."))
            {
                while (unformatted.EndsWith("0") || unformatted.EndsWith("."))
                {
                    unformatted = unformatted.Substring(0, unformatted.Length - 1);
                }
            }

            if (int.TryParse(unformatted, out var output))
            {
                return output;
            }
            Trace("Unable to convert value \"" + value + "\" of type \"" + value.GetType().Name + "\" to Integer");
            return null;
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, BigIntAttributeMetadata newAttribute, object value, Dictionary<string, string> migrationMapping)
        {
            var unformatted = value.ToString();
            unformatted = migrationMapping.TryGetValue(unformatted, out var mappedValue) ? mappedValue : unformatted;

            // Handle 1.0000
            if (unformatted.Contains("."))
            {
                while (unformatted.EndsWith("0") || unformatted.EndsWith("."))
                {
                    unformatted = unformatted.Substring(0, unformatted.Length - 1);
                }
            }

            if (long.TryParse(unformatted, out var output))
            {
                return output;
            }
            Trace("Unable to convert value \"" + value + "\" of type \"" + value.GetType().Name + "\" to Integer");
            return null;
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, LookupAttributeMetadata newAttribute, object value, Dictionary<string, string> migrationMapping)
        {
            CopyValueInternal((object)oldAttribute, newAttribute, value, migrationMapping);
            return null;
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, MoneyAttributeMetadata newAttribute, object value, Dictionary<string, string> migrationMapping)
        {
            return new Money((decimal)CopyValueInternal(oldAttribute, new DecimalAttributeMetadata(), value, migrationMapping));
        }

        private object CopyValueInternal(PicklistAttributeMetadata oldAttribute, PicklistAttributeMetadata newAttribute, object value, Dictionary<string, string> migrationMapping)
        {
            var copy = ((OptionSetValue)value).Value.ToString();
            copy = migrationMapping.TryGetValue(copy, out var mappedValue) ? mappedValue : copy;
            return new OptionSetValue(int.Parse(copy));
        }

        private object CopyValueInternal(BooleanAttributeMetadata oldAttribute, PicklistAttributeMetadata newAttribute, object value, Dictionary<string, string> migrationMapping)
        {
            var copy = value.ToString();
            copy = migrationMapping.TryGetValue(copy, out var mappedValue) ? mappedValue : null;
            if (string.IsNullOrWhiteSpace(copy))
            {
                Trace("Unable to convert value \"" + value + "\" of type \"" + value.GetType().Name + "\" to OptionSetValue");
                return null;
            }
            return new OptionSetValue(int.Parse(copy));
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, PicklistAttributeMetadata newAttribute, object value, Dictionary<string, string> migrationMapping)
        {
            var copy = value.ToString();
            copy = migrationMapping.TryGetValue(copy, out var mappedValue) ? mappedValue : null;
            if (string.IsNullOrWhiteSpace(copy) || !int.TryParse(copy, out var newValue))
            {
                Trace("Unable to convert value \"" + value + "\" of type \"" + value.GetType().Name + "\" to OptionSetValue");
                return null;
            }
            return new OptionSetValue(newValue);
        }

        private object CopyValueInternal(AttributeMetadata oldAttribute, StringAttributeMetadata newAttribute, object value, Dictionary<string, string> migrationMapping)
        {
            var copy = value.ToString();
            copy = migrationMapping.TryGetValue(copy, out var mappedValue) ? mappedValue : copy;
            if (copy.Length > newAttribute.MaxLength)
            {
                copy = copy.Limit(newAttribute.MaxLength ?? 100);
            }

            return copy;
        }


        private object CopyValueInternal(AttributeMetadata oldAttribute, MemoAttributeMetadata newAttribute, object value, Dictionary<string, string> migrationMapping)
        {
            var copy = value.ToString();
            return migrationMapping.TryGetValue(copy, out var mappedValue) ? mappedValue : copy;
        }
    }
}
