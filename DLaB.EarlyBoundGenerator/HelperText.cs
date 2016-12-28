using System;

namespace DLaB.EarlyBoundGenerator
{
    public partial class EarlyBoundGeneratorPlugin
    {
        private void BtnActionsToSkip_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Allows for the ability to specify Actions to not generate.";
        }

        private void BtnCreateActions_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Generates the Activity Classes.";
        }

        private void BtnCreateAll_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Asynchronously generates the Entites, Option Sets, and Actions (if available), with their perspective settings.";
        }

        private void BtnCreateEntities_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Generates the Entity Classes.";
        }

        private void BtnCreateOptionSets_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Generates the Enums for the Option Sets.";
        }

        private void BtnEntitesToSkip_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Allows for the ability to specify Entities to not generate.";
        }

        private void BtnEnumMappings_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Manually specifies an enum mapping for an OptionSetValue Property on an entity.";
        }

        private void BtnOptionSetsToSkip_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Allows for the ability to specify OptionSets to not generate.";
        }

        private void BtnSpecifyAttributeNames_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Allows for the ability to specify the capitalization of an attribute on an entity.";
        }

        private void BtnUnmappedProperties_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Allows for the ability to specify an OptionSetValue Property of an entity that doesn't have an enum mapping.";
        }

        private void ChkAddDebuggerNonUserCode_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Specifies that the DebuggerNonUserCodeAttribute should be applied to all generated properties and methods.";
        }

        private void ChkAddFilesToProject_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Adds any files that don't exist to the first project file found in the hierarchy of the output path.";
        }

        private void ChkAudibleCompletion_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Use speech synthesizer to notify of code generation completion.  May not work on VMs or machines without sound cards.";
        }

        private void ChkCreateOneActionFile_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Specifies that each Activity will be created in its own file.";
        }

        private void ChkCreateOneEntityFile_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Specifies that each Entity will be created in its own file.";
        }

        private void ChkCreateOneOptionSetFile_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Specifies that each Enum will be created in its own file.";
        }

        private void ChkGenerateAttributeNameConsts_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Adds a Struct to each Entity class that contains the Logical Names of all attributes for the Entity.";
        }

        private void ChkGenerateAnonymousTypeConstructor_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Adds an Object Constructor to each early bound entity class to simplify LINQ Projections (http://stackoverflow.com/questions/27623542).";
        }

        private void ChkGenerateEntityRelationships_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Specifies if 1:N, N:1, and N:N relationships are generated for entities.";
        }

        private void ChkGenerateOptionSetEnums_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Adds an additional property to each early bound entity class, for each optionset property it normally contains, with Enum postfixed to the existing optionset name.";
        }

        private void ChkIncludeCommandLine_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Specifies whether to include the command line in the early bound class used to generate it.";
        }

        private void ChkMakeReadonlyFieldsEditable_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Defines that Entities should be created with editable createdby, createdon, modifiedby, modifiedon, owningbusinessunit, owningteam, and owninguser properties. Helpful for writing linq statements where those attributes are wanting to be returned in the select.";
        }

        private void ChkMaskPassword_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Masks the password in the command line.";
        }

        private void ChkRemoveRuntimeComment_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Removes the ""//   Runtime Version:X.X.X.X"" comment from the header of generated files.  This helps to alleviate unnecessary differences that pop up when the classes are generated from machines with different .Net Framework updates installed.";
        }

        private void ChkUseDeprecatedOptionSetNaming_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Creates Local OptionSets Using the Deprecated Naming Convention. prefix_oobentityname_prefix_attribute";
        }

        private void ChkUseTFS_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Will use TFS to attempt to check out the early bound classes.";
        }

        private void ChkUseXrmClient_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Specifies the Service Context should inherit from CrmOrganizationServiceContext, and conversly, Entities from Xrm.Client.Entity." + Environment.NewLine +
                @"This results in a dependence on Microsoft.Xrm.Client.dll that must be accounted for during plugins and workflows since it isn't included with CRM by default:" + Environment.NewLine +
                @"http://develop1.net/public/post/MicrosoftXrmClient-Part-1.aspx .";
        }

        private void TxtOptionSetFormat_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"The Format of Local Option Sets where {0} is the Entity Schema Name, and {1} is the Attribute Schema Name.  The format Specified in the SDK is {0}{1}, but the default is {0}_{1}, but used to be prefix_{0}_{1}(all lower case)";
        }

        private void TxtServiceContextName_MouseEnter(object sender, EventArgs e)
        {
            TxtHelp.Text = @"Specifies that the name of the Generated CRM Context.";
        }

        private void ShowActionPathText(object sender, EventArgs e)
        {
            TxtHelp.Text = ChkCreateOneActionFile.Checked ?
                @"Since ""Create One File Per Action"" is checked, this needs to be a file path that ends in "".cs"".  This is realtive to the Path of the Settings File." :
                @"Since ""Create One File Per Action"" is not checked, this needs to be a path to a directory.  This is realtive to the Path of the Settings File.";
        }

        private void ShowEntityPathText(object sender, EventArgs e)
        {
            TxtHelp.Text = ChkCreateOneEntityFile.Checked ?
                @"Since ""Create One File Per Entity"" is checked, this needs to be a file path that ends in "".cs"".  This is realtive to the Path of the Settings File." :
                @"Since ""Create One File Per Entity"" is not checked, this needs to be a path to a directory.  This is realtive to the Path of the Settings File.";
        }

        private void ShowOptionSetPathText(object sender, EventArgs e)
        {
            TxtHelp.Text = ChkCreateOneOptionSetFile.Checked ?
                @"Since ""Create One File Per Option Set"" is checked, this needs to be a file path that ends in "".cs"".  This is realtive to the Path of the Settings File." :
                @"Since ""Create One File Per Option Set"" is not checked, this needs to be a path to a directory.  This is realtive to the Path of the Settings File.";
        }

        private void ShowSettingsPathText(object sender, EventArgs e)
        {
            TxtHelp.Text = @"The path to the settings file associated with this connection.  Changing it while connected, updates the path for the connection.  Changing the connection will cause the settings to reload for that conneciton";
        }
    }
}
