using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Source.DLaB.Xrm;
using DLaB.XrmToolBoxCommon;
using PropertyInterface = DLaB.XrmToolBoxCommon.PropertyInterface;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using XrmToolBox.Extensibility;

namespace DLaB.EarlyBoundGenerator
{
    public partial class AttributeToEnumMapperDialog : DialogBase
    {
        #region Properties

        public string EntityName { get; set; }
        public string AttributeName { get; set; }
        public string OptionSetSchemaName { get; set; }

        #endregion // Properties

        #region Constructor / Load

        public AttributeToEnumMapperDialog()
        {
            InitializeComponent();
        }

        public AttributeToEnumMapperDialog(PluginControlBase callingControl)
            : base(callingControl)
        {
            InitializeComponent();
        }

        private void LocalAttributeToEnumMapperDialog_Load(object sender, EventArgs e)
        {
            MakeLocalVisible(false);
            LblOptionSet.Visible = false;
            CmbOptionSets.Visible = false;

            Enable(false);
            RetrieveEntityMetadatasOnLoad(LoadEntities);
        }

        #endregion // Constructor / Load


        private void CmbEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            Enable(false);
            ExecuteMethod(RetrieveAttributes);
        }

        private void CmbAttributes_SelectedIndexChanged(object sender, EventArgs e)
        {
            var attribute = CmbAttributes.SelectedItem as ObjectCollectionItem<AttributeMetadata>;
            if (attribute == null)
            {
                return;
            }

            if (attribute.Value.IsLocalOptionSetAttribute())
            {
                OptionSetSchemaName = attribute.Value.EntityLogicalName + "_" + attribute.Value.SchemaName.ToLower();
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(OptionSetSchemaName))
            {
                MessageBox.Show(("No OptionSet Attribute Schema Name specified"),
                "No OptionSet!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (string.IsNullOrWhiteSpace(EntityName) || string.IsNullOrWhiteSpace(AttributeName))
            {
                MessageBox.Show(("No Entity Attribute was specified to map to the Option Set"),
                "No Entity Attribute!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else 
            { 
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void LoadEntities(IEnumerable<EntityMetadata> entities)
        {
            try
            {
                CmbAttributeEntities.BeginUpdate();
                CmbEntities.BeginUpdate();

                CmbAttributeEntities.Items.Clear();
                CmbEntities.Items.Clear();
                CmbEntities.Text = null;

                var values = entities.ToObjectCollectionArray();

                CmbAttributeEntities.Items.AddRange(values);
                CmbEntities.Items.AddRange(values);
            }
            finally
            {
                CmbAttributeEntities.EndUpdate();
                CmbEntities.EndUpdate();
                Enable(true);
            }
        }

        public void RetrieveAttributes()
        {
            RetrieveAttributesInternal(CmbEntities, CmbAttributes, true);          
        }

        public void RetrieveEntityAttributes()
        {
            RetrieveAttributesInternal(CmbAttributeEntities, CmbAttributeAttributes, false);
        }

        private void RetrieveAttributesInternal(ComboBox cmbEntities, ComboBox cmbAttributes, bool limitToLocalOptionSetAttributes)
        {
            var entity = cmbEntities.SelectedItem as ObjectCollectionItem<EntityMetadata>;
            if (entity == null)
            {
                return;
            }

            WorkAsync(new WorkAsyncInfo("Retrieving Attributes...",
                e =>
                {
                    e.Result = Service.Execute(new RetrieveEntityRequest
                    {
                        LogicalName = entity.Value.LogicalName,
                        EntityFilters = EntityFilters.Attributes,
                        RetrieveAsIfPublished = true
                    });
                })
            {
                PostWorkCallBack = e =>
                {
                    try
                    {
                        cmbAttributes.BeginUpdate();
                        cmbAttributes.Items.Clear();
                        cmbAttributes.Text = null;

                        var result = ((RetrieveEntityResponse) e.Result).EntityMetadata.Attributes.
                            Where(a => a.AttributeType == AttributeTypeCode.Picklist && (!limitToLocalOptionSetAttributes || a.IsLocalOptionSetAttribute())).
                            Select(a => new ObjectCollectionItem<AttributeMetadata>(a.SchemaName + " (" + a.LogicalName + ")", a)).
                            OrderBy(r => r.DisplayName);

                        cmbAttributes.Items.AddRange(result.Cast<object>().ToArray());
                    }
                    finally
                    {
                        cmbAttributes.EndUpdate();
                        Enable(true);
                    }
                }
            });
        }

        private void Enable(bool enable)
        {
            CmbAttributeEntities.Enabled = enable;
            CmbAttributeAttributes.Enabled = enable;
            CmbEntities.Enabled = enable;
            CmbAttributes.Enabled = enable;
            CmbOptionSets.Enabled = enable;
            RdoLocal.Enabled = enable;
            RdoGlobal.Enabled = enable;
            BtnAdd.Enabled = enable;
        }

        public void RetrieveOptionSets()
        {
            WorkAsync(new WorkAsyncInfo("Retrieving OptionSets...", e =>
            {
                e.Result = ((RetrieveAllOptionSetsResponse) Service.Execute(new RetrieveAllOptionSetsRequest())).OptionSetMetadata;
            })
            {
                PostWorkCallBack = e =>
                {
                    var entityContainer = (PropertyInterface.IGlobalOptionSets) CallingControl;
                    entityContainer.GlobalOptionSets = (IEnumerable<OptionSetMetadataBase>) e.Result;
                    LoadOptionSets(entityContainer.GlobalOptionSets);
                }
            });
        }

        private void LoadOptionSets(IEnumerable<OptionSetMetadataBase> optionSets)
        {
            try
            {
                CmbOptionSets.BeginUpdate();
                CmbOptionSets.Items.Clear();
                CmbOptionSets.Text = null;

                var values = optionSets.
                    Select(e => new ObjectCollectionItem<OptionSetMetadataBase>(e.Name, e)).
                    OrderBy(r => r.DisplayName).ToList();

                CmbOptionSets.Items.AddRange(values.Cast<object>().ToArray());
            }
            finally
            {
                CmbOptionSets.EndUpdate();
                Enable(true);
            }
        }

        private void RdoLocal_CheckedChanged(object sender, EventArgs e)
        {
            if (!RdoLocal.Checked) { return; }

            OptionSetSchemaName = string.Empty;
            MakeLocalVisible(true);
        }

        private void RdoGlobal_CheckedChanged(object sender, EventArgs e)
        {
            if (!RdoGlobal.Checked) { return; }

            OptionSetSchemaName = string.Empty;
            MakeLocalVisible(false);
            Enable(false);
            var optionSets = ((PropertyInterface.IGlobalOptionSets)CallingControl).GlobalOptionSets;

            if (optionSets == null)
            {
                ExecuteMethod(RetrieveOptionSets);
            }
            else
            {
                LoadOptionSets(optionSets);
            }
        }

        private void MakeLocalVisible(Boolean value)
        {
            LblOptionSet.Visible = !value;
            CmbOptionSets.Visible = !value;

            LblEntity.Visible = value;
            CmbEntities.Visible = value;
            LblAttribute.Visible = value;
            CmbAttributes.Visible = value;
        }

        private void CmbOptionSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            var optionSet = CmbOptionSets.SelectedItem as ObjectCollectionItem<OptionSetMetadataBase>;
            if (optionSet == null)
            {
                return;
            }

            if (optionSet.Value.IsGlobal.GetValueOrDefault(false))
            {
                OptionSetSchemaName = optionSet.Value.Name.ToLower();
            }
        }

        private void CmbAttributeEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            Enable(false);
            ExecuteMethod(RetrieveEntityAttributes);
        }

        private void CmbAttributeAttributes_SelectedIndexChanged(object sender, EventArgs e)
        {
            var attribute = CmbAttributeAttributes.SelectedItem as ObjectCollectionItem<AttributeMetadata>;
            if (attribute == null)
            {
                return;
            }

            EntityName = attribute.Value.EntityLogicalName;
            AttributeName = attribute.Value.SchemaName;
        }
    }
}
