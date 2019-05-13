using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DLaB.Xrm.Entities;
using Source.DLaB.Xrm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace DLaB.XrmToolBoxCommon
{
    public partial class DialogBase : Form, IWorkerHost
    {
        protected PluginControlBase CallingControl { get; set; }
        protected IOrganizationService Service => CallingControl.Service;

        public DialogBase()
        {
            InitializeComponent();
        }

        public DialogBase(PluginControlBase callingControl)
            : this()
        {
            if (callingControl == null)
            {
                throw new ArgumentNullException(nameof(callingControl));
            }
            CallingControl = callingControl;
        }

        #region Helper Methods

        protected void RetrieveEntityMetadatasOnLoad(Action<IEnumerable<EntityMetadata>> loadEntities)
        {
            var entities = ((PropertyInterface.IEntityMetadatas)CallingControl).EntityMetadatas;

            if (entities == null)
            {
                _callBackForRetrieveEntityMetadatas = loadEntities;
                ExecuteMethod(RetrieveEntities);
            }
            else
            {
                loadEntities(entities);
            }
        }

        private Action<IEnumerable<EntityMetadata>> _callBackForRetrieveEntityMetadatas;

        private void RetrieveEntities()
        {
            WorkAsync(new WorkAsyncInfo("Retrieving Entities...", e =>
            {
                e.Result = Service.Execute(new RetrieveAllEntitiesRequest {EntityFilters = EntityFilters.Entity, RetrieveAsIfPublished = true});
            })
            {
                PostWorkCallBack = e =>
                {
                    var entityContainer = ((PropertyInterface.IEntityMetadatas) CallingControl);
                    entityContainer.EntityMetadatas = ((RetrieveAllEntitiesResponse) e.Result).EntityMetadata;
                    _callBackForRetrieveEntityMetadatas(entityContainer.EntityMetadatas);
                }
            });
        }

        protected void RetrieveActionsOnLoad(Action<IEnumerable<Entity>> loadActions)
        {
            var actions = ((PropertyInterface.IActions)CallingControl).Actions;

            if (actions == null)
            {
                _callBackForRetrieveActions = loadActions;
                ExecuteMethod(RetrieveActions);
            }
            else
            {
                loadActions(actions);
            }
        }

        private Action<IEnumerable<Entity>> _callBackForRetrieveActions;

        private void RetrieveActions()
        {
            WorkAsync(new WorkAsyncInfo("Retrieving Actions...", e =>
                      {
                          var qe = QueryExpressionFactory.Create<Workflow>(w => new {w.Name, w.UniqueName},
                                                                           Workflow.Fields.Category, (int) Workflow_Category.Action,
                                                                           Workflow.Fields.ParentWorkflowId, null);
                          qe.AddLink<SdkMessage>(Workflow.Fields.SdkMessageId, m => new {m.Name});
                          var entities = Service.RetrieveMultiple(qe.Query).ToEntityList<Workflow>();
                          e.Result = entities.Select(w =>
                          {
                              w[@"sdklogicalname"] = w.GetAliasedEntity<SdkMessage>().Name;
                              return w.ToSdkEntity();
                          }).ToList();
                      })
            {
                PostWorkCallBack = e =>
                {
                    var actionContainer = ((PropertyInterface.IActions) CallingControl);
                    actionContainer.Actions = (List<Entity>) e.Result;
                    _callBackForRetrieveActions(actionContainer.Actions);
                }
            });
        }

        #endregion // Helper Methods

        #region IWorkerHost Members

        private readonly Worker _worker = new Worker();

        public void SetWorkingMessage(string message, int width = 340, int height = 100)
        {
            _worker.SetWorkingMessage(this, message, width, height);
        }

        public void WorkAsync(WorkAsyncInfo info)
        {
            info.Host = this;
            _worker.WorkAsync(info);
        }

        public void RaiseRequestConnectionEvent(RequestConnectionEventArgs args)
        {
            CallingControl.RaiseRequestConnectionEvent(args);
        }

        #endregion // IWorkerHost Members

        /// <summary>
        /// Checks to make sure that the Plugin has an IOrganizationService Connection, before calling the action.
        /// </summary>
        /// <param name="action"></param>
        protected void ExecuteMethod(Action action)
        {
            AssertCallingControlExists();
            CallingControl.ExecuteMethod(null, new ExternalMethodCallerInfo(action));
        }

        private void AssertCallingControlExists()
        {
            if (CallingControl == null) { throw new NullReferenceException("Use the \"DialogBase(PluginUserControlBase)\" Constructor"); }
        }
    }
}
