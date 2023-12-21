using DLaB.XrmToolBoxCommon.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DLaB.XrmToolBoxCommon.Editors
{
    public class FilterPropertyTypeConverter : TypeConverter
    {
        private FilteredPropertyGrid _filteredPropertyGrid;

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var properties = TypeDescriptor.GetProperties(value);

            if (_filteredPropertyGrid == null || string.IsNullOrEmpty(_filteredPropertyGrid.FilterProperties))
                return properties;

            var propertyDescriptors = new List<PropertyDescriptor>();

            foreach (PropertyDescriptor property in properties)
            {
                if (!property.IsBrowsable)
                    continue;

                if (Convert.ToString(property.DisplayName).ToLower().Contains(_filteredPropertyGrid.FilterProperties.ToLower()))
                    propertyDescriptors.Add(property);
            }

            var propertyDescriptorCollection = new PropertyDescriptorCollection(propertyDescriptors.ToArray());

            return propertyDescriptorCollection;
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            var result = base.GetPropertiesSupported(context);

            var pi = context.GetType().GetProperty("OwnerGrid");

            if (pi == null)
                return result;

            if (!(pi.GetValue(context) is FilteredPropertyGrid propertyGrid))
                return result;

            _filteredPropertyGrid = propertyGrid;

            return !string.IsNullOrEmpty(_filteredPropertyGrid.FilterProperties);
        }
    }

}
