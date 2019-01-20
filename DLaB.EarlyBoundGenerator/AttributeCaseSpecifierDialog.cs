using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Source.DLaB.Common;
using DLaB.XrmToolBoxCommon;
using PropertyInterface = DLaB.XrmToolBoxCommon.PropertyInterface;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using XrmToolBox.Extensibility;

namespace DLaB.EarlyBoundGenerator
{
    public partial class AttributeCaseSpecifierDialog : DialogBase
    {
        #region Properties
         
        public string EntityName { get; set; }
        private string AttributeSchemaName { get; set; }
        public string AttributeName { get; set; }

        #endregion // Properties

        #region Constructor / Load

        public AttributeCaseSpecifierDialog()
        {
            InitializeComponent();
        }

        public AttributeCaseSpecifierDialog(PluginControlBase callingControl)
            : base(callingControl)
        {
            InitializeComponent();
        }

        private void AttributeCaseSpecifierDialog_Load(object sender, EventArgs e)
        {
            Enable(false);
            RetrieveEntityMetadatasOnLoad(LoadEntities);
            var entities = ((PropertyInterface.IEntityMetadatas)CallingControl).EntityMetadatas;

            if (entities == null)
            {
                ExecuteMethod(RetrieveEntities);
            }
            else
            {
                LoadEntities(entities);
            }
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

            EntityName = attribute.Value.EntityLogicalName;
            AttributeSchemaName = attribute.Value.SchemaName;
            TxtAttribute.Text = AttributeSchemaName;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var name = TxtAttribute.Text.Trim();
            if (!string.Equals(AttributeSchemaName, name, StringComparison.InvariantCultureIgnoreCase))
            {
                MessageBox.Show(string.Format("Capitalization for attribute name must match exactly ({0} == {1})", AttributeSchemaName, name),
                    "Incorrect Attribute Name!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                AttributeName = name;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        public void RetrieveEntities()
        {
            WorkAsync(new WorkAsyncInfo("Retrieving Entities...",
            e =>
            {
                e.Result = Service.Execute(new RetrieveAllEntitiesRequest() { EntityFilters = EntityFilters.Entity, RetrieveAsIfPublished = true });
            })
            {
                PostWorkCallBack = e =>
                {

                    var entityContainer = ((PropertyInterface.IEntityMetadatas) CallingControl);
                    entityContainer.EntityMetadatas = ((RetrieveAllEntitiesResponse) e.Result).EntityMetadata;
                    LoadEntities(entityContainer.EntityMetadatas);
                }
            });
        }

        private void LoadEntities(IEnumerable<EntityMetadata> entities)
        {
            try
            {
                CmbEntities.BeginUpdate();
                CmbEntities.Items.Clear();
                CmbEntities.Items.AddRange(entities.ToObjectCollectionArray());
            }
            finally
            {
                CmbEntities.EndUpdate();
                Enable(true);
            }
        }

        public void RetrieveAttributes()
        {
            var entity = CmbEntities.SelectedItem as ObjectCollectionItem<EntityMetadata>;
            if (entity == null)
            {
                return;
            }

            WorkAsync(new WorkAsyncInfo("Retrieving Attributes...",
            e =>
            {
                e.Result = Service.Execute(new RetrieveEntityRequest()
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
                        CmbAttributes.BeginUpdate();
                        CmbAttributes.Items.Clear();

                        var result = ((RetrieveEntityResponse)e.Result).EntityMetadata.Attributes.
                                     Select(a => new ObjectCollectionItem<AttributeMetadata>(a.SchemaName + " (" + a.LogicalName + ")", a)).
                                     OrderBy(r => r.DisplayName);

                        CmbAttributes.Items.AddRange(result.ToArray());
                    }
                    finally
                    {
                        CmbAttributes.EndUpdate();
                        Enable(true);
                    }
                }
            });      
        }

        private void Enable(bool enable)
        {
            CmbEntities.Enabled = enable;
            CmbAttributes.Enabled = enable;
            TxtAttribute.Enabled = enable;
            BtnAdd.Enabled = enable;
        }
    }
}
