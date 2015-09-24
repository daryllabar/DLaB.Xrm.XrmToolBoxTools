using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace DLaB.XrmToolboxCommon
{
    public partial class DialogBase : Form, IWorkerHost
    {
        protected PluginControlBase CallingControl { get; set; }
        protected IOrganizationService Service { get { return CallingControl.Service; } }

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
            WorkAsync("Retrieving Entities...", e =>
            {
                e.Result = Service.Execute(new RetrieveAllEntitiesRequest() { EntityFilters = EntityFilters.Entity, RetrieveAsIfPublished = true });
            }, e =>
            {
                var entityContainer = ((PropertyInterface.IEntityMetadatas)CallingControl);
                entityContainer.EntityMetadatas = ((RetrieveAllEntitiesResponse)e.Result).EntityMetadata;
                _callBackForRetrieveEntityMetadatas(entityContainer.EntityMetadatas);
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
            WorkAsync("Retrieving Actions...", e =>
            {
                var qe = new QueryExpression("workflow");
                qe.ColumnSet = new ColumnSet(true);
                qe.Criteria.AddCondition("category", ConditionOperator.Equal, 3); // Action
                qe.Criteria.AddCondition("parentworkflowid", ConditionOperator.Null);
                e.Result = Service.RetrieveMultiple(qe);
            }, e =>
            {
                var actionContainer = ((PropertyInterface.IActions)CallingControl);
                actionContainer.Actions = ((EntityCollection)e.Result).Entities;
                _callBackForRetrieveActions(actionContainer.Actions);
            });
        }
        #endregion // Helper Methods

        #region IWorkerHost Members

        private readonly Worker _worker = new Worker();

        public void WorkAsync(string message, Action<DoWorkEventArgs> work, object argument, int messageWidth = 340, int messageHeight = 150)
        {
            _worker.WorkAsync(this, message, null, null, argument, messageWidth, messageHeight);
        }

        public void WorkAsync(string message, Action<DoWorkEventArgs> work, Action<RunWorkerCompletedEventArgs> callback, object argument = null, int messageWidth = 340, int messageHeight = 150)
        {
            _worker.WorkAsync(this, message, work, callback, argument, messageWidth, messageHeight);
        }

        public void WorkAsync(string message, Action<BackgroundWorker, DoWorkEventArgs> work, Action<RunWorkerCompletedEventArgs> callback, Action<ProgressChangedEventArgs> progressChanged, object argument = null, int messageWidth = 340, int messageHeight = 150)
        {
            _worker.WorkAsync(this, message, work, callback, progressChanged, argument, messageWidth, messageHeight);
        }

        public void SetWorkingMessage(string message, int width = 340, int height = 100)
        {
            _worker.SetWorkingMessage(this, message, width, height);
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
