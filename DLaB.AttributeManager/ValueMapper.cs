using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xrm.Sdk;

namespace DLaB.AttributeManager
{
    /// <summary>
    /// Mapping called for instances where the type isn't changing.  Not sure this will be used...
    /// </summary>
    public partial class Logic
    {
        public object MapValue(dynamic value, Dictionary<string, string> migrationMapping)
        {
            return MapValueInternal(value, migrationMapping);
        }

        private object MapValueInternal(OptionSetValue value, Dictionary<string, string> migrationMapping)
        {
            string mappedValue;
            return migrationMapping.TryGetValue(value.Value.ToString(), out mappedValue) 
                ? new OptionSetValue(int.Parse(mappedValue)) 
                : value;
        }

        private object MapValueInternal(Money value, Dictionary<string, string> migrationMapping)
        {
            string mappedValue;
            return migrationMapping.TryGetValue(value.Value.ToString(CultureInfo.InvariantCulture), out mappedValue) 
                ? new Money(decimal.Parse(mappedValue)) 
                : value;
        }

        private object MapValueInternal(EntityReference value, Dictionary<string, string> migrationMapping)
        {
            string mappedValue;
            return migrationMapping.TryGetValue(value.Id.ToString(), out mappedValue)
                ? new EntityReference(value.LogicalName, Guid.Parse(mappedValue))
                : value;
        }

        private object MapValueInternal(string value, Dictionary<string, string> migrationMapping)
        {
            string mappedValue;
            return migrationMapping.TryGetValue(value, out mappedValue)
                ? mappedValue
                : value;
        }

        private object MapValueInternal(int value, Dictionary<string, string> migrationMapping)
        {
            string mappedValue;
            return migrationMapping.TryGetValue(value.ToString(), out mappedValue)
                ? int.Parse(mappedValue)
                : value;
        }

        // ReSharper disable once UnusedParameter.Local
        private object MapValueInternal(object value, Dictionary<string, string> migrationMapping)
        {
            throw new NotImplementedException($"Migration Mapping is not currently supported for type {value.GetType().FullName}");
        }
    }
}
