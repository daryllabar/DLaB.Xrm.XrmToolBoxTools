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
                { nameof(AddNewFilesToProject), OnAddNewFilesToProjectChange },
                { nameof(CreateOneFilePerAction), OnCreateOneFilePerActionChange },
                { nameof(CreateOneFilePerEntity), OnCreateOneFilePerEntityChange },
                { nameof(CreateOneFilePerOptionSet), OnCreateOneFilePerOptionSetChange },
                { nameof(GenerateEnumProperties), OnGenerateEnumPropertiesChange },
                { nameof(IncludeCommandLine), OnIncludeCommandLineChange },
                { nameof(UseDeprecatedOptionSetNaming), OnUseDeprecatedOptionSetNamingChange },
            };
        }

        private void OnAddNewFilesToProjectChange(PropertyValueChangedEventArgs args)
        {
            SetProjectNameForEarlyBoundFilesVisibility();
        }

        private void OnCreateOneFilePerActionChange(PropertyValueChangedEventArgs args)
        {
            SetVisibilityForControlsDependentOnFileCreations();
            ActionOutPath = ActionOutPath;
        }

        private void OnCreateOneFilePerEntityChange(PropertyValueChangedEventArgs args)
        {
            SetVisibilityForControlsDependentOnFileCreations();
            EntityOutPath = EntityOutPath;
        }

        private void OnCreateOneFilePerOptionSetChange(PropertyValueChangedEventArgs args)
        {
            SetVisibilityForControlsDependentOnFileCreations();
            OptionSetOutPath = OptionSetOutPath;
        }

        private void OnGenerateEnumPropertiesChange(PropertyValueChangedEventArgs args)
        {
            SetPropertyEnumMappingVisibility();
            SetPropertyReplaceOptionSetPropertiesWithEnumVisibility();
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
            SetVisibilityForControlsDependentOnFileCreations();
            SetMaskPasswordVisibility();
            SetPropertyEnumMappingVisibility();
            SetPropertyReplaceOptionSetPropertiesWithEnumVisibility();
            SetUnmappedPropertiesVisibility();
            SetLocalOptionSetFormatVisibility();
            ActionOutPath = ActionOutPath;
            EntityOutPath = EntityOutPath;
            OptionSetOutPath = OptionSetOutPath;
            TypeDescriptor.Refresh(this);
        }

        private void SetVisibilityForControlsDependentOnFileCreations()
        {
            SetAddFilesToProjectVisibility();
            SetDeleteFilesFromOutputFoldersVisibility();
            SetProjectNameForEarlyBoundFilesVisibility();
        }

        private void SetAddFilesToProjectVisibility()
        {
            var prop = Descriptor.GetProperty(nameof(AddNewFilesToProject));
            prop.SetIsBrowsable(AtLeastOneCreateFilePerSelected);
        }

        private void SetDeleteFilesFromOutputFoldersVisibility()
        {
            var prop = Descriptor.GetProperty(nameof(DeleteFilesFromOutputFolders));
            prop.SetIsBrowsable(AtLeastOneCreateFilePerSelected);
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

        private void SetProjectNameForEarlyBoundFilesVisibility()
        {
            var parentProp = Descriptor.GetProperty(nameof(AddNewFilesToProject));
            var prop = Descriptor.GetProperty(nameof(ProjectNameForEarlyBoundFiles));
            prop.SetIsBrowsable(parentProp.IsBrowsable && AddNewFilesToProject);
        }

        private void SetPropertyEnumMappingVisibility()
        {
            var prop = Descriptor.GetProperty(nameof(PropertyEnumMappings));
            prop.SetIsBrowsable(GenerateEnumProperties);
        }

        private void SetPropertyReplaceOptionSetPropertiesWithEnumVisibility()
        {
            var prop = Descriptor.GetProperty(nameof(ReplaceOptionSetPropertiesWithEnum));
            prop.SetIsBrowsable(GenerateEnumProperties);
        }

        private void SetUnmappedPropertiesVisibility()
        {
            var prop = Descriptor.GetProperty(nameof(UnmappedProperties));
            prop.SetIsBrowsable(GenerateEnumProperties);
        }
    }
}
