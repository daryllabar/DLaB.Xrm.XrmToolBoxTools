using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DLaB.XrmToolBoxCommon;

namespace DLaB.EarlyBoundGeneratorV2.Settings
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
                { nameof(AddOptionSetMetadataAttribute), OnAddOptionSetMetadataAttributeChange },
                { nameof(CamelCaseClassNames), OnCamelCaseChange },
                { nameof(CamelCaseMemberNames), OnCamelCaseChange },
                { nameof(CreateOneFilePerEntity), OnCreateOneFilePerEntityChange },
                { nameof(CreateOneFilePerOptionSet), OnCreateOneFilePerOptionSetChange },
                { nameof(CreateOneFilePerMessage), OnCreateOneFilePerMessageChange },
                { nameof(DeleteFilesFromOutputFolders), OnDeleteFilesFromOutputFoldersChange },
                { nameof(GenerateEnumProperties), OnGenerateEnumPropertiesChange },
                { nameof(ReplaceOptionSetPropertiesWithEnum), OnReplaceOptionSetPropertiesWithEnumChange },
                { nameof(GenerateMessages), OnGenerateMessagesChange },
                { nameof(MakeAllFieldsEditable), OnMakeAllFieldsEditableChange },
            };
        }

        private void OnAddNewFilesToProjectChange(PropertyValueChangedEventArgs args)
        {
            SetProjectNameForEarlyBoundFilesVisibility();
        }

        private void OnAddOptionSetMetadataAttributeChange(PropertyValueChangedEventArgs args)
        {
            SetGenerateOptionSetMetadataAttributeVisibility();
            SetGenerateAllOptionSetLabelMetadataVisibility();
            GenerateOptionSetMetadataAttribute = AddOptionSetMetadataAttribute;
            GenerateAllOptionSetLabelMetadata = false;
        }

        /// <summary>
        /// Applies if either Camel Case Class Names or Camel Case Member Names is updated
        /// </summary>
        /// <param name="args"></param>
        private void OnCamelCaseChange(PropertyValueChangedEventArgs args)
        {
            SetVisibilityForControlsDependentOnCamelCasing();
        }

        private void OnCreateOneFilePerEntityChange(PropertyValueChangedEventArgs args)
        {
            SetVisibilityForControlsDependentOnFileCreations();
            // Convert the value to be a folder or a file
            EntityTypesFolder = EntityTypesFolder;
        }

        private void OnCreateOneFilePerOptionSetChange(PropertyValueChangedEventArgs args)
        {
            SetVisibilityForControlsDependentOnFileCreations();
            SetGroupLocalOptionSetsByEntityVisibility();
            // Convert the value to be a folder or a file
            OptionSetsTypesFolder = OptionSetsTypesFolder;
        }

        private void OnCreateOneFilePerMessageChange(PropertyValueChangedEventArgs args)
        {
            SetVisibilityForControlsDependentOnFileCreations();
            SetGroupMessageRequestWithResponseVisibility();
            // Convert the value to be a folder or a file
            MessageTypesFolder = MessageTypesFolder;
        }

        private void OnDeleteFilesFromOutputFoldersChange(PropertyValueChangedEventArgs args)
        {
            SetCleanupCrmSvcUtilLocalOptionSetsVisibility();
        }

        private void OnGenerateEnumPropertiesChange(PropertyValueChangedEventArgs args)
        {
            SetPropertyEnumMappingVisibility();
            SetReplaceOptionSetPropertiesWithEnumVisibility();
            SetUseEnumForStateCodesVisibility();
        }

        private void OnGenerateMessagesChange(PropertyValueChangedEventArgs args)
        {
            SetVisibilityForControlsDependentOnGenerateMessages();
        }

        private void OnMakeAllFieldsEditableChange(PropertyValueChangedEventArgs args)
        {
            SetMakeReadonlyFieldsEditableVisibility();
        }

        private void OnReplaceOptionSetPropertiesWithEnumChange(PropertyValueChangedEventArgs args)
        {
            SetUseEnumForStateCodesVisibility();
        }

        #endregion OnChange Handlers

        private void ProcessDynamicallyVisibleProperties()
        {
            SetCleanupCrmSvcUtilLocalOptionSetsVisibility();
            SetGenerateOptionSetMetadataAttributeVisibility();
            SetGroupLocalOptionSetsByEntityVisibility();
            SetGroupMessageRequestWithResponseVisibility();
            SetPropertyEnumMappingVisibility();
            SetReplaceOptionSetPropertiesWithEnumVisibility();
            SetUseLogicalNamesVisibility();
            SetVisibilityForControlsDependentOnCamelCasing();
            SetVisibilityForControlsDependentOnFileCreations();
            SetVisibilityForControlsDependentOnGenerateMessages();

            MessageTypesFolder = MessageTypesFolder;
            EntityTypesFolder = EntityTypesFolder;
            OptionSetsTypesFolder = OptionSetsTypesFolder;
            TypeDescriptor.Refresh(this);
        }

        private void SetVisibilityForControlsDependentOnCamelCasing()
        {
            SetCamelCaseCustomWordsVisibility();
            SetCamelCaseDictionaryRelativePathVisibility();
            SetUseLogicalNamesVisibility();
        }

        private void SetVisibilityForControlsDependentOnFileCreations()
        {
            SetAddFilesToProjectVisibility();
            SetDeleteFilesFromOutputFoldersVisibility();
            SetProjectNameForEarlyBoundFilesVisibility();
        }

        private void SetVisibilityForControlsDependentOnGenerateMessages()
        {
            SetCreateOneFilePerMessageVisibility();
            SetGenerateMessageAttributeNameConstsVisibility();
            SetGroupMessageRequestWithResponseVisibility();
            SetMakeResponseMessagesEditableVisibility();
            SetMessageBlacklistVisibility();
            SetMessageTypesFolderVisibility();
            SetMessageWhitelistVisibility();
            SetMessageWildcardWhitelistVisibility();
        }

        private void SetAddFilesToProjectVisibility()
        {
            SetPropertyBrowsable(nameof(AddNewFilesToProject), AtLeastOneCreateFilePerSelected);
        }

        private void SetCamelCaseCustomWordsVisibility()
        {
            SetPropertyBrowsable(nameof(CamelCaseCustomWords), CamelCaseClassNames || CamelCaseMemberNames);
        }

        private void SetCamelCaseDictionaryRelativePathVisibility()
        {
            SetPropertyBrowsable(nameof(CamelCaseNamesDictionaryRelativePath), CamelCaseClassNames || CamelCaseMemberNames);
        }

        private void SetCleanupCrmSvcUtilLocalOptionSetsVisibility()
        {
            SetPropertyBrowsable(nameof(CleanupCrmSvcUtilLocalOptionSets), !DeleteFilesFromOutputFolders);
        }

        private void SetCreateOneFilePerMessageVisibility()
        {
            SetPropertyBrowsable(nameof(CreateOneFilePerMessage), GenerateMessages);
        }
        
        private void SetDeleteFilesFromOutputFoldersVisibility()
        {
            SetPropertyBrowsable(nameof(DeleteFilesFromOutputFolders), AtLeastOneCreateFilePerSelected);
        }

        private void SetGenerateMessageAttributeNameConstsVisibility()
        {
            SetPropertyBrowsable(nameof(GenerateMessageAttributeNameConsts), GenerateMessages);
        }

        private void SetGenerateOptionSetMetadataAttributeVisibility()
        {
            SetPropertyBrowsable(nameof(GenerateOptionSetMetadataAttribute), AddOptionSetMetadataAttribute);
        }
        
        private void SetGenerateAllOptionSetLabelMetadataVisibility()
        {
            SetPropertyBrowsable(nameof(GenerateAllOptionSetLabelMetadata), AddOptionSetMetadataAttribute);
        }

        private void SetGroupLocalOptionSetsByEntityVisibility()
        {
            SetPropertyBrowsable(nameof(GroupLocalOptionSetsByEntity), CreateOneFilePerOptionSet);
        }

        private void SetGroupMessageRequestWithResponseVisibility()
        {
            SetPropertyBrowsable(nameof(GroupMessageRequestWithResponse), CreateOneFilePerMessage && GenerateMessages);
        }

        private void SetMakeReadonlyFieldsEditableVisibility()
        {
            SetPropertyBrowsable(nameof(MakeReadonlyFieldsEditable), MakeAllFieldsEditable);
        }

        private void SetMakeResponseMessagesEditableVisibility()
        {
            SetPropertyBrowsable(nameof(MakeResponseMessagesEditable), GenerateMessages);
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
        private void SetMessageBlacklistVisibility()
        {
            SetPropertyBrowsable(nameof(MessageBlacklist), GenerateMessages);
        }

        private void SetMessageTypesFolderVisibility()
        {
            SetPropertyBrowsable(nameof(MessageTypesFolder), GenerateMessages);
        }

        private void SetMessageWhitelistVisibility()
        {
            SetPropertyBrowsable(nameof(MessageWhitelist), GenerateMessages);
        }

        private void SetMessageWildcardWhitelistVisibility()
        {
            SetPropertyBrowsable(nameof(MessageWildcardWhitelist), GenerateMessages);
        }

        private void SetReplaceOptionSetPropertiesWithEnumVisibility()
        {
            SetPropertyBrowsable(nameof(ReplaceOptionSetPropertiesWithEnum), GenerateEnumProperties);
            SetUseEnumForStateCodesVisibility();
        }

        private void SetUseEnumForStateCodesVisibility()
        {
            var areOptionSetsGenerated = !GenerateEnumProperties || (GenerateEnumProperties && !ReplaceOptionSetPropertiesWithEnum);
            SetPropertyBrowsable(nameof(UseEnumForStateCodes), areOptionSetsGenerated && !ReplaceOptionSetPropertiesWithEnum);
        }

        private void SetUseLogicalNamesVisibility()
        {
            SetPropertyBrowsable(nameof(UseLogicalNames), !CamelCaseMemberNames);
        }

        private const string NotImplemented = "{Not Implemented} ";

        private void SetPropertyBrowsable(string propertyName, bool browsable)
        {
            var prop = Descriptor.GetProperty(propertyName);
            prop.SetIsBrowsable(browsable);
        }

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
