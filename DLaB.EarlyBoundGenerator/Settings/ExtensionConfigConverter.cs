using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace DLaB.EarlyBoundGenerator.Settings
{
    public class EntityCollectionConverter<T> : TypeConverter
    {
        //public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        //{
        //    //if (sourceType == typeof(string)) return true;
        //    return base.CanConvertFrom(context, sourceType);
        //}

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(HashSet<T>)) return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is string v)
            {
                using (var reader = new StringReader(v))
                using (var parser = new TextFieldParser(reader))
                {
                    parser.HasFieldsEnclosedInQuotes = true;
                    while (!parser.EndOfData)
                    {
                        var fields = parser.ReadFields();
                        try
                        {
                            return new ExtensionConfig(fields);
                        }
                        catch
                        {
                            throw new InvalidCastException(
                                "Cannot convert the string '" +
                                value + "' into an ExtensionConfig");
                        }
                    }

                    return ExtensionConfig.GetDefault();
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            return destinationType == typeof(string) 
                ? value.ToString() 
                : base.ConvertTo(context, culture, value, destinationType);
        }

        //// Return true to indicate that the object supports properties.
        //public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        //{
        //    return true;
        //}

        //// Return a property description collection.
        //public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        //{
        //    return TypeDescriptor.GetProperties(value);
        //}
    }
}
