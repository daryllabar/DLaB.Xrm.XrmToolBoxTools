using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DLaB.XrmToolBoxCommon;

namespace DLaB.EarlyBoundGenerator.Settings
{
    public partial class SettingsMap 
    {
        [Browsable(false)]
        public DynamicCustomTypeDescriptor Descriptor { get; set; }
        private void SetupCustomTypeDescriptor()
        {
            Descriptor = ProviderInstaller.Install(this);
        }


        #region OnChange Handlers

        private Dictionary<string, Action<PropertyValueChangedEventArgs>> OnChangeMap { get; }

        public void OnPropertyValueChanged(object o, PropertyValueChangedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.ChangedItem.PropertyDescriptor?.Name)
                && OnChangeMap.TryGetValue(args.ChangedItem.PropertyDescriptor.Name, out var action))
            {
                action(args);
                TypeDescriptor.Refresh(this);
            }
        }

        private Dictionary<string, Action<PropertyValueChangedEventArgs>> GetOnChangeHandlers()
        {
            return new Dictionary<string, Action<PropertyValueChangedEventArgs>>
            {
                { nameof(CreateOneFilePerAction), OnCreateOneFilePerActionChange },
                { nameof(CreateOneFilePerEntity), OnCreateOneFilePerEntityChange },
                { nameof(CreateOneFilePerOptionSet), OnCreateOneFilePerOptionSetChange },
                { nameof(GenerateEnumProperties), OnGenerateEnumPropertiesChange },
                { nameof(IncludeCommandLine), OnIncludeCommandLineChange },
                { nameof(UseDeprecatedOptionSetNaming), OnUseDeprecatedOptionSetNamingChange },
            };
        }

        private void OnCreateOneFilePerActionChange(PropertyValueChangedEventArgs args)
        {
            SetAddFilesToProjectVisibility();
            ActionOutPath = ActionOutPath;
        }

        private void OnCreateOneFilePerEntityChange(PropertyValueChangedEventArgs args)
        {
            SetAddFilesToProjectVisibility();
            EntityOutPath = EntityOutPath;
        }

        private void OnCreateOneFilePerOptionSetChange(PropertyValueChangedEventArgs args)
        {
            SetAddFilesToProjectVisibility();
            OptionSetOutPath = OptionSetOutPath;
        }

        private void OnGenerateEnumPropertiesChange(PropertyValueChangedEventArgs args)
        {
            SetPropertyEnumMappingVisibility();
            SetUnmappedPropertiesVisibility();
        }

        private void OnIncludeCommandLineChange(PropertyValueChangedEventArgs args)
        {
            SetMaskPasswordVisibility();
        }

        private void OnUseDeprecatedOptionSetNamingChange(PropertyValueChangedEventArgs args)
        {
            SetLocalOptionSetFormatVisibility();
        }

        #endregion OnChange Handlers

        private void ProcessDynamicallyVisibleProperties()
        {
            SetAddFilesToProjectVisibility();
            SetMaskPasswordVisibility();
            SetPropertyEnumMappingVisibility();
            SetUnmappedPropertiesVisibility();
            SetLocalOptionSetFormatVisibility();
            ActionOutPath = ActionOutPath;
            EntityOutPath = EntityOutPath;
            OptionSetOutPath = OptionSetOutPath;
            TypeDescriptor.Refresh(this);
        }

        private void SetAddFilesToProjectVisibility()
        {
            var prop = Descriptor.GetProperty(nameof(AddNewFilesToProject));
            prop.SetIsBrowsable(CreateOneFilePerAction 
                                || CreateOneFilePerEntity
                                || CreateOneFilePerOptionSet);
        }

        private void SetMaskPasswordVisibility()
        {
            var prop = Descriptor.GetProperty(nameof(MaskPassword));
            prop.SetIsBrowsable(IncludeCommandLine);
        }

        private void SetLocalOptionSetFormatVisibility()
        {
            var prop = Descriptor.GetProperty(nameof(LocalOptionSetFormat));
            prop.SetIsBrowsable(!UseDeprecatedOptionSetNaming);
        }

        private void SetPropertyEnumMappingVisibility()
        {
            var prop = Descriptor.GetProperty(nameof(PropertyEnumMappings));
            prop.SetIsBrowsable(GenerateEnumProperties);
        }

        private void SetUnmappedPropertiesVisibility()
        {
            var prop = Descriptor.GetProperty(nameof(UnmappedProperties));
            prop.SetIsBrowsable(GenerateEnumProperties);
        }
    }
}
