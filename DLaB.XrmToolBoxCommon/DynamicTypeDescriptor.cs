using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace DLaB.XrmToolBoxCommon
{
    // Taken from https://www.codeproject.com/Articles/189521/Dynamic-Properties-for-PropertyGrid

    public enum CustomSortOrder
    {
        /// <summary>
        /// No custom sorting
        /// </summary>
        None,
        /// <summary>
        /// sort asscending using the property name or category name
        /// </summary>
        AscendingByName,
        /// <summary>
        /// sort asscending using property id or categor id
        /// </summary>
        AscendingById,
        /// <summary>
        /// sort descending using the property name or category name
        /// </summary>
        DescendingByName,
        /// <summary>
        /// sort descending using property id or categor id
        /// </summary>
        DescendingById
    }

    [Flags]
    public enum PropertyFlags
    {
        [StandardValue("None", "None of the flags should be applied to this property.")]
        None = 0,
        [StandardValue("Display name", "Display name should be retrieved from resource if possible for this property.")]
        LocalizeDisplayName = 1,
        [StandardValue("Category name", "Category name should be retrieved from resource if possible for this property.")]
        LocalizeCategoryName = 2,
        [StandardValue("Description", "Description string should be retrieved from resource if possible for this property.")]
        LocalizeDescription = 4,
        [StandardValue("Enumeration", "Enumerations' display strings should be retrieved from resource if possible  for this property if it is an enumeration type.")]
        LocalizeEnumerations = 8,
        [StandardValue("Exclusive", "Values can only be selected from a list and user are not allowed to type in the value for this property.")]
        ExclusiveStandardValues = 16,

        [StandardValue("Use resource for all string", "Use resource for all string for this property.")]
        LocalizeAllString = LocalizeDisplayName | LocalizeDescription | LocalizeCategoryName | LocalizeEnumerations,

        [StandardValue("Expandable", "Make property expandable if property type is IEnumerable")]
        ExpandIEnumerable = 32,

        [StandardValue("Supports standard values", "Property supports standard values.")]
        SupportStandardValues = 64,

        [StandardValue("All flags", "All of the flags should be applied to this property.")]
        All = LocalizeAllString | ExclusiveStandardValues | ExpandIEnumerable | SupportStandardValues,

        Default = LocalizeAllString | SupportStandardValues
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyStateFlagsAttribute : Attribute
    {
        public PropertyStateFlagsAttribute()
        { }

        public PropertyStateFlagsAttribute(PropertyFlags flags)
        {
            Flags = flags;
        }

        public PropertyFlags Flags { get; set; } = PropertyFlags.All & ~PropertyFlags.ExclusiveStandardValues;
    }

    public interface IResourceAttribute
    {
        string BaseName { get; }

        string KeyPrefix { get; }

        string AssemblyFullName { get; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class ClassResourceAttribute : Attribute, IResourceAttribute
    {
        public ClassResourceAttribute()
        { }

        public ClassResourceAttribute(string baseString)
        {
            BaseName = baseString;
        }

        public ClassResourceAttribute(string baseString, string keyPrefix)
        {
            BaseName = baseString;
            KeyPrefix = keyPrefix;
        }

        public string BaseName { get; } = string.Empty;

        public string KeyPrefix { get; } = string.Empty;
        public string AssemblyFullName { get; } = string.Empty;

        // Use the hash code of the string objects and xor them together.
        public override int GetHashCode()
        {
            return (BaseName.GetHashCode() ^ KeyPrefix.GetHashCode()) ^ AssemblyFullName.GetHashCode();
        }


        public override bool Equals(object obj)
        {
            if (!(obj is ClassResourceAttribute))
            {
                return false;
            }
            var other = (ClassResourceAttribute) obj;

            if (string.Compare(BaseName, other.BaseName, StringComparison.OrdinalIgnoreCase) == 0 &&
                string.Compare(AssemblyFullName, other.AssemblyFullName, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            return false;
        }

        public override bool Match(object obj)
        {
            // Obviously a match.
            if (obj == this)
                return true;

            // Obviously we're not null, so no.
            if (obj == null)
                return false;

            if (obj is ClassResourceAttribute attribute)
                // Combine the hash codes and see if they're unchanged.
                return (attribute.GetHashCode() & GetHashCode())
                  == GetHashCode();
            return false;
        }

    }

    [AttributeUsage(AttributeTargets.Enum)]
    public class EnumResourceAttribute : Attribute, IResourceAttribute
    {
        public EnumResourceAttribute()
        {

        }
        public EnumResourceAttribute(string baseString)
        {
            BaseName = baseString;
        }
        public EnumResourceAttribute(string baseString, string keyPrefix)
        {
            BaseName = baseString;
            KeyPrefix = keyPrefix;
        }

        public string BaseName { get; } = string.Empty;

        public string KeyPrefix { get; } = string.Empty;
        public string AssemblyFullName { get; } = string.Empty;


        // Use the hash code of the string objects and xor them together.
        public override int GetHashCode()
        {

            return (BaseName.GetHashCode() ^ KeyPrefix.GetHashCode()) ^ AssemblyFullName.GetHashCode();
        }


        public override bool Equals(object obj)
        {
            if (!(obj is ClassResourceAttribute))
            {
                return false;
            }
            var other = (ClassResourceAttribute) obj;

            return string.Compare(BaseName, other.BaseName, StringComparison.OrdinalIgnoreCase) == 0 &&
                   string.Compare(AssemblyFullName, other.AssemblyFullName, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override bool Match(object obj)
        {
            // Obviously a match.
            if (obj == this)
                return true;

            // Obviously we're not null, so no.
            if (obj == null)
                return false;

            if (obj is ClassResourceAttribute attribute)
                // Combine the hash codes and see if they're unchanged.
                return (attribute.GetHashCode() & GetHashCode())
                  == GetHashCode();
            return false;
        }

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyResourceAttribute : Attribute, IResourceAttribute
    {
        public PropertyResourceAttribute()
        {

        }
        public PropertyResourceAttribute(string baseString)
        {
            BaseName = baseString;
        }
        public PropertyResourceAttribute(string baseString, string keyPrefix)
        {
            BaseName = baseString;
            KeyPrefix = keyPrefix;
        }

        public string BaseName { get; set; } = string.Empty;

        public string KeyPrefix { get; set; } = string.Empty;
        public string AssemblyFullName { get; set; } = string.Empty;

        // Use the hash code of the string objects and xor them together.
        public override int GetHashCode()
        {

            return (BaseName.GetHashCode() ^ KeyPrefix.GetHashCode()) ^ AssemblyFullName.GetHashCode();
        }


        public override bool Equals(object obj)
        {
            if (!(obj is ClassResourceAttribute))
            {
                return false;
            }
            var other = (ClassResourceAttribute) obj;

            return string.Compare(BaseName, other.BaseName, StringComparison.OrdinalIgnoreCase) == 0 &&
                   string.Compare(AssemblyFullName, other.AssemblyFullName, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public override bool Match(object obj)
        {
            // Obviously a match.
            if (obj == this)
                return true;

            // Obviously we're not null, so no.
            if (obj == null)
                return false;

            if (obj is ClassResourceAttribute attribute)
                // Combine the hash codes and see if they're unchanged.
                return (attribute.GetHashCode() & GetHashCode())
                  == GetHashCode();
            return false;
        }
    }


    [AttributeUsage(AttributeTargets.Property)]
    public class IdAttribute : Attribute
    {
        public IdAttribute()
        {
        }

        public IdAttribute(int propertyId, int categoryId)
        {
            PropertyId = propertyId;
            CategoryId = categoryId;
        }

        public int PropertyId { get; set; }

        public int CategoryId { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class StandardValueAttribute : Attribute
    {
        //public StandardValueAttribute()
        //{

        //}

        public StandardValueAttribute(object value)
        {
            Value = value;
        }
        public StandardValueAttribute(object value, string displayName)
        {
            m_DisplayName = displayName;
            Value = value;
        }
        public StandardValueAttribute(string displayName, string description)
        {
            m_DisplayName = displayName;
            Description = description;
        }
        private string m_DisplayName = string.Empty;
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(m_DisplayName))
                {
                    if (Value != null)
                    {
                        return Value.ToString();
                    }
                }
                return m_DisplayName;
            }
            set
            {
                m_DisplayName = value;
            }
        }
        public bool Visible { get; set; } = true;
        public bool Enabled { get; set; } = true;
        public string Description { get; set; } = string.Empty;

        public object Value { get; internal set; }

        public override string ToString()
        {
            return DisplayName;
        }
        internal static StandardValueAttribute[] GetEnumItems(Type enumType)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("enumType");
            }

            if (!enumType.IsEnum)
            {
                throw new ArgumentException("'enumInstance' is not Enum type.");
            }

            ArrayList arrAttr = new ArrayList();
            FieldInfo[] fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fi in fields)
            {
                if (fi.GetCustomAttributes(typeof(StandardValueAttribute), false) is StandardValueAttribute[] attr && attr.Length > 0)
                {
                    attr[0].Value = fi.GetValue(null);
                    arrAttr.Add(attr[0]);
                }
                else
                {
                    StandardValueAttribute atr = new StandardValueAttribute(fi.GetValue(null));
                    arrAttr.Add(atr);
                }
            }
            StandardValueAttribute[] retAttr = arrAttr.ToArray(typeof(StandardValueAttribute)) as StandardValueAttribute[];
            return retAttr;
        }
    }

    //public class StandardValueEditor : UITypeEditor
    //{
    //    private StandardValueEditorUI m_ui = new StandardValueEditorUI();
    //
    //    public StandardValueEditor()
    //    {
    //
    //    }
    //
    //    public override bool GetPaintValueSupported(System.ComponentModel.ITypeDescriptorContext context)
    //    {
    //        return false;
    //    }
    //
    //    public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
    //    {
    //        return UITypeEditorEditStyle.DropDown;
    //    }
    //
    //    public override bool IsDropDownResizable
    //    {
    //        get
    //        {
    //            return true;
    //        }
    //    }
    //
    //    public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
    //    {
    //        if (provider != null)
    //        {
    //            IWindowsFormsEditorService editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
    //            if (editorService == null)
    //                return value;
    //
    //            m_ui.SetData(context, editorService, value);
    //
    //            editorService.DropDownControl(m_ui);
    //
    //            value = m_ui.GetValue();
    //
    //        }
    //
    //        return value;
    //    }
    //}

    public class PropertyValuePaintEditor : UITypeEditor
    {
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            // let the property browser know we'd like
            // to do custom painting.
            if (context != null)
            {
                if (context.PropertyDescriptor != null)
                {
                    if (context.PropertyDescriptor is CustomPropertyDescriptor)
                    {
                        CustomPropertyDescriptor cpd = context.PropertyDescriptor as CustomPropertyDescriptor;
                        return (cpd.ValueImage != null);
                    }
                }
            }
            return base.GetPaintValueSupported(context);
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.None;
        }
        public override void PaintValue(PaintValueEventArgs pe)
        {
            if (pe.Context != null)
            {
                if (pe.Context.PropertyDescriptor != null)
                {
                    if (pe.Context.PropertyDescriptor is CustomPropertyDescriptor)
                    {
                        CustomPropertyDescriptor cpd = pe.Context.PropertyDescriptor as CustomPropertyDescriptor;

                        if (cpd.ValueImage != null)
                        {
                            pe.Graphics.DrawImage(cpd.ValueImage, pe.Bounds);
                            return;
                        }
                    }
                }
            }
            base.PaintValue(pe);
        }

    }

    internal class StandardValuesConverter : TypeConverter
    {
        private static int Count { get; set; }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            if (context?.PropertyDescriptor is CustomPropertyDescriptor)
            {
                CustomPropertyDescriptor cpd = context.PropertyDescriptor as CustomPropertyDescriptor;
                if (cpd.GetValue(context.Instance) is IEnumerable enu && cpd.PropertyFlags != PropertyFlags.None && (cpd.PropertyFlags & PropertyFlags.ExpandIEnumerable) > 0)
                {
                    return true;
                }
            }
            return base.GetPropertiesSupported(context);

        }
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            if (value == null)
            {
                return base.GetProperties(context, value, attributes);
            }

            //ICollection<StandardValueAttribute> col = null;
            var propType = Type.Missing.GetType();
            if (context?.PropertyDescriptor is CustomPropertyDescriptor)
            {
                CustomPropertyDescriptor cpd = context.PropertyDescriptor as CustomPropertyDescriptor;
                UpdateEnumDisplayText(cpd);
                //col = cpd.StandardValues;
                propType = cpd.PropertyType;
            }
            var pdl = new List<CustomPropertyDescriptor>();
            var nIndex = -1;
            if (pdl.Count == 0)
            {
                if (value is IEnumerable en)
                {
                    var enu = en.GetEnumerator();
                    enu.Reset();
                    while (enu.MoveNext())
                    {
                        nIndex++;
                        string sPropName = enu.Current.ToString();

                        if (enu.Current is IComponent comp && comp.Site != null && !String.IsNullOrEmpty(comp.Site.Name))
                        {
                            sPropName = comp.Site.Name;
                        }
                        else if (propType.IsArray)
                        {
                            sPropName = "[" + nIndex + "]";
                        }
                        pdl.Add(new CustomPropertyDescriptor(null, sPropName, enu.Current.GetType(), enu.Current));
                    }
                }
            }
            return pdl.Count > 0 
                ? new PropertyDescriptorCollection(pdl.Cast<PropertyDescriptor>().ToArray()) 
                : base.GetProperties(context, value, attributes);
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            if (context?.PropertyDescriptor is CustomPropertyDescriptor cpd)
            {
                if (cpd.PropertyFlags != PropertyFlags.None && (cpd.PropertyFlags & PropertyFlags.SupportStandardValues) > 0)
                {
                    return true;
                }
            }
            return base.GetStandardValuesSupported(context);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            if (context?.PropertyDescriptor is CustomPropertyDescriptor)
            {
                var cpd = context.PropertyDescriptor as CustomPropertyDescriptor;
                if (cpd.PropertyType == typeof(bool) || cpd.PropertyType.IsEnum)
                {
                    return true;
                }
                if (cpd.PropertyFlags != PropertyFlags.None && ((cpd.PropertyFlags & PropertyFlags.ExclusiveStandardValues) > 0))
                {
                    return true;
                }
                return false;
            }

            return base.GetStandardValuesExclusive(context);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            //WriteContext("ConvertFrom", context, value, Type.Missing.GetType( ));

            ICollection<StandardValueAttribute> col = null;
            var propType = Type.Missing.GetType();
            if (context?.PropertyDescriptor is CustomPropertyDescriptor cpd)
            {
                UpdateEnumDisplayText(cpd);
                col = cpd.StandardValues;
                propType = cpd.PropertyType;
            }
            if (value == null)
            {
                return null;
            }

            if (value is string inputValue)
            {
                if (propType.IsEnum)
                {
                    var displayNames = inputValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    var sb = new StringBuilder(1000);
                    foreach (string displayName in displayNames)
                    {
                        string sTrimValue = displayName.Trim();
                        foreach (var sva in col)
                        {
                            if (String.Compare(sva.Value.ToString(), sTrimValue, true) == 0 ||
                                String.Compare(sva.DisplayName, sTrimValue, true) == 0)
                            {
                                if (sb.Length > 0)
                                {
                                    sb.Append(",");
                                }
                                sb.Append(sva.Value);
                            }
                        }

                    }  // end of foreach..loop
                    return Enum.Parse(propType, sb.ToString(), true);
                }
                foreach (StandardValueAttribute sva in col)
                {
                    if (String.Compare(inputValue.ToString(), sva.DisplayName, true, culture) == 0 ||
                        String.Compare(inputValue.ToString(), sva.Value.ToString(), true, culture) == 0)
                    {
                        return sva.Value;

                    }
                }
                TypeConverter tc = TypeDescriptor.GetConverter(propType);
                if (tc != null)
                {
                    object convertedValue = null;
                    try
                    {
                        convertedValue = tc.ConvertFrom(context, culture, inputValue);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    if (tc.IsValid(convertedValue))
                    {
                        return convertedValue;
                    }
                }
            }
            else if (value.GetType() == propType)
            {
                return value;
            }
            else if (value is StandardValueAttribute)
            {
                return (value as StandardValueAttribute).Value;
            }

            return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            //WriteContext("ConvertTo", context, value, destinationType);

            ICollection<StandardValueAttribute> col = null;
            Type propType = Type.Missing.GetType();
            if (context?.PropertyDescriptor is CustomPropertyDescriptor)
            {
                CustomPropertyDescriptor cpd = context.PropertyDescriptor as CustomPropertyDescriptor;
                UpdateEnumDisplayText(cpd);
                col = cpd.StandardValues;
                propType = cpd.PropertyType;
            }
            if (value == null)
            {
                return null;
            }

            if (value is string)
            {
                if (destinationType == typeof(string))
                {
                    return value;
                }

                if (destinationType == propType)
                {
                    return ConvertFrom(context, culture, value);
                }

                if (destinationType == typeof(StandardValueAttribute))
                {
                    foreach (StandardValueAttribute sva in col)
                    {
                        if (String.Compare(value.ToString(), sva.DisplayName, true, culture) == 0 ||
                            String.Compare(value.ToString(), sva.Value.ToString(), true, culture) == 0)
                        {
                            return sva;
                        }
                    }
                }
            }
            else if (value.GetType() == propType)
            {
                if (destinationType == typeof(string))
                {
                    if (propType.IsEnum)
                    {
                        string sDelimitedValues = Enum.Format(propType, value, "G");
                        string[] arrValue = sDelimitedValues.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        StringBuilder sb = new StringBuilder(1000);
                        foreach (string sDispName in arrValue)
                        {
                            string sTrimValue = sDispName.Trim();
                            foreach (StandardValueAttribute sva in col)
                            {
                                if (String.Compare(sva.Value.ToString(), sTrimValue, true) == 0 ||
                                    String.Compare(sva.DisplayName, sTrimValue, true) == 0)
                                {
                                    if (sb.Length > 0)
                                    {
                                        sb.Append(", ");
                                    }
                                    sb.Append(sva.DisplayName);
                                }
                            }

                        }  // end of foreach..loop
                        return sb.ToString();
                    }
                    foreach (StandardValueAttribute sva in col)
                    {
                        if (sva.Value.Equals(value))
                        {
                            return sva.DisplayName;
                        }
                    }
                    TypeConverter tc = TypeDescriptor.GetConverter(propType);
                    if (tc != null)
                    {
                        object convertedValue = null;
                        try
                        {
                            convertedValue = tc.ConvertTo(context, culture, value, destinationType);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        if (tc.IsValid(convertedValue))
                        {
                            return convertedValue;
                        }
                    }
                }
                else if (destinationType == typeof(StandardValueAttribute))
                {
                    foreach (StandardValueAttribute sva in col)
                    {
                        if (sva.Value.Equals(value))
                        {
                            return sva;
                        }
                    }

                }
                else if (destinationType == propType)
                {
                    return value;
                }
            }
            else if (value is StandardValueAttribute)
            {
                if (destinationType == typeof(string))
                {
                    return (value as StandardValueAttribute).DisplayName;
                }

                if (destinationType == typeof(StandardValueAttribute))
                {
                    return value;
                }

                if (destinationType == propType)
                {
                    return (value as StandardValueAttribute).Value;
                }
            }
            return base.ConvertFrom(context, culture, value);

        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            ICollection<StandardValueAttribute> col = null;
            if (context?.PropertyDescriptor is CustomPropertyDescriptor)
            {
                CustomPropertyDescriptor cpd = context.PropertyDescriptor as CustomPropertyDescriptor;
                UpdateEnumDisplayText(cpd);
                col = cpd.StandardValues;
            }

            List<StandardValueAttribute> list = new List<StandardValueAttribute>();
            foreach (StandardValueAttribute sva in col)
            {
                if (sva.Visible)
                {
                    list.Add(sva);
                }
            }
            if (list.Count > 0)
            {
                StandardValuesCollection svc = new StandardValuesCollection(list);
                return svc;
            }

            return base.GetStandardValues(context);
        }
        private void UpdateEnumDisplayText(CustomPropertyDescriptor cpd)
        {
            if (!(cpd.PropertyType.IsEnum || cpd.PropertyType == typeof(bool)))
            {
                return;
            }
            if ((cpd.PropertyFlags & PropertyFlags.LocalizeEnumerations) <= 0)
            {
                return;
            }
            string prefix = string.Empty;
            ResourceManager rm = null;
            StandardValueAttribute sva = null;

            sva = cpd.StandardValues.FirstOrDefault();

            // first try property itself
            if (cpd.ResourceManager != null)
            {
                string keyName = cpd.KeyPrefix + cpd.Name + "_" + sva.Value + "_Name";
                string valueName = cpd.ResourceManager.GetString(keyName);
                if (!String.IsNullOrEmpty(valueName))
                {
                    rm = cpd.ResourceManager;
                    prefix = cpd.KeyPrefix + cpd.Name;
                }
            }

            // now try class level
            if (rm == null && cpd.ResourceManager != null)
            {
                string keyName = cpd.KeyPrefix + cpd.PropertyType.Name + "_" + sva.Value + "_Name";
                string valueName = cpd.ResourceManager.GetString(keyName);
                if (!string.IsNullOrEmpty(valueName))
                {
                    rm = cpd.ResourceManager;
                    prefix = cpd.KeyPrefix + cpd.PropertyType.Name;
                }
            }

            // try the enum itself if still null
            if (rm == null && cpd.PropertyType.IsEnum)
            {
                EnumResourceAttribute attr = (EnumResourceAttribute)cpd.AllAttributes.FirstOrDefault(a => a is EnumResourceAttribute);
                if (attr != null)
                {
                    try
                    {
                        if (String.IsNullOrEmpty(attr.AssemblyFullName) == false)
                        {
                            rm = new ResourceManager(attr.BaseName, Assembly.ReflectionOnlyLoad(attr.AssemblyFullName));
                        }
                        else
                        {
                            rm = new ResourceManager(attr.BaseName, cpd.PropertyType.Assembly);
                        }
                        prefix = attr.KeyPrefix + cpd.PropertyType.Name;
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
            }

            if (rm != null)
            {
                foreach (StandardValueAttribute sv in cpd.StandardValues)
                {
                    string keyName = prefix + "_" + sv.Value + "_Name";  // display name
                    string keyDesc = prefix + "_" + sv.Value + "_Desc"; // description
                    string dispName = string.Empty;
                    string description = string.Empty;

                    try
                    {
                        dispName = rm.GetString(keyName);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    if (String.IsNullOrEmpty(dispName) == false)
                    {
                        sv.DisplayName = dispName;
                    }

                    try
                    {
                        description = rm.GetString(keyDesc);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    if (String.IsNullOrEmpty(description) == false)
                    {
                        sv.Description = description;
                    }
                }
            }
        }

        private void WriteContext(string prefix, ITypeDescriptorContext ctx, object value, Type destinationType)
        {
            Count++;
            StringBuilder sb = new StringBuilder(1024);

            if (ctx != null)
            {
                if (ctx.Instance != null)
                {
                    sb.Append("ctx.Instance is " + ctx.Instance + ". ");
                }

                if (ctx.PropertyDescriptor != null)
                {
                    sb.Append("ctx.PropertyDescriptor is " + ctx.PropertyDescriptor + ". ");
                }
            }
            else
            {
                sb.Append("ctx is null. ");
            }

            if (value == null)
            {
                sb.AppendLine("Value is null. ");
            }
            else
            {
                sb.AppendLine("Value is " + value + ", " + value.GetType() + ". ");
            }
            sb.AppendLine(destinationType.ToString());
            Console.WriteLine(Count + " " + prefix + ": " + sb);
        }

    }

    internal class PropertyDescriptorList : List<CustomPropertyDescriptor>
    {
    }

    internal class AttributeList : List<Attribute>
    {
        public AttributeList()
        {

        }
        public AttributeList(AttributeCollection ac)
        {
            foreach (Attribute attr in ac)
            {
                Add(attr);
            }
        }
        public AttributeList(Attribute[] aa)
        {
            foreach (Attribute attr in aa)
            {
                Add(attr);
            }
        }
    }

    internal class PropertySorter : IComparer<CustomPropertyDescriptor>
    {
        #region IComparer<PropertyDescriptor> Members

        public int Compare(CustomPropertyDescriptor x, CustomPropertyDescriptor y)
        {

            switch (SortOrder)
            {
                case CustomSortOrder.AscendingById:
                    if (x.PropertyId > y.PropertyId)
                    {
                        return 1;
                    }
                    else if (x.PropertyId < y.PropertyId)
                    {
                        return -1;
                    }
                    return 0;
                case CustomSortOrder.AscendingByName:
                    return (String.Compare(x.DisplayName, y.DisplayName, true));
                case CustomSortOrder.DescendingById:
                    if (x.PropertyId > y.PropertyId)
                    {
                        return -1;
                    }
                    else if (x.PropertyId < y.PropertyId)
                    {
                        return 1;
                    }
                    return 0;
                case CustomSortOrder.DescendingByName:
                    return (String.Compare(y.DisplayName, x.DisplayName, true));
            }
            return 0;
        }

        public CustomSortOrder SortOrder { get; set; } = CustomSortOrder.AscendingByName;
        
        #endregion
    }

    internal class CategorySorter : IComparer<CustomPropertyDescriptor>
    {
        #region IComparer<PropertyDescriptor> Members

        public int Compare(CustomPropertyDescriptor x, CustomPropertyDescriptor y)
        {
            x.TabAppendCount = 0;
            y.TabAppendCount = 0;
            switch (SortOrder)
            {
                case CustomSortOrder.AscendingById:
                    if (x.CategoryId > y.CategoryId)
                    {
                        return 1;
                    }
                    else if (x.CategoryId < y.CategoryId)
                    {
                        return -1;
                    }
                    return 0;
                case CustomSortOrder.AscendingByName:
                    return (String.Compare(x.Category, y.Category, true));
                case CustomSortOrder.DescendingById:
                    if (x.CategoryId > y.CategoryId)
                    {
                        return -1;
                    }
                    else if (x.CategoryId < y.CategoryId)
                    {
                        return 1;
                    }
                    return 0;
                case CustomSortOrder.DescendingByName:
                    return (String.Compare(y.Category, x.Category, true));
            }
            return 0;
        }

        public CustomSortOrder SortOrder { get; set; } = CustomSortOrder.AscendingByName;
        #endregion
    }

    public class DynamicCustomTypeDescriptor : CustomTypeDescriptor
    {
        private PropertyDescriptorList m_pdl = new PropertyDescriptorList();
        private object m_instance;
        private Hashtable m_hashRM = new Hashtable();
        public DynamicCustomTypeDescriptor(ICustomTypeDescriptor ctd, object instance)
          : base(ctd)
        {
            m_instance = instance;
            GetProperties();
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {

            List<CustomPropertyDescriptor> pdl = m_pdl.FindAll(pd => pd.Attributes.Contains(attributes));

            PreProcess(pdl);
            PropertyDescriptorCollection pdcReturn = new PropertyDescriptorCollection(pdl.ToArray());

            return pdcReturn;
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            if (m_pdl.Count == 0)
            {
                PropertyDescriptorCollection pdc = base.GetProperties();  // this gives us a readonly collection, no good    
                foreach (PropertyDescriptor pd in pdc)
                {
                    if (!(pd is CustomPropertyDescriptor))
                    {
                        CustomPropertyDescriptor cpd = new CustomPropertyDescriptor(base.GetPropertyOwner(pd), pd);
                        m_pdl.Add(cpd);
                    }
                }
            }

            List<CustomPropertyDescriptor> pdl = m_pdl.FindAll(pd => pd != null);

            PreProcess(pdl);
            PropertyDescriptorCollection pdcReturn = new PropertyDescriptorCollection(m_pdl.ToArray());

            return pdcReturn;
        }

        private void PreProcess(List<CustomPropertyDescriptor> pdl)
        {
            if (PropertySortOrder != CustomSortOrder.None && pdl.Count > 0)
            {
                PropertySorter propSorter = new PropertySorter();
                propSorter.SortOrder = PropertySortOrder;
                pdl.Sort(propSorter);
            }
            UpdateCategoryTabAppendCount();
            UpdateResourceManager();
        }

        public CustomSortOrder PropertySortOrder { get; set; } = CustomSortOrder.AscendingById;

        public CustomSortOrder CategorySortOrder { get; set; } = CustomSortOrder.AscendingById;

        private void UpdateResourceManager()
        {
            foreach (CustomPropertyDescriptor cpd in m_pdl)
            {
                IResourceAttribute attr = (PropertyResourceAttribute)cpd.AllAttributes.FirstOrDefault(a => a is PropertyResourceAttribute);
                if (attr == null)
                {
                    AttributeCollection ac = GetAttributes();
                    AttributeList al = new AttributeList(ac);
                    attr = (ClassResourceAttribute)al.FirstOrDefault(a => a is ClassResourceAttribute);
                }
                if (attr == null)
                {
                    cpd.ResourceManager = null;
                    continue;
                }
                cpd.KeyPrefix = attr.KeyPrefix;
                if (m_hashRM[attr] is ResourceManager rm)
                {
                    cpd.ResourceManager = rm;
                    continue;
                }
                try
                {
                    if (String.IsNullOrEmpty(attr.AssemblyFullName) == false)
                    {
                        rm = new ResourceManager(attr.BaseName, Assembly.ReflectionOnlyLoad(attr.AssemblyFullName));
                    }
                    else
                    {
                        rm = new ResourceManager(attr.BaseName, base.GetPropertyOwner(cpd).GetType().Assembly);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
                m_hashRM.Add(attr, rm);
                cpd.ResourceManager = rm;
            }
        }
        private void UpdateCategoryTabAppendCount()
        {
            // get a copy of the list as we do not want to sort around the actual list
            List<CustomPropertyDescriptor> pdl = m_pdl.FindAll(pd => pd != null);
            if (pdl.Count == 0)
            {
                return;
            }
            CategorySorter propSorter = new CategorySorter();

            int nTabCount = 0;

            switch (CategorySortOrder)
            {
                case CustomSortOrder.AscendingById:
                    propSorter.SortOrder = CustomSortOrder.DescendingById;
                    pdl.Sort(propSorter);
                    nTabCount = 0;
                    int sortIndex = pdl[0].CategoryId;
                    foreach (CustomPropertyDescriptor cpd in pdl)
                    {
                        if (cpd.CategoryId == sortIndex)
                        {
                            cpd.TabAppendCount = nTabCount;
                        }
                        else
                        {
                            sortIndex = cpd.CategoryId;
                            nTabCount++;
                            cpd.TabAppendCount = nTabCount;
                        }
                    }
                    break;
                case CustomSortOrder.None:
                case CustomSortOrder.AscendingByName:  // by default, property grid sorts the category ascendingly
                    foreach (CustomPropertyDescriptor cpd in m_pdl)
                    {
                        cpd.TabAppendCount = 0;
                    }
                    break;
                case CustomSortOrder.DescendingById:
                    propSorter.SortOrder = CustomSortOrder.AscendingById;
                    pdl.Sort(propSorter);
                    nTabCount = 0;
                    int nCategorySortIndex = pdl[0].CategoryId;
                    foreach (CustomPropertyDescriptor cpd in pdl)
                    {
                        if (nCategorySortIndex == cpd.CategoryId)
                        {
                            cpd.TabAppendCount = nTabCount;
                        }
                        else
                        {
                            nCategorySortIndex = cpd.CategoryId;
                            nTabCount++;
                            cpd.TabAppendCount = nTabCount;
                        }
                    }
                    break;
                case CustomSortOrder.DescendingByName:
                    propSorter.SortOrder = CustomSortOrder.AscendingByName;
                    pdl.Sort(propSorter);
                    nTabCount = 0;
                    pdl[0].TabAppendCount = 0;
                    string sCat = pdl[0].Category;
                    foreach (CustomPropertyDescriptor cpd in pdl)
                    {
                        cpd.TabAppendCount = 0;
                        if (String.Compare(sCat, cpd.Category) == 0)
                        {
                            cpd.TabAppendCount = nTabCount;
                        }
                        else
                        {
                            sCat = cpd.Category;
                            nTabCount++;
                            cpd.TabAppendCount = nTabCount;
                        }
                    }
                    break;
            }
        }

        //private ISite m_site = null;
        //public ISite GetSite()
        //{
        //    if (m_site == null)
        //    {
        //        SimpleSite site = new SimpleSite();
        //        IPropertyValueUIService service = new PropertyValueUIService();
        //        service.AddPropertyValueUIHandler(new PropertyValueUIHandler(this.GenericPropertyValueUIHandler));
        //        site.AddService(service);
        //        m_site = site;
        //    }
        //    return m_site;
        //}

        private void GenericPropertyValueUIHandler(ITypeDescriptorContext context, PropertyDescriptor propDesc, ArrayList itemList)
        {
            if (propDesc is CustomPropertyDescriptor cpd)
            {
                itemList.AddRange(cpd.StateItems as ICollection);
            }
        }

        public CustomPropertyDescriptor GetProperty(string propertyName)
        {
            CustomPropertyDescriptor cpd = m_pdl.FirstOrDefault(a => String.Compare(a.Name, propertyName, true) == 0);
            return cpd;
        }
        public CustomPropertyDescriptor CreateProperty(string name, Type type, object value, int index, params Attribute[] attributes)
        {
            CustomPropertyDescriptor cpd = new CustomPropertyDescriptor(m_instance, name, type, value, attributes);
            if (index == -1)
            {
                m_pdl.Add(cpd);
            }
            else
            {
                m_pdl.Insert(index, cpd);
            }
            TypeDescriptor.Refresh(m_instance);
            return cpd;
        }
        public bool RemoveProperty(string propertyName)
        {
            CustomPropertyDescriptor cpd = m_pdl.FirstOrDefault(a => String.Compare(a.Name, propertyName, true) == 0);
            bool bReturn = m_pdl.Remove(cpd);
            TypeDescriptor.Refresh(m_instance);
            return bReturn;
        }
        public void ResetProperties()
        {
            m_pdl.Clear();
            GetProperties();
        }
    }

    internal class CustomTypeDescriptionProvider : TypeDescriptionProvider
    {
        private TypeDescriptionProvider m_parent;
        private ICustomTypeDescriptor m_ctd;

        public CustomTypeDescriptionProvider()
        {

        }

        public CustomTypeDescriptionProvider(TypeDescriptionProvider parent)
          : base(parent)
        {
            m_parent = parent;
        }
        public CustomTypeDescriptionProvider(TypeDescriptionProvider parent, ICustomTypeDescriptor ctd)
          : base(parent)
        {
            m_parent = parent;
            m_ctd = ctd;
        }
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return m_ctd;
        }


    }

    public static class ProviderInstaller
    {
        public static DynamicCustomTypeDescriptor Install(object instance)
        {
            TypeDescriptionProvider parentProvider = TypeDescriptor.GetProvider(instance);
            ICustomTypeDescriptor parentCtd = parentProvider.GetTypeDescriptor(instance);
            DynamicCustomTypeDescriptor ourCtd = new DynamicCustomTypeDescriptor(parentCtd, instance);
            CustomTypeDescriptionProvider ourProvider = new CustomTypeDescriptionProvider(parentProvider, ourCtd);
            TypeDescriptor.AddProvider(ourProvider, instance);
            return ourCtd;
        }
    }

    public class CustomPropertyDescriptor : PropertyDescriptor
    {
        internal object _owner;
        private readonly Type _propType = Type.Missing.GetType();
        private readonly AttributeList _attributes = new AttributeList();
        private readonly PropertyDescriptor _pd;
        private readonly Collection<PropertyValueUIItem> _colUiItem = new Collection<PropertyValueUIItem>();

        internal CustomPropertyDescriptor(object owner, string sName, Type type, object value, params Attribute[] attributes)
          : base(sName, attributes)
        {
            _owner = owner;
            _value = value;
            _propType = type;
            _attributes.AddRange(attributes);

            UpdateMemberData();
        }

        internal CustomPropertyDescriptor(object owner, PropertyDescriptor pd)
          : base(pd)
        {
            _pd = pd;
            _owner = owner;
            _attributes = new AttributeList(pd.Attributes);
            UpdateMemberData();
        }
        public override TypeConverter Converter
        {
            get
            {
                var tca = (TypeConverterAttribute)_attributes.FirstOrDefault(a => a is TypeConverterAttribute);

                if (tca != null)
                {
                    return base.Converter;
                }

                if (StandardValues.Count > 0)
                {
                    return new StandardValuesConverter();
                }

                if (GetValue(_owner) is IEnumerable en && (PropertyFlags & PropertyFlags.ExpandIEnumerable) > 0)
                {
                    return new StandardValuesConverter();
                }
                return base.Converter;

            }
        }
        private void UpdateMemberData()
        {

            if (_pd != null)
            {
                _value = _pd.GetValue(_owner);
            }

            if (PropertyType.IsEnum)
            {
                StandardValueAttribute[] sva = StandardValueAttribute.GetEnumItems(PropertyType);
                _standardValues.AddRange(sva);
            }
            else if (PropertyType == typeof(bool))
            {
                _standardValues.Add(new StandardValueAttribute(true));
                _standardValues.Add(new StandardValueAttribute(false));
            }
        }

        public override Type ComponentType
        {
            get
            {
                return _owner.GetType();
            }
        }
        public override Type PropertyType
        {
            get
            {
                if (_pd != null)
                {
                    return _pd.PropertyType;
                }
                return _propType;
            }
        }

        protected override Attribute[] AttributeArray
        {
            get => _attributes.ToArray();
            set
            {
                _attributes.Clear();
                _attributes.AddRange(value);
            }
        }

        public override AttributeCollection Attributes
        {
            get
            {
                var ac = new AttributeCollection(_attributes.ToArray());
                return ac;
            }
        }
        protected override void FillAttributes(IList attributeList)
        {
            foreach (Attribute attr in _attributes)
            {
                attributeList.Add(attr);
            }
        }
        public IList<Attribute> AllAttributes
        {
            get
            {
                return _attributes;
            }
        }
        /// <summary>
        /// Must override abstract properties.
        /// </summary>
        /// 

        public override bool IsLocalizable
        {
            get
            {
                LocalizableAttribute attr = (LocalizableAttribute)_attributes.FirstOrDefault(a => a is LocalizableAttribute);
                if (attr != null)
                {
                    return attr.IsLocalizable;
                }
                return base.IsLocalizable;
            }
        }
        public void SetIsLocalizable(bool isLocalizable)
        {
            LocalizableAttribute attr = (LocalizableAttribute)_attributes.FirstOrDefault(a => a is LocalizableAttribute);
            if (attr != null)
            {
                _attributes.RemoveAll(a => a is LocalizableAttribute);
            }
            attr = new LocalizableAttribute(isLocalizable);
            _attributes.Add(attr);
        }
        public override bool IsReadOnly
        {
            get
            {
                ReadOnlyAttribute attr = (ReadOnlyAttribute)_attributes.FirstOrDefault(a => a is ReadOnlyAttribute);
                if (attr != null)
                {
                    return attr.IsReadOnly;
                }
                return false;
            }
        }
        public void SetIsReadOnly(bool isReadOnly)
        {
            ReadOnlyAttribute attr = (ReadOnlyAttribute)_attributes.FirstOrDefault(a => a is ReadOnlyAttribute);
            if (attr != null)
            {
                _attributes.RemoveAll(a => a is ReadOnlyAttribute);
            }
            attr = new ReadOnlyAttribute(isReadOnly);
            _attributes.Add(attr);
        }
        public override bool IsBrowsable
        {
            get
            {
                BrowsableAttribute attr = (BrowsableAttribute)_attributes.FirstOrDefault(a => a is BrowsableAttribute);
                if (attr != null)
                {
                    return attr.Browsable;
                }
                return base.IsBrowsable;
            }
        }
        public void SetIsBrowsable(bool isBrowsable)
        {
            BrowsableAttribute attr = (BrowsableAttribute)_attributes.FirstOrDefault(a => a is BrowsableAttribute);
            if (attr != null)
            {
                _attributes.RemoveAll(a => a is BrowsableAttribute);
            }
            attr = new BrowsableAttribute(isBrowsable);
            _attributes.Add(attr);
        }

        internal string KeyPrefix { get; set; } = string.Empty;

        public override string DisplayName
        {
            get
            {

                if (ResourceManager != null && (PropertyFlags & PropertyFlags.LocalizeDisplayName) > 0)
                {
                    string sKey = KeyPrefix + base.Name + "_Name";

                    string sResult = ResourceManager.GetString(sKey, CultureInfo.CurrentUICulture);
                    if (!String.IsNullOrEmpty(sResult))
                    {
                        return sResult;
                    }
                }
                DisplayNameAttribute attr = (DisplayNameAttribute)_attributes.FirstOrDefault(a => a is DisplayNameAttribute);
                if (attr != null)
                {
                    return attr.DisplayName;
                }
                return base.DisplayName;
            }
        }
        public void SetDisplayName(string displayName)
        {
            DisplayNameAttribute attr = (DisplayNameAttribute)_attributes.FirstOrDefault(a => a is DisplayNameAttribute);
            if (attr != null)
            {
                _attributes.RemoveAll(a => a is DisplayNameAttribute);
            }
            attr = new DisplayNameAttribute(displayName);
            _attributes.Add(attr);
        }
        public override string Category
        {
            get
            {
                string sResult = string.Empty;
                if (ResourceManager != null && CategoryId != 0 && (PropertyFlags & PropertyFlags.LocalizeCategoryName) > 0)
                {
                    string sKey = KeyPrefix + "Cat" + CategoryId;
                    sResult = ResourceManager.GetString(sKey, CultureInfo.CurrentUICulture);
                    if (!String.IsNullOrEmpty(sResult))
                    {
                        return sResult.PadLeft(sResult.Length + TabAppendCount, '\t');
                    }

                }
                var attr = (CategoryAttribute)_attributes.FirstOrDefault(a => a is CategoryAttribute);
                if (attr != null)
                {
                    sResult = attr.Category;
                }
                if (string.IsNullOrEmpty(sResult))
                {
                    sResult = base.Category;
                }
                return sResult?.PadLeft(base.Category.Length + TabAppendCount, '\t');
            }
        }
        public void SetCategory(string category)
        {
            var attr = (CategoryAttribute)_attributes.FirstOrDefault(a => a is CategoryAttribute);
            if (attr != null)
            {
                _attributes.RemoveAll(a => a is CategoryAttribute);
            }
            attr = new CategoryAttribute(category);
            _attributes.Add(attr);
        }
        public override string Description
        {
            get
            {
                if (ResourceManager != null && (PropertyFlags & PropertyFlags.LocalizeDescription) > 0)
                {
                    string sKey = KeyPrefix + base.Name + "_Desc";
                    string sResult = ResourceManager.GetString(sKey, CultureInfo.CurrentUICulture);
                    if (!String.IsNullOrEmpty(sResult))
                    {
                        return sResult;
                    }
                }
                var attr = (DescriptionAttribute)_attributes.FirstOrDefault(a => a is DescriptionAttribute);
                return attr != null ? attr.Description : base.Description;
            }
        }
        public void SetDescription(string description)
        {
            var attr = (DescriptionAttribute)_attributes.FirstOrDefault(a => a is DescriptionAttribute);
            if (attr != null)
            {
                _attributes.RemoveAll(a => a is DescriptionAttribute);
            }
            attr = new DescriptionAttribute(description);
            _attributes.Add(attr);
        }

        public object DefaultValue
        {
            get
            {
                var attr = (DefaultValueAttribute)_attributes.FirstOrDefault(a => a is DefaultValueAttribute);
                if (attr != null)
                {
                    return attr.Value;
                }
                return null;
            }
            set
            {
                var attr = (DefaultValueAttribute)_attributes.FirstOrDefault(a => a is DefaultValueAttribute);
                if (attr == null)
                {
                    _attributes.RemoveAll(a => a is DefaultValueAttribute);
                }
                attr = new DefaultValueAttribute(value);
            }
        }

        public int PropertyId
        {
            get
            {
                var rsa = (IdAttribute)_attributes.FirstOrDefault(a => a is IdAttribute);
                if (rsa != null)
                {
                    return rsa.PropertyId;
                }
                return 0;
            }
            set
            {
                var rsa = (IdAttribute)_attributes.FirstOrDefault(a => a is IdAttribute);
                if (rsa == null)
                {
                    rsa = new IdAttribute();
                    _attributes.Add(rsa);
                }
                rsa.PropertyId = value;
            }
        }
        public int CategoryId
        {
            get
            {
                var rsa = (IdAttribute)_attributes.FirstOrDefault(a => a is IdAttribute);
                if (rsa != null)
                {
                    return rsa.CategoryId;
                }
                return 0;
            }
            set
            {
                var rsa = (IdAttribute)_attributes.FirstOrDefault(a => a is IdAttribute);
                if (rsa == null)
                {
                    rsa = new IdAttribute();
                    _attributes.Add(rsa);
                }
                rsa.CategoryId = value;
            }
        }

        internal int TabAppendCount { get; set; }

        internal ResourceManager ResourceManager { get; set; }

        private object _value;
        public override object GetValue(object component)
        {
            if (_pd != null)
            {
                return _pd.GetValue(component);
            }
            return _value;
        }

        public override void SetValue(object component, object value)
        {
            if (value is StandardValueAttribute)
            {
                _value = (value as StandardValueAttribute).Value;
            }
            else
            {
                _value = value;
            }

            if (_pd != null)
            {
                _pd.SetValue(component, _value);
                OnValueChanged(this, new EventArgs());

            }
            else
            {
                var eh = GetValueChangedHandler(_owner);
                if (eh != null)
                {
                    eh.Invoke(this, new EventArgs());
                }
                OnValueChanged(this, new EventArgs());
            }
        }
        protected override void OnValueChanged(object component, EventArgs e)
        {
            var md = component as MemberDescriptor;

            base.OnValueChanged(component, e);
        }

        /// <summary>
        /// Abstract base members
        /// </summary>			
        public override void ResetValue(object component)
        {
            var dva = (DefaultValueAttribute)_attributes.FirstOrDefault(a => a is DefaultValueAttribute);
            if (dva == null)
            {
                return;
            }
            SetValue(component, dva.Value);
        }

        public override bool CanResetValue(object component)
        {
            var dva = (DefaultValueAttribute)_attributes.FirstOrDefault(a => a is DefaultValueAttribute);
            if (dva == null)
            {
                return false;
            }
            bool bOk = (dva.Value.Equals(_value));
            return !bOk;

        }

        public override bool ShouldSerializeValue(object component)
        {
            return CanResetValue(_owner);
        }

        public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter)
        {
            PropertyDescriptorCollection pdc = null;
            TypeConverter tc = Converter;
            if (tc.GetPropertiesSupported(null) == false)
            {
                pdc = base.GetChildProperties(instance, filter);
            }

            if (_pd != null)
            {
                tc = _pd.Converter;
            }
            else
            {
                //pdc = base.GetChildProperties(instance, filter);// this gives us a readonly collection, no good    
                tc = TypeDescriptor.GetConverter(instance, true);
            }
            if (pdc == null || pdc.Count == 0)
            {
                return pdc;
            }
            if (pdc[0] is CustomPropertyDescriptor)
            {
                return pdc;
            }
            // now wrap these properties with our CustomPropertyDescriptor
            var pdl = new PropertyDescriptorList();

            foreach (PropertyDescriptor pd in pdc)
            {
                if (pd is CustomPropertyDescriptor)
                {
                    pdl.Add(pd as CustomPropertyDescriptor);
                }
                else
                {
                    pdl.Add(new CustomPropertyDescriptor(instance, pd));
                }
            }

            pdl.Sort(new PropertySorter());
            PropertyDescriptorCollection pdcReturn = new PropertyDescriptorCollection(pdl.ToArray());
            pdcReturn.Sort();
            return pdcReturn;

        }

        public ICollection<PropertyValueUIItem> StateItems
        {
            get
            {
                return _colUiItem;
            }
        }

        private List<StandardValueAttribute> _standardValues = new List<StandardValueAttribute>();
        public ICollection<StandardValueAttribute> StandardValues
        {
            get
            {
                if (PropertyType.IsEnum || PropertyType == typeof(bool))
                {
                    return _standardValues.AsReadOnly();
                }
                return _standardValues;
            }
        }

        public Image ValueImage { get; set; } = null;

        public PropertyFlags PropertyFlags
        {
            get
            {
                PropertyStateFlagsAttribute attr = (PropertyStateFlagsAttribute)_attributes.FirstOrDefault(a => a is PropertyStateFlagsAttribute);
                if (attr == null)
                {
                    attr = new PropertyStateFlagsAttribute();
                    _attributes.Add(attr);
                    attr.Flags = PropertyFlags.Default;
                }

                return attr.Flags;
            }
            set
            {
                PropertyStateFlagsAttribute attr = (PropertyStateFlagsAttribute)_attributes.FirstOrDefault(a => a is PropertyStateFlagsAttribute);
                if (attr == null)
                {
                    attr = new PropertyStateFlagsAttribute();
                    _attributes.Add(attr);
                    attr.Flags = PropertyFlags.Default;
                }
                attr.Flags = value;

            }
        }

    }
}