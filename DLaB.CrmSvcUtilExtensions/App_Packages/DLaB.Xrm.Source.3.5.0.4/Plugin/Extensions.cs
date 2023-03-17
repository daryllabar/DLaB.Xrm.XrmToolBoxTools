using System;
using System.Collections.Generic;
#if !DLAB_XRM_DEBUG
using System.Diagnostics;
#endif
using System.Linq;
using Microsoft.Xrm.Sdk;
#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
using DLaB.Xrm.Exceptions;

namespace DLaB.Xrm.Plugin
#else
using Source.DLaB.Xrm.Exceptions;

namespace Source.DLaB.Xrm.Plugin
#endif
{
    /// <summary>
    /// Extension Class for Plugins
    /// </summary>
#if !DLAB_XRM_DEBUG
    [DebuggerNonUserCode]
#endif
    public static class Extensions
    {
#region HashSet<T>

        /// <summary>
        /// Adds the all values that don't already exist in the HashSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashSet"></param>
        /// <param name="values"></param>
        public static void AddMissing<T>(this HashSet<T> hashSet, IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                if (!hashSet.Contains(value))
                {
                    hashSet.Add(value);
                }
            }
        }

        /// <summary>
        /// Adds the value if it doesn't already exist in the HashSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashSet"></param>
        /// <param name="value"></param>
        public static void AddMissing<T>(this HashSet<T> hashSet, T value)
        {
            if (!hashSet.Contains(value))
            {
                hashSet.Add(value);
            }
        }

#endregion HashSet<T>

#region List<RegisteredEvent>

#region AddEvent

        /// <summary>
        /// Defaults the execute method to be InternalExecute and run against all entities.
        /// </summary>
        /// <param name="events"></param>
        /// <param name="stage"></param>
        /// <param name="message"></param>
        public static void AddEvent(this List<RegisteredEvent> events, PipelineStage stage, MessageType message)
        {
            events.AddEvent(stage, null, message, null);
        }

        /// <summary>
        /// Runs against all entities.
        /// </summary>
        /// <param name="events"></param>
        /// <param name="stage"></param>
        /// <param name="message"></param>
        /// <param name="execute"></param>
        public static void AddEvent(this List<RegisteredEvent> events, PipelineStage stage, MessageType message, Action<IExtendedPluginContext> execute)
        {
            events.AddEvent(stage, null, message, execute);
        }

        /// <summary>
        /// Defaults the execute method to be InternalExecute and runs against the specified entity.
        /// </summary>
        /// <param name="events"></param>
        /// <param name="stage"></param>
        /// <param name="entityLogicalName"></param>
        /// <param name="message"></param>
        public static void AddEvent(this List<RegisteredEvent> events, PipelineStage stage, string entityLogicalName, MessageType message)
        {
            events.AddEvent(stage, entityLogicalName, message, null);
        }

        /// <summary>
        /// Runs against the specified entity
        /// </summary>
        /// <param name="events"></param>
        /// <param name="stage"></param>
        /// <param name="entityLogicalName"></param>
        /// <param name="message"></param>
        /// <param name="execute"></param>
        public static void AddEvent(this List<RegisteredEvent> events, PipelineStage stage, string entityLogicalName, MessageType message, Action<IExtendedPluginContext> execute){
            events.Add(new RegisteredEvent(stage, message, execute, entityLogicalName));
        }

#endregion AddEvent

#region AddEvents

        /// <summary>
        /// Defaults the execute method to be InternalExecute and run against all entities.
        /// </summary>
        /// <param name="events"></param>
        /// <param name="stage"></param>
        /// <param name="messages"></param>
        public static void AddEvents(this List<RegisteredEvent> events, PipelineStage stage, params MessageType[] messages)
        {
            events.AddEvents(stage, null, null, messages);
        }

        /// <summary>
        /// Defaults the execute method to be InternalExecute and runs against the specified entity.
        /// </summary>
        /// <param name="events"></param>
        /// <param name="stage"></param>
        /// <param name="messages"></param>
        /// <param name="entityLogicalName"></param>
        public static void AddEvents(this List<RegisteredEvent> events, PipelineStage stage, string entityLogicalName, params MessageType[] messages)
        {
            events.AddEvents(stage, entityLogicalName, null, messages);
        }

        /// <summary>
        /// Runs against all entities.
        /// </summary>
        /// <param name="events"></param>
        /// <param name="stage"></param>
        /// <param name="messages"></param>
        /// <param name="execute"></param>
        public static void AddEvents(this List<RegisteredEvent> events, PipelineStage stage, Action<IExtendedPluginContext> execute, params MessageType[] messages)
        {
            events.AddEvents(stage, null, execute, messages);
        }


        /// <summary>
        /// Runs against the specified entity
        /// </summary>
        /// <param name="events"></param>
        /// <param name="stage"></param>
        /// <param name="entityLogicalName"></param>
        /// <param name="execute"></param>
        /// <param name="messages"></param>
        public static void AddEvents(this List<RegisteredEvent> events, PipelineStage stage, string entityLogicalName, Action<IExtendedPluginContext> execute, params MessageType[] messages)
        {
            foreach (var message in messages)
            {
                events.Add(new RegisteredEvent(stage, message, execute, entityLogicalName));
            }
        }

#endregion AddEvent

#endregion List<RegisteredEvent>

#region IExtendedPluginContext

#region GetContextInfo

        /// <summary>
        /// Gets the context information.
        /// </summary>
        /// <returns></returns>
        public static string GetContextInfo(this IExtendedPluginContext context)
        {
            return
                "**** Context Info ****" + Environment.NewLine +
                "Plugin: " + context.PluginTypeName + Environment.NewLine +
                "* Registered Event *" + Environment.NewLine + context.Event.ToString("   ") + Environment.NewLine +
                context.ToStringDebug();
        }

#endregion GetContextInfo

        /// <summary>
        /// Determines whether a shared variable exists that specifies that the plugin or the plugin and specific message type should be prevented from executing.
        /// This is used in conjunction with PreventPluginExecution
        /// </summary>
        /// <returns></returns>
        public static bool HasPluginExecutionBeenPrevented(this IExtendedPluginContext context)
        {
            return context.HasPluginExecutionBeenPreventedInternal(context.Event, GetPreventPluginSharedVariableName(context.PluginTypeName));
        }

        /// <summary>
        /// Cast the Target to the given Entity Type T. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetTarget<T>(this IExtendedPluginContext context) where T : Entity
        {
            // Obtain the target business entity from the input parmameters.
            try
            {
                return ((IPluginExecutionContext)context).GetTarget<T>();
            }
            catch (Exception ex)
            {
                context.LogException(ex);
            }
            return null;
        }

#endregion IExtendedPluginContext

#region IPluginExecutionContext

#region AssertEntityImageAttributesExist

        /// <summary>
        /// Checks the Pre/Post Entity Images to determine if the image collections contains an image with the given key, that contains the attributes.
        /// Throws an exception if the image name is contained in both the Pre and Post Image.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <param name="attributeNames">The attribute names.</param>
        public static void AssertEntityImageAttributesExist(this IPluginExecutionContext context, string imageName, params string[] attributeNames)
        {
            AssertEntityImageRegistered(context, imageName);
            var imageCollection = context.PreEntityImages.TryGetValue(imageName, out Entity preImage) ?
                InvalidPluginStepRegistrationException.ImageCollection.Pre :
                InvalidPluginStepRegistrationException.ImageCollection.Post;
            context.PostEntityImages.TryGetValue(imageName, out Entity postImage);

            var image = preImage ?? postImage;
            var missingAttributes = attributeNames.Where(attribute => !image.Contains(attribute)).ToList();

            if (missingAttributes.Any())
            {
                throw InvalidPluginStepRegistrationException.ImageMissingRequiredAttributes(imageCollection, imageName, missingAttributes);
            }
        }

#endregion AssertEntityImageAttributesExist

#region AssertEntityImageRegistered

        /// <summary>
        /// Checks the Pre/Post Entity Images to determine if the the collection contains an image with the given key.
        /// Throws an exception if the image name is contained in both the Pre and Post Image.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="imageName">Name of the image.</param>
        public static void AssertEntityImageRegistered(this IPluginExecutionContext context, string imageName)
        {
            var pre = context.PreEntityImages.ContainsKey(imageName);
            var post =  context.PostEntityImages.ContainsKey(imageName);

            if (pre && post)
            {
                throw new Exception($"Both Preimage and Post Image Contain the Image \"{imageName}\".  Unable to determine what entity collection to search for the given attributes.");
            }

            if (!pre && !post)
            {
                throw InvalidPluginStepRegistrationException.ImageMissing(imageName);
            }
        }

#endregion AssertEntityImageRegistered

#region CalledFrom

        /// <summary>
        /// Returns true if the current plugin maps to the Registered Event, or the current plugin has been triggered by the given registered event
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="event">The event.</param>
        /// <returns></returns>
        public static bool CalledFrom(this IPluginExecutionContext context, RegisteredEvent @event)
        {
            return context.GetContexts().Any(c => c.MessageName == @event.MessageName && c.PrimaryEntityName == @event.EntityLogicalName && c.Stage == (int)@event.Stage);
        }

        /// <summary>
        /// Returns true if the current plugin maps to the parameters given, or the current plugin has been triggered by the given parameters
        /// </summary>
        /// <returns></returns>
        public static bool CalledFrom(this IPluginExecutionContext context, string entityLogicalName = null, MessageType message = null, int? stage = null)
        {
            if (message == null && entityLogicalName == null && stage == null)
            {
                throw new Exception("At least one parameter for IPluginExecutionContext.CalledFrom must be populated");
            }
            return context.GetContexts().Any(c =>
                (message == null || c.MessageName == message.Name) &&
                (entityLogicalName == null || c.PrimaryEntityName == entityLogicalName) &&
                (stage == null || c.Stage == stage.Value));
        }

#endregion CalledFrom

#region CoalesceTarget

        /// <summary>
        /// Creates a new Entity of type T, adding the attributes from both the Target and the Post Image if they exist.
        /// If imageName is null, the first non-null image found is used.
        /// Does not return null.
        /// Does not return a reference to Target
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="imageName">Name of the image.</param>
        /// <returns></returns>
        public static T CoalesceTargetWithPreEntity<T>(this IPluginExecutionContext context, string imageName = null) where T : Entity
        {
            return DereferenceTarget<T>(context).CoalesceEntity(context.GetPreEntity<T>(imageName));
        }

        /// <summary>
        /// Creates a new Entity of type T, adding the attributes from both the Target and the Post Image if they exist.
        /// If imageName is null, the first non-null image found is used.
        /// Does not return null.
        /// Does not return a reference to Target
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="imageName">Name of the image.</param>
        /// <returns></returns>
        public static T CoalesceTargetWithPostEntity<T>(this IPluginExecutionContext context, string imageName = null) where T : Entity
        {
            return DereferenceTarget<T>(context).CoalesceEntity(context.GetPostEntity<T>(imageName));
        }

#endregion CoalesceTarget

#region GetContexts

        /// <summary>
        /// The current PluginExecutionContext and the parent context hierarchy of the plugin.
        /// </summary>
        public static IEnumerable<IPluginExecutionContext> GetContexts(this IPluginExecutionContext context)
        {
            yield return context;
            foreach (var parent in context.GetParentContexts())
            {
                yield return parent;
            }
        }

        /// <summary>
        /// Iterates through all parent contexts.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static IEnumerable<IPluginExecutionContext> GetParentContexts(this IPluginExecutionContext context)
        {
            var parent = context.ParentContext;
            while (parent != null)
            {
                yield return parent;
                parent = parent.ParentContext;
            }
        }

#endregion GetContexts

        /// <summary>
        /// Gets the event by iterating over all of the expected registered events to ensure that the plugin has been invoked by an expected event.
        /// For any given plug-in event at an instance in time, we would expect at most 1 result to match.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="events">The events.</param>
        /// <returns></returns>
        public static RegisteredEvent GetEvent(this IPluginExecutionContext context, IEnumerable<RegisteredEvent> events)
        {
            return events.Where(
                    e =>
                        (int) e.Stage == context.Stage
                        && (e.MessageName == context.MessageName || e.Message == RegisteredEvent.Any)
                        && (string.IsNullOrWhiteSpace(e.EntityLogicalName) || e.EntityLogicalName == context.PrimaryEntityName))
                .OrderBy(e => e.MessageName == RegisteredEvent.Any) // Favor the specific message match first
                .FirstOrDefault();
        }

#region GetFirstSharedVariable

        /// <summary>
        /// Gets the variable value from the PluginExecutionContext.SharedVariables or anywhere in the Plugin Context Hierarchy collection, cast to type 'T', or default(T) if the collection doesn't contain a variable with the given name.
        /// </summary>
        /// <typeparam name="T">Type of the variable to be returned</typeparam>
        /// <param name="context"></param>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public static T GetFirstSharedVariable<T>(this IPluginExecutionContext context, string variableName)
        {
            while (context != null)
            {
                if (context.SharedVariables.ContainsKey(variableName))
                {
                    return context.SharedVariables.GetParameterValue<T>(variableName);
                }
                context = context.ParentContext;
            }
            return default(T);
        }

        /// <summary>
        /// Gets the variable value from the PluginExecutionContext.SharedVariables or anywhere in the Plugin Context Hierarchy collection, or null if the collection doesn't contain a variable with the given name.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public static object GetFirstSharedVariable(this IPluginExecutionContext context, string variableName)
        {
            while (context != null)
            {
                if (context.SharedVariables.ContainsKey(variableName))
                {
                    return context.SharedVariables.GetParameterValue(variableName);
                }
                context = context.ParentContext;
            }
            return null;
        }

#endregion GetFirstSharedVariable

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static MessageType GetMessageType(this IPluginExecutionContext context) { return new MessageType(context.MessageName); }

#region Get(Pre/Post)Entities

        /// <summary>
        /// If the imageName is populated and the PreEntityImages contains the given imageName Key, the Value is cast to the Entity type T, else null is returned
        /// If the imageName is not populated, than the first image in PreEntityImages with a value, is cast to the Entity type T, else null is returned
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="imageName"></param>
        /// <returns></returns>
        public static T GetPreEntity<T>(this IExecutionContext context, string imageName = null) where T : Entity
        {
            return context.PreEntityImages.GetEntity<T>(imageName, DLaBExtendedPluginContextBase.PluginImageNames.PreImage);
        }

        /// <summary>
        /// If the imageName is populated and the PostEntityImages contains the given imageName Key, the Value is cast to the Entity type T, else null is returned
        /// If the imageName is not populated, than the first image in PostEntityImages with a value, is cast to the Entity type T, else null is returned
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="imageName"></param>
        /// <returns></returns>
        public static T GetPostEntity<T>(this IExecutionContext context, string imageName = null) where T : Entity
        {
            return context.PostEntityImages.GetEntity<T>(imageName, DLaBExtendedPluginContextBase.PluginImageNames.PostImage);
        }

#endregion Get(Pre/Post)Entities

        /// <summary>
        /// Gets the pipeline stage.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static PipelineStage GetPipelineStage(this IPluginExecutionContext context) { return (PipelineStage)context.Stage; }

#region GetTarget

        /// <summary>
        /// Dereferences the target so an update to it will not cause an update to the actual target and result in a crm update post plugin execution
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private static T DereferenceTarget<T>(IPluginExecutionContext context) where T : Entity
        {
            var target = context.GetTarget<T>();
            return target?.Clone() ?? Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Cast the Target to the given Entity Type T. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static T GetTarget<T>(this IPluginExecutionContext context) where T : Entity
        {
            var parameters = context.InputParameters;
            if (!parameters.ContainsKey(ParameterName.Target) || !(parameters[ParameterName.Target] is Entity))
            {
                return null;
            }

            // Obtain the target business entity from the input parmameters.

            return ((Entity)parameters[ParameterName.Target]).AsEntity<T>();
        }

        /// <summary>
        /// Finds and returns the Target as an Entity Reference (Delete Plugins)
        /// </summary>
        /// <returns></returns>
        public static EntityReference GetTargetEntityReference(this IPluginExecutionContext context)
        {
            EntityReference entity = null;
            var parameters = context.InputParameters;
            if (parameters.ContainsKey(ParameterName.Target) &&
                 parameters[ParameterName.Target] is EntityReference)
            {
                entity = (EntityReference)parameters[ParameterName.Target];
            }
            return entity;
        }

#endregion GetTarget

#region Prevent Plugin Execution

#region PreventPluginExecution

        /// <summary>
        /// Adds a shared Variable to the context that is checked by the GenericPluginBase to determine if it should be skipped  * NOTE * The Plugin has to finish executing for the Shared Variable to be passed to a new plugin
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="pluginTypeFullName">The Full Type Name of the Plugin to Prevent</param>
        /// <param name="messageType">Type of the message.</param>
        public static void PreventPluginExecution(this IPluginExecutionContext context, string pluginTypeFullName, MessageType messageType)
        {
            context.PreventPluginExecution(pluginTypeFullName, messageType.Name);
        }

        /// <summary>
        /// Adds a shared Variable to the context that is checked by the GenericPluginBase to determine if it should be skipped  * NOTE * The Plugin has to finish executing for the Shared Variable to be passed to a new plugin
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="pluginTypeFullName">The Full Type Name of the Plugin to Prevent</param>
        /// <param name="event">Type of the event.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void PreventPluginExecution(this IPluginExecutionContext context, string pluginTypeFullName, RegisteredEvent @event)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));

            context.PreventPluginExecution(pluginTypeFullName, @event.MessageName, @event.EntityLogicalName, @event.Stage);
        }

        /// <summary>
        /// Adds a shared Variable to the context that is checked by the GenericPluginBase to determine if it should be skipped  * NOTE * The Plugin has to finish executing for the Shared Variable to be passed to a new plugin
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="pluginTypeFullName">The Full Type Name of the Plugin to Prevent</param>
        /// <param name="messageName">Name of the message.</param>
        /// <param name="logicalName">Name of the logical.</param>
        /// <param name="stage">The stage.</param>
        public static void PreventPluginExecution(this IPluginExecutionContext context, string pluginTypeFullName, string messageName = null, string logicalName = null, PipelineStage? stage = null)
        {
            var preventionName = GetPreventPluginSharedVariableName(pluginTypeFullName);
            if (!context.SharedVariables.TryGetValue(preventionName, out object value))
            {
                value = new Entity();
                context.SharedVariables.Add(preventionName, value);
            }

            // Wish I could use a Hash<T> here, but CRM won't serialize it.  I'll Hack the Entity Object for now
            var hash = ((Entity)value).Attributes;
            var rule = GetPreventionRule(messageName, logicalName, stage);
            if (!hash.Contains(rule))
            {
                hash.Add(rule, null);
            }
        }

#endregion PreventPluginExecution

#region PreventPluginExecution<T>

        /// <summary>
        /// Adds a shared Variable to the context that is checked by the GenericPluginBase to determine if it should be skipped  * NOTE * The Plugin has to finish executing for the Shared Variable to be passed to a new plugin
        /// </summary>
        /// <typeparam name="T">The type of the plugin.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="messageType">Type of the message.</param>
        public static void PreventPluginExecution<T>(this IPluginExecutionContext context, MessageType messageType)
            where T : IRegisteredEventsPlugin
        {
            context.PreventPluginExecution<T>(messageType.Name);
        }

        /// <summary>
        /// Adds a shared Variable to the context that is checked by the GenericPluginBase to determine if it should be skipped  * NOTE * The Plugin has to finish executing for the Shared Variable to be passed to a new plugin
        /// </summary>
        /// <typeparam name="T">The type of the plugin.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="event">Type of the event.</param>
        public static void PreventPluginExecution<T>(this IPluginExecutionContext context, RegisteredEvent @event)
            where T : IRegisteredEventsPlugin
        {
            context.PreventPluginExecution(typeof(T).FullName, @event);
        }

        /// <summary>
        /// Adds a shared Variable to the context that is checked by the GenericPluginBase to determine if it should be skipped  * NOTE * The Plugin has to finish executing for the Shared Variable to be passed to a new plugin
        /// </summary>
        /// <typeparam name="T">The type of the plugin.</typeparam>
        public static void PreventPluginExecution<T>(this IPluginExecutionContext context, string messageName = null, string logicalName = null, PipelineStage? stage = null)
            where T : IRegisteredEventsPlugin
        {
            context.PreventPluginExecution(typeof(T).FullName, messageName, logicalName, stage);
        }

#endregion PreventPluginExecution<T>

        private static string GetPreventionRule(string messageName = null, string logicalName = null, PipelineStage? stage = null)
        {
            var rule = messageName == null ? string.Empty : "MessageName:" + messageName + "|";
            rule += logicalName == null ? string.Empty : "LogicalName:" + logicalName + "|";
            rule += stage == null ? string.Empty : "Stage:" + stage + "|";
            return rule;
        }

        private static string GetPreventPluginSharedVariableName(string pluginTypeName)
        {
            return pluginTypeName + "PreventExecution";
        }

#region HasPluginExecutionBeenPrevented

        /// <summary>
        /// Determines whether a shared variable exists that specifies that the plugin or the plugin and specifc message type should be prevented from executing.
        /// This is used in conjunction with PreventPluginExecution
        /// </summary>
        /// <typeparam name="T">The type of the plugin.</typeparam>
        /// <returns></returns>
        public static bool HasPluginExecutionBeenPrevented<T>(this IPluginExecutionContext context, RegisteredEvent @event)
            where T : IRegisteredEventsPlugin
        {
            var preventionName = GetPreventPluginSharedVariableName(typeof(T).FullName);
            return context.HasPluginExecutionBeenPreventedInternal(@event, preventionName);
        }

        private static bool HasPluginExecutionBeenPreventedInternal(this IPluginExecutionContext context, RegisteredEvent @event, string preventionName)
        {
            var value = context.GetFirstSharedVariable(preventionName);
            if (value == null)
            {
                return false;
            }

            var hash = ((Entity)value).Attributes;
            return hash.Contains(string.Empty) ||
                   hash.Contains(GetPreventionRule(@event.MessageName)) ||
                   hash.Contains(GetPreventionRule(@event.MessageName, @event.EntityLogicalName)) ||
                   hash.Contains(GetPreventionRule(@event.MessageName, stage: @event.Stage)) ||
                   hash.Contains(GetPreventionRule(@event.MessageName, @event.EntityLogicalName, @event.Stage)) ||
                   hash.Contains(GetPreventionRule(logicalName: @event.EntityLogicalName)) ||
                   hash.Contains(GetPreventionRule(logicalName: @event.EntityLogicalName, stage: @event.Stage)) ||
                   hash.Contains(GetPreventionRule(stage: @event.Stage));
        }

#endregion HasPluginExecutionBeenPrevented

#endregion Prevent Plugin Execution

#endregion IPluginExecutionContext
    }
}
