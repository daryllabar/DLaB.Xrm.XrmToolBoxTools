using System;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Plugin
#else
namespace Source.DLaB.Xrm.Plugin
#endif

{
    /// <summary>
    /// Class for Defining a Plugin Execution Event
    /// </summary>
    public class RegisteredEvent
    {
        /// <summary>
        /// Defines an Any Message Type
        /// </summary>
        public static MessageType Any = new MessageType("Any");

        /// <summary>
        /// Gets or sets the pipeline stage.
        /// </summary>
        /// <value>
        /// The stage.
        /// </value>
        public PipelineStage Stage { get; set; }
        /// <summary>
        /// Gets or sets the message type.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public MessageType Message { get; set; }
        /// <summary>
        /// The text value of the MessageType
        /// </summary>
        /// <value>
        /// The name of the message.
        /// </value>
        public string MessageName => Message.ToString();
        /// <summary>
        /// The logical entity name of the entity the plugin is executing for.
        /// </summary>
        /// <value>
        /// The name of the entity logical.
        /// </value>
        public string EntityLogicalName { get; set; }
        /// <summary>
        /// Gets or sets the execute.
        /// </summary>
        /// <value>
        /// The execute.
        /// </value>
        public Action<IExtendedPluginContext> Execute { get; set; }

        private string _executeMethodName;
        /// <summary>
        /// Gets or sets the Execute's Method Name for logging purposes.
        /// </summary>
        /// <value>
        /// The execute.
        /// </value>
        public string ExecuteMethodName {
            get { return _executeMethodName ?? Execute?.Method.Name ?? "Execute"; }
            set { _executeMethodName = value; } }

        /// <summary>
        /// Defaults the execute method to be InternalExecute and run against all entities.
        /// </summary>
        /// <param name="stage"></param>
        /// <param name="message"></param>
        public RegisteredEvent(PipelineStage stage, MessageType message) : this(stage, message, null, null) { }

        /// <summary>
        /// Defaults the execute method to be InternalExecute and runs against the specified entity.
        /// </summary>
        /// <param name="stage"></param>
        /// <param name="message"></param>
        /// <param name="entityLogicalName"></param>
        public RegisteredEvent(PipelineStage stage, MessageType message, string entityLogicalName) : this(stage, message, null, entityLogicalName) { }

        /// <summary>
        /// Runs against all entities.
        /// </summary>
        /// <param name="stage"></param>
        /// <param name="message"></param>
        /// <param name="execute"></param>
        public RegisteredEvent(PipelineStage stage, MessageType message, Action<IExtendedPluginContext> execute) : this(stage, message, execute, null) { }


        /// <summary>
        /// Runs against the specified entity
        /// </summary>
        /// <param name="stage"></param>
        /// <param name="message"></param>
        /// <param name="execute"></param>
        /// <param name="entityLogicalName"></param>
        public RegisteredEvent(PipelineStage stage, MessageType message, Action<IExtendedPluginContext> execute, string entityLogicalName)
        {
            Stage = stage;
            EntityLogicalName = entityLogicalName;
            Execute = execute;
            Message = message;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance, formatted with the given tab to allow for nexting
        /// </summary>
        /// <param name="tab">The tab.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public virtual string ToString(string tab)
        {
            tab = tab ?? string.Empty;
            return string.Join(Environment.NewLine + tab,
                tab + "Stage: " + Stage,
                "Message: " + Message,
                "Message Name: " + MessageName,
                "Entity Logical Name: " + EntityLogicalName, "Execute: " + (Execute?.Method.Name ?? "Null"));
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ToString(null);
        }
    }
}
