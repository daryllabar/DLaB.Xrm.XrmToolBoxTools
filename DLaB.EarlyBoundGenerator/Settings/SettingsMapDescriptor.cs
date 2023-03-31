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
                { nameof(CreateOneFilePerAction), OnCreateOneFilePerMessageChange },
                { nameof(CreateOneFilePerEntity), OnCreateOneFilePerEntityChange },
                { nameof(CreateOneFilePerOptionSet), OnCreateOneFilePerOptionSetChange },
                { nameof(DeleteFilesFromOutputFolders), OnDeleteFilesFromOutputFoldersChange },
                { nameof(GenerateEnumProperties), OnGenerateEnumPropertiesChange },
                { nameof(AddOptionSetMetadataAttribute), OnAddOptionSetMetadataAttributeChange },
                { nameof(MakeAllFieldsEditable), OnMakeAllFieldsEditableChange },
            };
        }

        private void OnAddNewFilesToProjectChange(PropertyValueChangedEventArgs args)
        {
            SetProjectNameForEarlyBoundFilesVisibility();
        }

        private void OnCreateOneFilePerEntityChange(PropertyValueChangedEventArgs args)
        {
            SetVisibilityForControlsDependentOnFileCreations();
            EntityTypesFolder = EntityTypesFolder;
        }

        private void OnCreateOneFilePerOptionSetChange(PropertyValueChangedEventArgs args)
        {
            SetVisibilityForControlsDependentOnFileCreations();
            SetGroupLocalOptionSetsByEntityVisibility();
            OptionSetsTypesFolder = OptionSetsTypesFolder;
        }

        private void OnCreateOneFilePerMessageChange(PropertyValueChangedEventArgs args)
        {
            SetVisibilityForControlsDependentOnFileCreations();
            SetGroupMessageRequestWithResponseVisibility();
            MessageTypesFolder = MessageTypesFolder;
        }

        private void OnDeleteFilesFromOutputFoldersChange(PropertyValueChangedEventArgs args)
        {
            SetCleanupCrmSvcUtilLocalOptionSets();
        }

        private void OnGenerateEnumPropertiesChange(PropertyValueChangedEventArgs args)
        {
            SetPropertyEnumMappingVisibility();
            SetPropertyReplaceOptionSetPropertiesWithEnumVisibility();
            SetUnmappedPropertiesVisibility();
        }

        private void OnAddOptionSetMetadataAttributeChange(PropertyValueChangedEventArgs args)
        {
            SetGenerateOptionSetMetadataAttributeVisibility();
            GenerateOptionSetMetadataAttribute = AddOptionSetMetadataAttribute;
        }

        private void OnMakeAllFieldsEditableChange(PropertyValueChangedEventArgs args)
        {
            SetMakeReadonlyFieldsEditableVisibility();
        }

        #endregion OnChange Handlers

        private void ProcessDynamicallyVisibleProperties()
        {
            SetVisibilityForControlsDependentOnFileCreations();
            SetCleanupCrmSvcUtilLocalOptionSets();
            SetGroupLocalOptionSetsByEntityVisibility();
            SetGroupMessageRequestWithResponseVisibility();
            SetPropertyEnumMappingVisibility();
            SetPropertyReplaceOptionSetPropertiesWithEnumVisibility();
            SetUnmappedPropertiesVisibility();
            SetGenerateOptionSetMetadataAttributeVisibility();
            MessageTypesFolder = MessageTypesFolder;
            EntityTypesFolder = EntityTypesFolder;
            OptionSetsTypesFolder = OptionSetsTypesFolder;
            TypeDescriptor.Refresh(this);
        }

        private void DisableUnsupportedProperties()
        {
            var disabledProperties = new[]
            {
                nameof(ActionPrefixesWhitelist),
                nameof(AddDebuggerNonUserCode),
                nameof(AudibleCompletionNotification),
                nameof(EntityPrefixesToSkip),
                nameof(EntityPrefixesWhitelist),
                nameof(FilePrefixText),
                nameof(GenerateEntityRelationships),
                nameof(GenerateEntityTypeCode),
                nameof(GenerateEnumProperties),
                nameof(GenerateOptionSetMetadataAttribute),
                nameof(IncludeCommandLine),
                nameof(OptionSetPrefixesToSkip),
                nameof(OptionSetsToSkip),
                nameof(ProjectNameForEarlyBoundFiles),
                nameof(RemoveRuntimeVersionComment),
                nameof(UnmappedProperties),
                nameof(UseTfsToCheckoutFiles),
                nameof(WorkflowlessActions),
            };

            foreach (var property in disabledProperties)
            {
                SetPropertyDisabled(property, true);
            }
        }

        private void SetVisibilityForControlsDependentOnFileCreations()
        {
            SetAddFilesToProjectVisibility();
            SetDeleteFilesFromOutputFoldersVisibility();
            SetProjectNameForEarlyBoundFilesVisibility();
        }

        private void SetAddFilesToProjectVisibility()
        {
            SetPropertyBrowsable(nameof(AddNewFilesToProject), AtLeastOneCreateFilePerSelected);
        }

        private void SetCleanupCrmSvcUtilLocalOptionSets()
        {
            SetPropertyBrowsable(nameof(CleanupCrmSvcUtilLocalOptionSets), !DeleteFilesFromOutputFolders);
        }

        private void SetDeleteFilesFromOutputFoldersVisibility()
        {
            SetPropertyBrowsable(nameof(DeleteFilesFromOutputFolders), AtLeastOneCreateFilePerSelected);
        }

        private void SetGroupLocalOptionSetsByEntityVisibility()
        {
            SetPropertyBrowsable(nameof(GroupLocalOptionSetsByEntity), CreateOneFilePerOptionSet);
        }

        private void SetGroupMessageRequestWithResponseVisibility()
        {
            SetPropertyBrowsable(nameof(GroupMessageRequestWithResponse), CreateOneFilePerAction);
        }

        private void SetMakeReadonlyFieldsEditableVisibility()
        {
            SetPropertyBrowsable(nameof(MakeReadonlyFieldsEditable), MakeAllFieldsEditable);
        }

        private void SetProjectNameForEarlyBoundFilesVisibility()
        {
            var parentProp = Descriptor.GetProperty(nameof(AddNewFilesToProject));
            SetPropertyBrowsable(nameof(ProjectNameForEarlyBoundFiles), parentProp.IsBrowsable && AddNewFilesToProject);
        }

        private void SetPropertyEnumMappingVisibility()
        {
            SetPropertyBrowsable(nameof(PropertyEnumMappings), GenerateEnumProperties);
        }

        private void SetPropertyReplaceOptionSetPropertiesWithEnumVisibility()
        {
            SetPropertyBrowsable(nameof(ReplaceOptionSetPropertiesWithEnum), GenerateEnumProperties);
        }
        
        private void SetGenerateOptionSetMetadataAttributeVisibility()
        {
            SetPropertyBrowsable(nameof(GenerateOptionSetMetadataAttribute), AddOptionSetMetadataAttribute);
        }

        private void SetUnmappedPropertiesVisibility()
        {
            SetPropertyBrowsable(nameof(UnmappedProperties), GenerateEnumProperties);
        }

        private void SetPropertyBrowsable(string propertyName, bool browsable)
        {
            var prop = Descriptor.GetProperty(propertyName);
            prop.SetIsBrowsable(browsable);
        }

        private const string NotImplemented = "{Not Implemented} ";

        private void SetPropertyDisabled(string propertyName, bool disabled)
        {
            var prop = Descriptor.GetProperty(propertyName);
            if (disabled)
            {
                prop.SetDisplayName(NotImplemented + prop.DisplayName);
            }
            else
            {
                prop.SetDisplayName(prop.DisplayName.Replace(NotImplemented, string.Empty));
            }
            prop.SetIsReadOnly(disabled);
        }
    }
}
