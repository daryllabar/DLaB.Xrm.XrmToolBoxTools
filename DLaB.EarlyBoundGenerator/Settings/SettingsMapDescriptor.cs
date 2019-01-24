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

        #endregion OnChange Handlers


        private void ProcessDynamicallyVisibleProperties()
        {
            SetAddFilesToProjectVisibility();
            SetMaskPasswordVisibility();
            SetPropertyEnumMappingVisibility();
            SetUnmappedPropertiesVisibility();
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

        /*

    private void ChkCreateOneActionFile_CheckedChanged(object sender, EventArgs e)
    {
        LblActionsDirectory.Visible = ChkCreateOneActionFile.Checked;
        LblActionPath.Visible = !ChkCreateOneActionFile.Checked;

        ConditionallyAddRemoveExtension(TxtActionPath, "Actions.cs", ChkCreateOneActionFile.Checked);
    }

    private void ChkCreateOneOptionSetFile_CheckedChanged(object sender, EventArgs e)
    {
        LblOptionSetsDirectory.Visible = ChkCreateOneOptionSetFile.Checked;
        LblOptionSetPath.Visible = !ChkCreateOneOptionSetFile.Checked;

        ChkAddFilesToProject.Visible = !ChkCreateOneEntityFile.Checked;

        ConditionallyAddRemoveExtension(TxtOptionSetPath, "OptionSets.cs", ChkCreateOneOptionSetFile.Checked);
    }

    private void ChkUseDeprecatedOptionSetNaming_CheckedChanged(object sender, EventArgs e)
    {
        LblOptionSetFormat.Visible = !ChkUseDeprecatedOptionSetNaming.Checked;
        TxtOptionSetFormat.Visible = !ChkUseDeprecatedOptionSetNaming.Checked;
    }

            private void ChkCreateOneEntityFile_CheckedChanged(object sender, EventArgs e)
    {
        LblEntitiesDirectory.Visible = ChkCreateOneEntityFile.Checked;
        LblEntityPath.Visible = !ChkCreateOneEntityFile.Checked;

        ConditionallyAddRemoveExtension(TxtEntityPath, "Entities.cs", ChkCreateOneEntityFile.Checked);
    }

    private void TxtLanguageCodeOverride_TextChanged(object sender, EventArgs e)
    {
        var value = TxtLanguageCodeOverride.Text;
        if (!string.IsNullOrWhiteSpace(value)
            && !int.TryParse(value, out var _))
        {
            TxtLanguageCodeOverride.Text = "";
        }
    }

*/
    }
}
