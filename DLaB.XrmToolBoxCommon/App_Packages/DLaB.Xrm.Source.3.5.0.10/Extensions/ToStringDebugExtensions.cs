using Microsoft.Xrm.Sdk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using Microsoft.Xrm.Sdk.Query;
#if DLAB_UNROOT_COMMON_NAMESPACE
using DLaB.Common;
#else
using Source.DLaB.Common;
#endif

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif

{
    public static partial class Extensions
    {

        #region byte[]

        /// <summary>
        /// Returns the length of the number of byte array
        /// </summary>
        /// <param name="bits">The Byte Array.</param>
        public static string ToStringDebug(this byte[] bits)
        {
            if (bits == null)
            {
                return "null";
            }

            return "{length: " + bits.Length + "}";
        }

        #endregion byte[]

        #region DataCollection<string, object>

        private static IEnumerable<string> ToStringDebug(this DataCollection<string, object> items, StringDebugInfo info)
        {
            foreach (var item in items.OrderBy(kvp => kvp.Key))
            {
                yield return item.Key + ": " + ObjectToStringDebugInternal(item.Value, info).TrimStart();
            }
        }

        private static string ObjectToStringDebugInternal(object obj, StringDebugInfo info)
        {
            string value;
            switch (obj)
            {
                case null:
                    value = "null";
                    break;

                case ColumnSet cs:
                    value = cs.AllColumns
                        ? "\"ColumnSet(allColumns:true)\""
                        : $"\"{string.Join(",", cs.Columns.OrderBy(c => c))}\"";
                    break;

                case Entity entity:
                    value = entity.ToStringAttributes(info);
                    break;

                case EntityReference entityRef:
                    value = entityRef.ToStringDebug();
                    break;

                case EntityCollection entities:
                    value = entities.ToStringDebug(info);
                    break;

                case EntityReferenceCollection entityRefCollection:
                    value = entityRefCollection.ToStringDebug(info);
                    break;

                case Dictionary<string, string> dict:
                    value = dict.ToStringDebug(info);
                    break;

                case FetchExpression fetch:
                    value = $"\"{fetch.Query.Trim()}\"";
                    break;

                case byte[] imageArray:
                    value = imageArray.ToStringDebug();
                    break;

                case IEnumerable enumerable when !(enumerable is string):
                    value = enumerable.ToStringDebug(info);
                    break;

                case OptionSetValue optionSet:
                    value = optionSet.Value.ToString(CultureInfo.InvariantCulture);
                    break;

                case Money money:
                    value = money.Value.ToString(CultureInfo.InvariantCulture);
                    break;

                case QueryExpression qe:
                    value = $"\"{qe.GetSqlStatement().Trim()}\"";
                    break;

                case bool yesNo:
                    value = yesNo
                        ? "true"
                        : "false";
                    break;

                default:
                    value = obj.IsNumeric()
                        ? obj.ToString()
                        : $"\"{obj}\"";
                    break;
            }

            return value;
        }

        #endregion DataCollection<string, object>

        #region Dictionary<string, string>

        /// <summary>
        /// Iterates and displays the values of the dictionary.
        /// </summary>
        /// <param name="dict">The dictionary.</param>
        /// <param name="info">Optional arguments.</param>
        public static string ToStringDebug(this Dictionary<string, string> dict, StringDebugInfo info = null)
        {
            return Wrap("{",
                dict.Select(e => $@"{e.Key}: ""{e.Value}"""),
                "}",
                info);
        }

        #endregion Dictionary<string, string>

        #region Entity

        /// <summary>
        /// Iterates and displays the attributes listed in the entity's Attributes collection.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="info">Optional arguments.</param>
        public static string ToStringAttributes(this Entity entity, StringDebugInfo info = null)
        {
            info = info ?? StringDebugInfo.Default;
            if (entity == null)
            {
                return info.Indent + "null";
            }

            return Wrap("{",
                entity.Attributes.ToStringDebug(info),
                "}",
                info);
        }

        /// <summary>
        /// Iterates and displays the attributes listed in the entity's Attributes collection as well as the Id and LogicalName.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="info">Optional arguments.</param>
        public static string ToStringDebug(this Entity entity, StringDebugInfo info = null)
        {
            info = info ?? StringDebugInfo.Default;
            if (entity == null)
            {
                return info.Indent + "null";
            }

            return Wrap("{",
                new []
                {
                     GetIdString(entity), 
                    "LogicalName: \"" + entity.LogicalName + "\""
                }.Concat(
                    entity.Attributes.ToStringDebug(info)
                ),
                "}",
                info);
        }

        #endregion Entity

        #region EntityCollection

        /// <summary>
        /// Iterates and displays the entities in the entity collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="info">Optional arguments.</param>
        public static string ToStringDebug(this EntityCollection collection, StringDebugInfo info = null)
        {
            info = info ?? StringDebugInfo.Default;
            if (collection == null)
            {
                return info.Indent + "null";
            }

            var values = collection.Entities.Select(r => r.ToStringAttributes(info.IncreaseIndent()).TrimStart());
            if (string.IsNullOrWhiteSpace(collection.EntityName))
            {
                return Wrap("[",
                    values,
                    "]",
                    info,
                    string.Empty);
            }

            var prefix = info.SingleLine
                ? string.Empty
                : Environment.NewLine + info.Indent;

            var start = $"{{{prefix}{collection.EntityName}:{prefix}[";
            var end = info.SingleLine
                ? "]}"
                : $"]{Environment.NewLine + info.Indent}}}";
            return Wrap(start,
                collection.Entities.Select(r => r.ToStringAttributes(info)),
                end,
                info,
                string.Empty);
        }

        #endregion EntityCollection

        #region EntityImageCollection

        /// <summary>
        /// Iterates and displays the entity images in the entity image collection.
        /// </summary>
        /// <param name="images">The images.</param>
        /// <param name="name">The name.</param>
        /// <param name="info">Optional arguments.</param>
        public static string ToStringDebug(this EntityImageCollection images, string name, StringDebugInfo info = null)
        {
            info = info ?? StringDebugInfo.Default;
            if (images == null)
            {
                return info.Indent + name + ": null";
            }

            
            if (images.Count == 1)
            {
                var kvp = images.First();
                var id = kvp.Value?.Id == Guid.Empty
                    ? string.Empty
                    : "_" + kvp.Value?.LogicalName + "_" + kvp.Value?.Id.ToString("N");
                return info.Indent + name + id + ": " + images.Values.First().ToStringAttributes(info).TrimStart();
            }

            return Wrap(name + ": {",
                images.Select(v => v.Key + ": " + v.Value.ToStringDebug(info).TrimStart()),
                "}",
                info.WithNoTab(),
                string.Empty);
        }

        #endregion EntityImageCollection

        #region EntityReference

        /// <summary>
        /// Returns the Logical Name, Name, and Id of the EntityReference
        /// </summary>
        /// <param name="entity">The EntityReference.</param>
        /// <returns></returns>
        public static string ToStringDebug(this EntityReference entity)
        {
            var name = string.IsNullOrWhiteSpace(entity.Name)
                ? string.Empty  
                : $"Name: \"{entity.Name}\", ";
            return "{" + name + GetIdString(entity) + "}";
        }

        private static string GetIdString(EntityReference entity)
        {
#if PRE_KEYATTRIBUTE
            var required = $"LogicalName: \"{entity.LogicalName}\", Id: \"{entity.Id}\"";
#else
            var required = $"LogicalName: \"{entity.LogicalName}\", {ToIdString(entity.Id, entity.KeyAttributes)}";
#endif
            return required;
        }

        private static string GetIdString(Entity entity)
        {
#if PRE_KEYATTRIBUTE
            return $"Id: \"{entity.Id}\"";
#else
            return ToIdString(entity.Id, entity.KeyAttributes);
#endif
        }


#if !PRE_KEYATTRIBUTE
        private static string ToIdString(Guid id, KeyAttributeCollection keys)
        {
            var parts = new List<string>();
            var keyCount = keys?.Count;
            if (keyCount > 0 && id == Guid.Empty)
            {
                if (keyCount == 1)
                {
                    var kvp = keys.First();
                    parts.Add($"Key: \"{kvp.Key}\"");
                    parts.Add($"Value: \"{kvp.Value}\"");
                }
                else
                {
                    var info = new StringDebugInfo(singleLine: true);
                    parts.Add(Wrap("Keys: {",
                        keys.ToStringDebug(info),
                        "}",
                        info,
                        null,
                        ", "));
                }
            }
            else
            {
                parts.Add($"Id: \"{id}\"");
            }

            return string.Join(", ", parts);
        }

#endif
        #endregion EntityReference

        #region EntityReferenceCollection

        /// <summary>
        /// Iterates and displays the entity references in the entity reference collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="info">Optional arguments.</param>
        public static string ToStringDebug(this EntityReferenceCollection collection, StringDebugInfo info = null)
        {
            if (collection == null)
            {
                return "null";
            }

            return Wrap("[",
                collection.Select(r => r.ToStringDebug()),
                "]",
                info);
        }

        #endregion EntityReferenceCollection

        #region IEnumerable

        /// <summary>
        /// Iterates and displays the values in the IEnumerable.
        /// </summary>
        /// <param name="collection">The images.</param>
        /// <param name="info">Optional arguments.</param>
        public static string ToStringDebug(this IEnumerable collection, StringDebugInfo info = null)
        {
            info = info ?? StringDebugInfo.Default;
            var prefix = info.SingleLine
                ? string.Empty
                : Environment.NewLine + info.Indent + info.Tab;

            var start = $"{{{prefix}\"{collection.GetType().Name}\": [";
            var end = info.SingleLine
                ? "]}"
                : $"]{Environment.NewLine + info.Indent}}}";
            return Wrap(start,
                from object item in collection select ObjectToStringDebugInternal(item, info),
                end,
                info.IncreaseIndent());
        }

        #endregion IEnumerable

        #region IExecutionContext

        /// <summary>
        /// Returns an in depth view of the context
        /// </summary>
        /// <param name="context">The Context</param>
        /// <returns></returns>
        internal static List<string> ToStringDebug(this IExecutionContext context)
        {
            var info = StringDebugInfo.Default;
            var lines = new List<string>
            {
                "BusinessUnitId: " + context.BusinessUnitId,
                "CorrelationId: " + context.CorrelationId,
                "Depth: " + context.Depth,
                "InitiatingUserId: " + context.InitiatingUserId,
                "IsInTransaction: " + context.IsInTransaction,
                "IsolationMode: " + context.IsolationMode,
                "MessageName: " + context.MessageName,
                "Mode: " + context.Mode,
                "OperationCreatedOn: " + context.OperationCreatedOn,
                "OperationId: " + context.OperationId,
                "Organization: " + context.OrganizationName + "(" + context.OrganizationId + ")",
                "OwningExtension: " + (context.OwningExtension == null ? "Null" : context.OwningExtension.GetNameId()),
                "PrimaryEntityId: " + context.PrimaryEntityId,
                "PrimaryEntityName: " + context.PrimaryEntityName,
                "SecondaryEntityName: " + context.SecondaryEntityName,
                "UserId: " + context.UserId
            };
            lines.Add(context.InputParameters.ToStringDebug("InputParameters", info));
            lines.Add(context.OutputParameters.ToStringDebug("OutputParameters", info));
            lines.Add(context.PostEntityImages.ToStringDebug("PostEntityImages", info));
            lines.Add(context.PreEntityImages.ToStringDebug("PreEntityImages", info));
            lines.Add(context.SharedVariables.ToStringDebug("SharedVariables", info));

            if (ConfigurationManager.AppSettings.AllKeys.Any())
            {
                lines.Add("* App Config Values *");
                lines.AddRange(ConfigurationManager.AppSettings.AllKeys.Select(key => $"    [{key}]: {GetConfigValueMaskingPasswords(key)}"));
            }

            return lines;
        }

        #endregion IExecutionContext

        #region Object

        /// <summary>
        /// Converts the object to a json like string
        /// </summary>
        /// <returns></returns>
        public static string ObjectToStringDebug(this object obj, StringDebugInfo info = null)
        {
            return ObjectToStringDebugInternal(obj, info ?? StringDebugInfo.Default);
        }

        #endregion Object

        #region ParameterCollection

        /// <summary>
        /// Iterates and displays the parameters in the parameter collection.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="name">The name.</param>
        /// <param name="info">Optional arguments.</param>
        public static string ToStringDebug(this ParameterCollection parameters, string name, StringDebugInfo info = null)
        {
            info = info ?? StringDebugInfo.Default;
            if (parameters == null)
            {
                return info.Indent + name + ": null";
            }

            if (parameters.Count <= 0)
            {
                return info.Indent + name + ": {}";
            }

            var allEntities = parameters.Values.All(a => a is Entity);
            return Wrap(info.Indent + name + ": {",
                parameters.ToStringDebug(allEntities ? info: info.IncreaseIndent()),
                "}",
                allEntities ? info.WithNoTab(): info, string.Empty);
        }

        #endregion ParameterCollection

        #region IPluginExecutionContext

        /// <summary>
        /// Returns an in-depth view of the context
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static string ToStringDebug(this IPluginExecutionContext context)
        {
            var lines = ((IExecutionContext) context).ToStringDebug();
            lines.AddRange(new[]
            {
                "Has Parent Context: " + (context.ParentContext != null),
                "Stage: " + context.Stage
            });

            return string.Join(Environment.NewLine, lines);
        }

        #endregion IPluginExecutionContext

        private static bool IsNumeric(this object o)
        {
            return o is byte
                   || o is sbyte
                   || o is ushort
                   || o is uint
                   || o is ulong
                   || o is short
                   || o is int
                   || o is long
                   || o is float
                   || o is double
                   || o is decimal;
        }

        internal static string GenerateNonBreakingSpace(int spaces)
        {
            const string space = " "; // This is not a space, it is a Non-Breaking Space (alt+255).  In the log things get trimmed, and this will prevent that from happening;
            return new string(space[0], spaces);
        }

        private static string Wrap(string start, IEnumerable<string> middle, string end, StringDebugInfo info, string initialIndent = null, string middleJoinSeparator = ",")
        {
            initialIndent = initialIndent ?? info.Indent;
            if (info.SingleLine)
            {
                return initialIndent + start + string.Join(middleJoinSeparator, middle) + end;
            }

            var tab = Environment.NewLine + info.Indent + info.Tab;
            var joinedMiddle = string.Join(middleJoinSeparator + tab, middle);
            if (string.IsNullOrWhiteSpace(joinedMiddle))
            {
                return initialIndent + start + end;
            }

            return initialIndent
                   + start
                   + tab + joinedMiddle + Environment.NewLine
                   + info.Indent
                   + end;
        }

        private static string GetConfigValueMaskingPasswords(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrWhiteSpace(value) && key.ContainsIgnoreCase("password"))
            {
                value = new string('*', value.Length);
            }

            return value;
        }

    }

    /// <summary>
    /// Used to control indenting/formatting of StringDebug/ToStringAttributes Methods
    /// </summary>
    public class StringDebugInfo
    {
        /// <summary>
        /// Number of Spaces of the current indent.
        /// </summary>
        public int IndentSpaces { get; }
        /// <summary>
        /// Width of a single Tab.
        /// </summary>
        public int TabWidth { get; }
        /// <summary>
        /// If the ToString should output on a single line.
        /// </summary>
        public bool SingleLine { get; }

        /// <summary>
        /// Indent string.
        /// </summary>
        public string Indent { get; }
        /// <summary>
        /// Tab string.
        /// </summary>
        public string Tab { get; }
        private Dictionary<int,StringDebugInfo> IncreasedIndents { get; set; }
        private StringDebugInfo NoTab { get; set; }

        /// <summary>
        /// Default StringDebugInfo.
        /// </summary>
        public static StringDebugInfo Default = new StringDebugInfo();

        /// <summary>
        /// Creates a new StringDebugInfo
        /// </summary>
        /// <param name="indentSpaces">Number of Spaces of the Indent.</param>
        /// <param name="tabWidth">Number of Spaces of a Tab.</param>
        /// <param name="singleLine">If the ToStringDebug should be a single line.</param>
        public StringDebugInfo(int indentSpaces = 0, int tabWidth = 2, bool singleLine = false)
            : this (null, indentSpaces, tabWidth, singleLine)
        {
        }

        private StringDebugInfo(StringDebugInfo toCopy, int? indentSpaces = null, int? tabWidth = null, bool? singleLine = null)
        {
            IndentSpaces = indentSpaces ?? toCopy.IndentSpaces;
            TabWidth = tabWidth ?? toCopy.TabWidth;
            SingleLine = singleLine ?? toCopy.SingleLine;

            Indent = Extensions.GenerateNonBreakingSpace(IndentSpaces );
            Tab = Extensions.GenerateNonBreakingSpace(TabWidth);
            IncreasedIndents = new Dictionary<int, StringDebugInfo>();
        }

        /// <summary>
        /// Increases the number of Tabs to increase the Current indent
        /// </summary>
        /// <param name="tabs"></param>
        /// <returns></returns>
        public StringDebugInfo IncreaseIndent(int tabs = 1)
        {
            if(!IncreasedIndents.TryGetValue(tabs, out var indent))
            {
                indent = new StringDebugInfo(this, IndentSpaces + TabWidth * tabs);
                IncreasedIndents[tabs] = indent;
            }

            return indent;
        }

        /// <summary>
        /// Returns a StringDebugInfo with no tabs.
        /// </summary>
        /// <returns></returns>
        public StringDebugInfo WithNoTab()
        {
            if (this.TabWidth == 0)
            {
                return this;
            }
            return NoTab ?? (NoTab = new StringDebugInfo(this, tabWidth: 0));
        }
    }
}

