using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Plugin
#else
namespace Source.DLaB.Xrm.Plugin
#endif
	
{
    /// <summary>
    /// Fluent Builder for Registered Events
    /// </summary>
    public class RegisteredEventBuilder
    {
        /// <summary>
        /// Gets or sets the Assert Validators
        /// </summary>
        public List<AssertValidator> AssertValidators { get; set; } = new List<AssertValidator>();
        /// <summary>
        /// Gets or sets the entity logical names.
        /// </summary>
        /// <value>
        /// The entity logical names.
        /// </value>
        protected List<string> EntityLogicalNames { get; set; }
        /// <summary>
        /// Gets or sets the execute.
        /// </summary>
        /// <value>
        /// The execute.
        /// </value>
        protected Action<IExtendedPluginContext> Execute { get; set; }
        /// <summary>
        /// Gets or sets the name of the Execute method for logging purposes.
        /// </summary>
        /// <value>
        /// The name of the execute method.
        /// </value>
        protected string ExecuteMethodName { get; set; }
        /// <summary>
        /// Gets or sets the message types.
        /// </summary>
        /// <value>
        /// The message types.
        /// </value>
        protected List<MessageType> MessageTypes { get; set; }
        /// <summary>
        /// The Previous Builder if any.  This is set via the And() method
        /// </summary>
        protected RegisteredEventBuilder PreviousBuilder { get; set; }
        /// <summary>
        /// Gets or sets the requirement validator.
        /// </summary>
        protected IRequirementValidator RequirementValidator { get; set; }
        /// <summary>
        /// Gets or sets the stage.
        /// </summary>
        /// <value>
        /// The stage.
        /// </value>
        protected PipelineStage Stage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredEventBuilder"/> class.  With the only required attributes, what pipelines stage to execute
        /// </summary>
        /// <param name="stage">The stage.</param>
        /// <param name="messageTypes">The message types.</param>
        public RegisteredEventBuilder(PipelineStage stage, params MessageType[] messageTypes)
        {
            if (!messageTypes.Any())
            {
                messageTypes = new [] { RegisteredEvent.Any };
            }

            EntityLogicalNames = new List<string>();
            Execute = null;
            MessageTypes = new List<MessageType>(messageTypes);
            Stage = stage;
        }

        #region Fluent Methods

        #region ForEntities

        /// <summary>
        /// Defines the Entities that will be created for the registered events
        /// </summary>
        /// <param name="logicalnames">The logicalnames.</param>
        /// <returns></returns>
        public RegisteredEventBuilder ForEntities(params string[] logicalnames)
        {
            EntityLogicalNames.AddRange(logicalnames);
            return this;
        }

        /// <summary>
        /// Defines the Entities that will be created for the registered events
        /// </summary>
        /// <returns></returns>
        public RegisteredEventBuilder ForEntities<T1>() where T1: Entity
        {
            EntityLogicalNames.Add(EntityHelper.GetEntityLogicalName<T1>());
            return this;
        }

        /// <summary>
        /// Defines the Entities that will be created for the registered events
        /// </summary>
        /// <returns></returns>
        public RegisteredEventBuilder ForEntities<T1, T2>() 
            where T1: Entity
            where T2: Entity
        {
            EntityLogicalNames.AddRange(
            new [] {
                EntityHelper.GetEntityLogicalName<T1>(),
                EntityHelper.GetEntityLogicalName<T2>()
            });
            return this;
        }

        /// <summary>
        /// Defines the Entities that will be created for the registered events
        /// </summary>
        /// <returns></returns>
        public RegisteredEventBuilder ForEntities<T1, T2, T3>() 
            where T1: Entity
            where T2: Entity
            where T3: Entity
        {
            EntityLogicalNames.AddRange(
                new [] {
                    EntityHelper.GetEntityLogicalName<T1>(),
                    EntityHelper.GetEntityLogicalName<T2>(),
                    EntityHelper.GetEntityLogicalName<T3>()
                });
            return this;
        }

        /// <summary>
        /// Defines the Entities that will be created for the registered events
        /// </summary>
        /// <returns></returns>
        public RegisteredEventBuilder ForEntities<T1, T2, T3, T4>() 
            where T1: Entity
            where T2: Entity
            where T3: Entity
            where T4: Entity
        {
            EntityLogicalNames.AddRange(
                new [] {
                    EntityHelper.GetEntityLogicalName<T1>(),
                    EntityHelper.GetEntityLogicalName<T2>(),
                    EntityHelper.GetEntityLogicalName<T3>(),
                    EntityHelper.GetEntityLogicalName<T4>()
                });
            return this;
        }

        /// <summary>
        /// Defines the Entities that will be created for the registered events
        /// </summary>
        /// <returns></returns>
        public RegisteredEventBuilder ForEntities<T1, T2, T3, T4, T5>() 
            where T1: Entity
            where T2: Entity
            where T3: Entity
            where T4: Entity
            where T5: Entity
        {
            EntityLogicalNames.AddRange(
                new [] {
                    EntityHelper.GetEntityLogicalName<T1>(),
                    EntityHelper.GetEntityLogicalName<T2>(),
                    EntityHelper.GetEntityLogicalName<T3>(),
                    EntityHelper.GetEntityLogicalName<T4>(),
                    EntityHelper.GetEntityLogicalName<T5>()
                });
            return this;
        }

        #endregion ForEntities

        /// <summary>
        /// Defines the custom Action to be performed rather than the standard ExecuteInternal.
        /// </summary>
        /// <param name="execute">Action that is invoked when the Plugin Executes.</param>
        /// <returns></returns>
        public RegisteredEventBuilder WithExecuteAction<T>(Action<T> execute) where T : IExtendedPluginContext
        {
            ExecuteMethodName = execute.Method.Name;
            Execute = context => execute((T)context);
            return this;
        }

        /// <summary>
        /// Defines the custom Action to be performed rather than the standard ExecuteInternal.
        /// </summary>
        /// <param name="execute">Action that is invoked when the Plugin Executes.</param>
        /// <returns></returns>
        public RegisteredEventBuilder WithExecuteAction(Action<IExtendedPluginContext> execute)
        {
            ExecuteMethodName = execute.Method.Name;
            Execute = execute;
            return this;
        }

        #region Validator

        /// <summary>
        /// Defines the Validator to use to throw an exception for this Registered Event if it isn't met.
        /// Assert Validators will only be evaluated after the Validator (if defined) allows execution of the plugin
        /// </summary>
        /// <param name="validator">Validator</param>
        /// <param name="exToThrow">Exception to throw if execution shouldn't happen</param>
        /// <returns></returns>
        public RegisteredEventBuilder WithAssertValidator(IRequirementValidator validator, Exception exToThrow)
        {
            AssertValidators.Add(new AssertValidator(validator, exToThrow));
            return this;
        }

        /// <summary>
        /// Defines the Validator to use to throw an exception for this Registered Event if it isn't met.
        /// Assert Validators will only be evaluated after the Validator (if defined) allows execution of the plugin
        /// </summary>
        /// <param name="validator">Validator</param>
        /// <param name="exMessageToThrow">Message to throw in an InvalidPluginExecutionException if execution shouldn't happen</param>
        /// <returns></returns>
        public RegisteredEventBuilder WithAssertValidator(IRequirementValidator validator, string exMessageToThrow)
        {
            AssertValidators.Add(new AssertValidator(validator, exMessageToThrow));
            return this;
        }

        /// <summary>
        /// Defines the Validator to use to throw an exception for this Registered Event if it isn't met.
        /// Assert Validators will only be evaluated after the Validator (if defined) allows execution of the plugin
        /// </summary>
        /// <param name="validator">Validator</param>
        /// <param name="errorFactory">Function that creates an exception based on the InvalidColumnRequirementReason</param>
        /// <returns></returns>
        public RegisteredEventBuilder WithAssertValidator(IRequirementValidator validator, Func<InvalidRequirementReason, IExtendedPluginContext, Exception> errorFactory)
        {
            AssertValidators.Add(new AssertValidator(validator, errorFactory));
            return this;
        }

        /// <summary>
        /// Defines the Validator to use to throw an exception for this Registered Event if it isn't met.
        /// Assert Validators will only be evaluated after the Validator (if defined) allows execution of the plugin
        /// </summary>
        /// <param name="assertValidator">The validator</param>
        /// <returns></returns>
        public RegisteredEventBuilder WithAssertValidator(AssertValidator assertValidator)
        {
            AssertValidators.Add(assertValidator);
            return this;
        }

        /// <summary>
        /// Defines the Validator to use to throw an exception for this Registered Event if it isn't met.
        /// Assert Validators will only be evaluated after the Validator (if defined) allows execution of the plugin
        /// </summary>
        /// <param name="validators">The validators</param>
        /// <returns></returns>
        public RegisteredEventBuilder WithAssertValidators(IEnumerable<AssertValidator> validators)
        {
            AssertValidators.AddRange(validators);
            return this;
        }

        /// <summary>
        /// Defines the Validator to use for this Registered Event.  A registered event can only have a single validator.
        /// </summary>
        /// <param name="validator">Validator</param>
        /// <returns></returns>
        public RegisteredEventBuilder WithValidator(IRequirementValidator validator)
        {
            RequirementValidator = validator;
            return this;
        }

        #endregion Validator

        #endregion Fluent Methods

        /// <summary>
        /// Creates a new RegisteredEventBuilder, but keeps the previous RegisteredEventBuilder linked so a call to build, will build the previous objects in the chain.
        /// </summary>
        /// <param name="stage"></param>
        /// <param name="messageTypes"></param>
        /// <returns></returns>
        public RegisteredEventBuilder And(PipelineStage stage, params MessageType[] messageTypes)
        {
            var builder = new RegisteredEventBuilder(stage, messageTypes)
            {
                PreviousBuilder = this
            };

            return builder;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns></returns>
        public List<RegisteredEvent> Build()
        {
            var events = new List<RegisteredEvent>();
            BuildAndAddTo(events);
            return events;
        }

        private void BuildAndAddTo(List<RegisteredEvent> events)
        {
            PreviousBuilder?.BuildAndAddTo(events);

            foreach (var messageType in MessageTypes)
            {
                if (EntityLogicalNames.Any())
                {
                    events.AddRange(
                        EntityLogicalNames.Select(
                            logicalName => new RegisteredEvent(Stage, messageType, Execute, logicalName)
                            {
                                AssertValidators = AssertValidators,
                                ExecuteMethodName = ExecuteMethodName,
                                RequirementValidator = RequirementValidator
                            }));
                }
                else
                {
                    events.Add(
                        new RegisteredEvent(Stage, messageType, Execute)
                        {
                            AssertValidators = AssertValidators,
                            ExecuteMethodName = ExecuteMethodName,
                            RequirementValidator = RequirementValidator
                        });
                }
            }
        }
    }
}
