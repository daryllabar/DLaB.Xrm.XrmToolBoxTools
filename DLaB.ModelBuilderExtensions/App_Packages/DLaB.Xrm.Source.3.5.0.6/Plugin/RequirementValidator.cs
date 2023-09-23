using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk.Query;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
using DLaB.Common;
using DLaB.Xrm.Comparers;

namespace DLaB.Xrm.Plugin
#else
using Source.DLaB.Common;
using Source.DLaB.Xrm.Comparers;

namespace Source.DLaB.Xrm.Plugin
#endif
{
    /// <summary>
    /// Used to validate requirements
    /// </summary>
    public class RequirementValidator: IRequirementValidator
    {
        private Dictionary<ContextEntity, Requirement> Requirements { get; } = new Dictionary<ContextEntity,Requirement>();

        /// <summary>
        /// The reason why the requirement was not met.  SkipExecution must be called first.
        /// </summary>
        public InvalidRequirementReason Reason { get; protected set; }

        /// <summary>
        /// Returns true if the context does not meet the requirements for execution
        /// </summary>
        /// <param name="context">The Context</param>
        /// <returns></returns>
        public bool SkipExecution(IExtendedPluginContext context)
        {
            foreach (var requirement in Requirements.Values)
            {
                if (requirement.SkipExecution(context))
                {
                    Reason = requirement.Reason;
                    return true;
                }
            }
            return false;
        }


        #region Contains

        #region Non-null

        /// <summary>
        /// Requires all columns are in the attributes collection with a non-null value
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="columnNames">The column names</param>
        /// <returns></returns>
        public RequirementValidator Contains(ContextEntity entityType, params string[] columnNames)
        {
            Get(entityType).RequiredColumns.AddMissing(columnNames);
            return this;
        }

        /// <summary>
        /// Requires all columns are in the attributes collection with a non-null value
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="columns">The column names</param>
        /// <returns></returns>
        public RequirementValidator Contains(ContextEntity entityType, ColumnSet columns)
        {
            Get(entityType).RequiredColumns.AddMissing(columns.Columns);
            return this;
        }

        /// <summary>
        /// Requires all columns are in the attributes collection with a non-null value
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="anonymousTypeInitializer">The type initializer</param>
        /// <returns></returns>
        public RequirementValidator Contains<T>(ContextEntity entityType, Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
        {
            return Contains(entityType, anonymousTypeInitializer.GetAttributeNamesArray());
        }

        /// <summary>
        /// Requires at least one column is in the attributes collection with a non-null value
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="columnNames">The column names</param>
        /// <returns></returns>
        public RequirementValidator ContainsAny(ContextEntity entityType, params string[] columnNames)
        {
            Get(entityType).RequiredOrColumns.Add(new List<string>(columnNames));
            return this;
        }

        /// <summary>
        /// Requires at least one column is in the attributes collection with a non-null value
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="columns">The column names</param>
        /// <returns></returns>
        public RequirementValidator ContainsAny(ContextEntity entityType, ColumnSet columns)
        {
            Get(entityType).RequiredOrColumns.Add(columns.Columns.ToList());
            return this;
        }

        /// <summary>
        /// Requires at least one column is in the attributes collection with a non-null value
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="anonymousTypeInitializer">The type initializer</param>
        /// <returns></returns>
        public RequirementValidator ContainsAny<T>(ContextEntity entityType, Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
        {
            return ContainsAny(entityType, anonymousTypeInitializer.GetAttributeNamesArray());
        }

        #endregion Non-null

        #region Nullable

        /// <summary>
        /// Requires all columns are in the attributes collection (Allows nulls)
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="columnNames">The column names</param>
        /// <returns></returns>
        public RequirementValidator ContainsNullable(ContextEntity entityType, params string[] columnNames)
        {
            Get(entityType).RequiredColumnsAllowNulls.AddMissing(columnNames);
            return this;
        }

        /// <summary>
        /// Requires all columns are in the attributes collection (Allows nulls)
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="columns">The column names</param>
        /// <returns></returns>
        public RequirementValidator ContainsNullable(ContextEntity entityType, ColumnSet columns)
        {
            Get(entityType).RequiredColumnsAllowNulls.AddMissing(columns.Columns);
            return this;
        }

        /// <summary>
        /// Requires all columns are in the attributes collection (Allows nulls)
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="anonymousTypeInitializer">The type initializer</param>
        /// <returns></returns>
        public RequirementValidator ContainsNullable<T>(ContextEntity entityType, Expression<Func<T, object>> anonymousTypeInitializer) where T: Entity
        {
            return ContainsNullable(entityType, anonymousTypeInitializer.GetAttributeNamesArray());
        }

        /// <summary>
        /// Requires at least one column is in the attributes collection (Allows nulls)
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="columnNames">The column names</param>
        /// <returns></returns>
        public RequirementValidator ContainsAnyNullable(ContextEntity entityType, params string[] columnNames)
        {
            Get(entityType).RequiredOrColumnsAllowNulls.Add(new List<string>(columnNames));
            return this;
        }

        /// <summary>
        /// Requires at least one column is in the attributes collection (Allows nulls)
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="columns">The column names</param>
        /// <returns></returns>
        public RequirementValidator ContainsAnyNullable(ContextEntity entityType, ColumnSet columns)
        {
            Get(entityType).RequiredOrColumnsAllowNulls.Add(columns.Columns.ToList());
            return this;
        }

        /// <summary>
        /// Requires at least one column is in the attributes collection (Allows nulls)
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="anonymousTypeInitializer">The type initializer</param>
        /// <returns></returns>
        public RequirementValidator ContainsAnyNullable<T>(ContextEntity entityType, Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
        {
            return ContainsAnyNullable(entityType, anonymousTypeInitializer.GetAttributeNamesArray());
        }

        #endregion Nullable

        #region Null

        /// <summary>
        /// Requires all columns are in the attributes collection with a null value
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="columnNames">The column names</param>
        /// <returns></returns>
        public RequirementValidator ContainsNull(ContextEntity entityType, params string[] columnNames)
        {
            Get(entityType).RequiredNullColumns.AddMissing(columnNames);
            return this;
        }

        /// <summary>
        /// Requires all columns are in the attributes collection with a null value
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="columns">The column names</param>
        /// <returns></returns>
        public RequirementValidator ContainsNull(ContextEntity entityType, ColumnSet columns)
        {
            Get(entityType).RequiredNullColumns.AddMissing(columns.Columns);
            return this;
        }

        /// <summary>
        /// Requires all columns are in the attributes collection with a null value
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="anonymousTypeInitializer">The type initializer</param>
        /// <returns></returns>
        public RequirementValidator ContainsNull<T>(ContextEntity entityType, Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
        {
            return ContainsNull(entityType, anonymousTypeInitializer.GetAttributeNamesArray());
        }

        /// <summary>
        /// Requires at least one column is in the attributes collection with a null value
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="columnNames">The column names</param>
        /// <returns></returns>
        public RequirementValidator ContainsAnyNull(ContextEntity entityType, params string[] columnNames)
        {
            Get(entityType).RequiredNullOrColumns.Add(new List<string>(columnNames));
            return this;
        }

        /// <summary>
        /// Requires at least one column is in the attributes collection with a null value
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="columns">The column names</param>
        /// <returns></returns>
        public RequirementValidator ContainsAnyNull(ContextEntity entityType, ColumnSet columns)
        {
            Get(entityType).RequiredNullOrColumns.Add(columns.Columns.ToList());
            return this;
        }

        /// <summary>
        /// Requires at least one column is in the attributes collection with a null value
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="anonymousTypeInitializer">The type initializer</param>
        /// <returns></returns>
        public RequirementValidator ContainsAnyNull<T>(ContextEntity entityType, Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
        {
            return ContainsAnyNull(entityType, anonymousTypeInitializer.GetAttributeNamesArray());
        }

        #endregion Null

        #region Value

        /// <summary>
        /// Requires all columns to have the specified values
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="entity">The entity with Attributes to require to have been to</param>
        /// <returns></returns>
        public RequirementValidator Contains(ContextEntity entityType, Entity entity)
        {
            Get(entityType).ContainedValues.Add(entity);
            return this;
        }

        /// <summary>
        /// Requires at least one column to have the specified value
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="entity">The entity with Attributes to require to have been to</param>
        /// <returns></returns>
        public RequirementValidator ContainsAny(ContextEntity entityType, Entity entity)
        {
            Get(entityType).ContainedOrValues.Add(entity);
            return this;
        }

        #endregion Value

        #endregion Contains

        #region Updated (Non-Null) / Changed (Nullable) / Cleared (Null) / Updated (value)

        #region Updated (Non-Null)

        /// <summary>
        /// Requires all columns have been updated to a non-null value that is different than the pre-image value
        /// </summary>
        /// <param name="columnNames">The column names</param>
        /// <returns></returns>
        public RequirementValidator Updated(params string[] columnNames)
        {
            Get(ContextEntity.Target).UpdatedColumns.AddMissing(columnNames);
            return this;
        }

        /// <summary>
        /// Requires all columns have been updated to a non-null value that is different than the pre-image value
        /// </summary>
        /// <param name="columns">The column names</param>
        /// <returns></returns>
        public RequirementValidator Updated(ColumnSet columns)
        {
            Get(ContextEntity.Target).UpdatedColumns.AddMissing(columns.Columns);
            return this;
        }

        /// <summary>
        /// Requires all columns to have been updated to a non-null value that is different than the pre-image value
        /// </summary>
        /// <param name="anonymousTypeInitializer">The type initializer</param>
        /// <returns></returns>
        public RequirementValidator Updated<T>(Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
        {
            return Updated(anonymousTypeInitializer.GetAttributeNamesArray());
        }

        /// <summary>
        /// Requires at least one column has been updated to a non-null value that is different than the pre-image value
        /// </summary>
        /// <param name="columnNames">The column names</param>
        /// <returns></returns>
        public RequirementValidator UpdatedAny(params string[] columnNames)
        {
            Get(ContextEntity.Target).UpdatedOrColumns.Add(new List<string>(columnNames));
            return this;
        }

        /// <summary>
        /// Requires at least one column has been updated to a non-null value that is different than the pre-image value
        /// </summary>
        /// <param name="columns">The column names</param>
        /// <returns></returns>
        public RequirementValidator UpdatedAny(ColumnSet columns)
        {
            Get(ContextEntity.Target).UpdatedOrColumns.Add(columns.Columns.ToList());
            return this;
        }

        /// <summary>
        /// Requires at least one column has been updated to a non-null value that is different than the pre-image value
        /// </summary>
        /// <param name="anonymousTypeInitializer">The type initializer</param>
        /// <returns></returns>
        public RequirementValidator UpdatedAny<T>(Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
        {
            return UpdatedAny(anonymousTypeInitializer.GetAttributeNamesArray());
        }

        #endregion Updated (Non-Null)

        #region Changed (Nullable)

        /// <summary>
        /// Requires all columns to have been updated to a value that is different than the pre-image (allows updating to null)
        /// </summary>
        /// <param name="columnNames">The column names</param>
        /// <returns></returns>
        public RequirementValidator Changed(params string[] columnNames)
        {
            Get(ContextEntity.Target).UpdatedColumnsAllowNulls.AddMissing(columnNames);
            return this;
        }

        /// <summary>
        /// Requires all columns to have been updated to a value that is different than the pre-image (allows updating to null)
        /// </summary>
        /// <param name="columns">The column names</param>
        /// <returns></returns>
        public RequirementValidator Changed(ColumnSet columns)
        {
            Get(ContextEntity.Target).UpdatedColumnsAllowNulls.AddMissing(columns.Columns);
            return this;
        }

        /// <summary>
        /// Requires all columns to have been updated to a value that is different than the pre-image (allows updating to null)
        /// </summary>
        /// <param name="anonymousTypeInitializer">The type initializer</param>
        /// <returns></returns>
        public RequirementValidator Changed<T>(Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
        {
            return Changed(anonymousTypeInitializer.GetAttributeNamesArray());
        }

        /// <summary>
        /// Requires at least one column has been updated to a value that is different than the pre-image value (allows updating to null)
        /// </summary>
        /// <param name="columnNames">The column names</param>
        /// <returns></returns>
        public RequirementValidator ChangedAny(params string[] columnNames)
        {
            Get(ContextEntity.Target).UpdatedOrColumnsAllowNulls.Add(new List<string>(columnNames));
            return this;
        }

        /// <summary>
        /// Requires at least one column has been updated to a value that is different than the pre-image value (allows updating to null)
        /// </summary>
        /// <param name="columns">The column names</param>
        /// <returns></returns>
        public RequirementValidator ChangedAny(ColumnSet columns)
        {
            Get(ContextEntity.Target).UpdatedOrColumnsAllowNulls.Add(columns.Columns.ToList());
            return this;
        }

        /// <summary>
        /// Requires at least one column has been updated to a value that is different than the pre-image value (allows updating to null)
        /// </summary>
        /// <param name="anonymousTypeInitializer">The type initializer</param>
        /// <returns></returns>
        public RequirementValidator ChangedAny<T>(Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
        {
            return ChangedAny(anonymousTypeInitializer.GetAttributeNamesArray());
        }

        #endregion Changed (Nullable)

        #region Cleared (Null)

        /// <summary>
        /// Requires all columns to have been updated from a non-null value to a null value
        /// </summary>
        /// <param name="columnNames">The column names</param>
        /// <returns></returns>
        public RequirementValidator Cleared(params string[] columnNames)
        {
            Get(ContextEntity.Target).UpdatedNullColumns.AddMissing(columnNames);
            return this;
        }

        /// <summary>
        /// Requires all columns to have been updated from a non-null value to a null value
        /// </summary>
        /// <param name="columns">The column names</param>
        /// <returns></returns>
        public RequirementValidator Cleared(ColumnSet columns)
        {
            Get(ContextEntity.Target).UpdatedNullColumns.AddMissing(columns.Columns);
            return this;
        }

        /// <summary>
        /// Requires all columns to have been updated from a non-null value to a null value
        /// </summary>
        /// <param name="anonymousTypeInitializer">The type initializer</param>
        /// <returns></returns>
        public RequirementValidator Cleared<T>(Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
        {
            return Cleared(anonymousTypeInitializer.GetAttributeNamesArray());
        }

        /// <summary>
        /// Requires at least one column has been updated from a non-null value to a null value)
        /// </summary>
        /// <param name="columnNames">The column names</param>
        /// <returns></returns>
        public RequirementValidator ClearedAny(params string[] columnNames)
        {
            Get(ContextEntity.Target).UpdatedNullOrColumns.Add(new List<string>(columnNames));
            return this;
        }

        /// <summary>
        /// Requires at least one column has been updated from a non-null value to a null value)
        /// </summary>
        /// <param name="columns">The column names</param>
        /// <returns></returns>
        public RequirementValidator ClearedAny(ColumnSet columns)
        {
            Get(ContextEntity.Target).UpdatedNullOrColumns.Add(columns.Columns.ToList());
            return this;
        }

        /// <summary>
        /// Requires at least one column has been updated from a non-null value to a null value)
        /// </summary>
        /// <param name="anonymousTypeInitializer">The type initializer</param>
        /// <returns></returns>
        public RequirementValidator ClearedAny<T>(Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
        {
            return ClearedAny(anonymousTypeInitializer.GetAttributeNamesArray());
        }

        #endregion Cleared (Null)

        #region Update (Value)

        /// <summary>
        /// Requires all columns to have been updated to the specified values, which are different than the pre-image values
        /// </summary>
        /// <param name="entity">The entity with Attributes to require to have been to</param>
        /// <returns></returns>
        public RequirementValidator Updated(Entity entity)
        {
            Get(ContextEntity.Target).UpdatedToValues.Add(entity);
            return this;
        }

        /// <summary>
        /// Requires at least one column has been updated to the specified value, which is different than the pre-image value
        /// </summary>
        /// <param name="entity">The entity with Attributes to require to have been updated to</param>
        /// <returns></returns>
        public RequirementValidator UpdatedAny(Entity entity)
        {
            Get(ContextEntity.Target).UpdatedToOrValues.Add(entity);
            return this;
        }

        #endregion Updated (Value)

        #endregion Updated (Non-Null) / Changed (Nullable) / Cleared (Null) / Updated (value)

        private Requirement Get(ContextEntity entityType)
        {
            if (Requirements.TryGetValue(entityType, out var requirement))
            {
                return requirement;
            }

            requirement = new Requirement(entityType);
            Requirements[entityType] = requirement;
            return requirement;
        }

        private class Requirement
        {
            private ContextEntity EntityType { get; }

            public HashSet<string> RequiredColumns { get; } = new HashSet<string>();
            public List<List<string>> RequiredOrColumns { get; } = new List<List<string>>();

            public HashSet<string> RequiredColumnsAllowNulls { get; } = new HashSet<string>();
            public List<List<string>> RequiredOrColumnsAllowNulls { get; } = new List<List<string>>();

            public HashSet<string> RequiredNullColumns { get; } = new HashSet<string>();
            public List<List<string>> RequiredNullOrColumns { get; } = new List<List<string>>();

            public HashSet<string> UpdatedColumns { get; } = new HashSet<string>();
            public List<List<string>> UpdatedOrColumns { get; } = new List<List<string>>();

            public HashSet<string> UpdatedColumnsAllowNulls { get; } = new HashSet<string>();
            public List<List<string>> UpdatedOrColumnsAllowNulls { get; } = new List<List<string>>();

            public HashSet<string> UpdatedNullColumns { get; } = new HashSet<string>();
            public List<List<string>> UpdatedNullOrColumns { get; } = new List<List<string>>();

            public EntityList ContainedValues { get; } = new EntityList();
            public EntityOrList ContainedOrValues { get; } = new EntityOrList();

            public EntityList UpdatedToValues { get; } = new EntityList();
            public EntityOrList UpdatedToOrValues { get; } = new EntityOrList();
            
            public InvalidRequirementReason Reason { get; private set; }

            private bool IsPreImageRequired { get; set; } = true;


            public Requirement(ContextEntity entityType)
            {
                EntityType = entityType;
            }

            public bool SkipExecution(IExtendedPluginContext context)
            {
                var entity = GetEntity(context);

                var preImage = context.GetMessageType() == MessageType.Update
                    ? context.GetPreEntity<Entity>()
                    : new Entity();
                var service = context.SystemOrganizationService;
                AssertHasPreImageIfRequired(service, preImage);

                return SkipExecution(context, entity, RequiredColumns, RequiredOrColumns, checkNotNull: true)
                       || SkipExecution(context, entity, RequiredColumnsAllowNulls, RequiredOrColumnsAllowNulls, checkNotNull: false)
                       || SkipNonNullExecution(context, entity, RequiredNullColumns, RequiredNullOrColumns)
                       || SkipValueExecution(context, entity, ContainedValues.GetValues(service), ContainedOrValues.GetValues())
                       || SkipExecutionForUpdate(context, entity, preImage, UpdatedColumns, UpdatedOrColumns, checkNotNull: true)
                       || SkipExecutionForUpdate(context, entity, preImage, UpdatedColumnsAllowNulls, UpdatedOrColumnsAllowNulls, checkNotNull: false)
                       || SkipNonNullExecutionForUpdate(context, entity, preImage, UpdatedNullColumns, UpdatedNullOrColumns)
                       || SkipValueExecutionForUpdate(context, entity, preImage, UpdatedToValues.GetValues(service), UpdatedToOrValues.GetValues());
            }

            private bool SkipExecution(IExtendedPluginContext context, Entity entity, HashSet<string> allColumns, List<List<string>> atLeastOneColumns, bool checkNotNull)
            {
                var type = checkNotNull
                    ? ColumnRequirementCheck.Contains
                    : ColumnRequirementCheck.ContainsNullable;
                var columnReason = checkNotNull
                    ? InvalidColumnReason.Null
                    : InvalidColumnReason.Missing;
                var treatNotContainsAsNull =  !checkNotNull && (EntityType == ContextEntity.PreImage || EntityType == ContextEntity.CoalesceTargetPreImage);
                foreach (var column in allColumns)
                {
                    if (!entity.Contains(column))
                    {
                        if (treatNotContainsAsNull)
                        {
                            continue;
                        }
                        Reason = new InvalidRequirementReason
                        {
                            Columns = new List<string> { column },
                            ColumnReason = InvalidColumnReason.Missing,
                            ContextEntity = EntityType,
                            IsAny = false,
                            RequirementType = type
                        };
                        context.Trace("The {0} entity type did not contain the required column {1}!", EntityType, column);
                        return true;
                    }

                    if (checkNotNull && entity[column] == null)
                    {
                        Reason = new InvalidRequirementReason
                        {
                            Columns = new List<string> { column },
                            ColumnReason = columnReason,
                            ContextEntity = EntityType,
                            IsAny = false,
                            RequirementType = type
                        };
                        context.Trace("The {0} entity type contained a null value for the required column {1}!", EntityType, column);
                        return true;
                    }
                }

                foreach (var set in atLeastOneColumns)
                {
                    var requirementMet = false;
                    foreach (var column in set)
                    {
                        if (entity.Contains(column) && (!checkNotNull || entity[column] != null)
                            || treatNotContainsAsNull && !entity.Contains(column))
                        {
                            requirementMet = true;
                            break;
                        }
                    }

                    if (!requirementMet)
                    {
                        Reason = new InvalidRequirementReason
                        {
                            Columns = set,
                            ColumnReason = columnReason,
                            ContextEntity = EntityType,
                            IsAny = true,
                            RequirementType = type
                        };
                        if (checkNotNull)
                        {
                            context.Trace("The {0} entity type did not contain a non-null value for at least one of the following columns: {1}!", EntityType, string.Join(", ", set));
                        }
                        else
                        {
                            context.Trace("The {0} entity type did not contain at least one of the following columns: {1}!", EntityType, string.Join(", ", set));
                        }
                        return true;
                    }
                }

                return false;
            }

            private bool SkipNonNullExecution(IExtendedPluginContext context, Entity entity, HashSet<string> requiredNulls, List<List<string>> atLeastOneNulls)
            {
                var treatNotContainsAsNull = EntityType == ContextEntity.PreImage || EntityType == ContextEntity.CoalesceTargetPreImage;
                foreach (var column in requiredNulls)
                {
                    if (!entity.Contains(column))
                    {
                        if (treatNotContainsAsNull)
                        {
                            continue;
                        }
                        Reason = new InvalidRequirementReason
                        {
                            Columns = new List<string> { column },
                            ColumnReason = InvalidColumnReason.Missing,
                            ContextEntity = EntityType,
                            IsAny = false,
                            RequirementType = ColumnRequirementCheck.ContainsNull
                        };
                        context.Trace("The {0} entity type did not contain the required to be null column {1}!", EntityType, column);
                        return true;
                    }

                    if (entity[column] != null)
                    {
                        Reason = new InvalidRequirementReason
                        {
                            Columns = new List<string> { column },
                            ColumnReason = InvalidColumnReason.NotNull,
                            ContextEntity = EntityType,
                            IsAny = false,
                            RequirementType = ColumnRequirementCheck.ContainsNull
                        };
                        context.Trace("The {0} entity type contained a non-null value for the required to be null column {1}!", EntityType, column);
                        return true;
                    }
                }

                foreach (var set in atLeastOneNulls)
                {
                    var requirementMet = false;
                    foreach (var column in set)
                    {
                        if (entity.Contains(column) && entity[column] == null
                            || treatNotContainsAsNull && !entity.Contains(column))
                        {
                            requirementMet = true;
                            break;
                        }
                    }

                    if (!requirementMet)
                    {
                        Reason = new InvalidRequirementReason
                        {
                            Columns = set,
                            ColumnReason = InvalidColumnReason.NotNull,
                            ContextEntity = EntityType,
                            IsAny = true,
                            RequirementType = ColumnRequirementCheck.ContainsNull
                        };
                        context.Trace("The {0} entity type did not contain a null value for at least one of the following columns: {1}!", EntityType, string.Join(", ", set));
                        return true;
                    }
                }

                return false;
            }

            private bool SkipValueExecution(IExtendedPluginContext context, Entity entity, Dictionary<string, object> requiredValues, List<Dictionary<string, object>> atLeastOneMatches)
            {
                var service = context.SystemOrganizationService;
                var treatNotContainsAsNull = EntityType == ContextEntity.PreImage || EntityType == ContextEntity.CoalesceTargetPreImage;
                foreach (var att in requiredValues)
                {
                    if (!entity.Contains(att.Key))
                    {
                        if (treatNotContainsAsNull && att.Value == null)
                        {
                            continue;
                        }
                        Reason = new InvalidRequirementReason
                        {
                            Columns = new List<string> { att.Key },
                            ColumnReason = InvalidColumnReason.Missing,
                            ContextEntity = EntityType,
                            IsAny = false,
                            RequirementType = ColumnRequirementCheck.ContainsValue
                        };
                        context.Trace("The {0} entity type did not contain a value for column {1} which was required to be {2}!", EntityType, att.Key, att.Value.ObjectToStringDebug());
                        return true;
                    }

                    if (!AttributeComparer.ValuesAreEqual(service, att.Value, entity[att.Key]))
                    {
                        Reason = new InvalidRequirementReason
                        {
                            Columns = new List<string> { att.Key },
                            ColumnReason = InvalidColumnReason.UnspecifiedValue,
                            ContextEntity = EntityType,
                            IsAny = false,
                            RequirementType = ColumnRequirementCheck.ContainsValue
                        };
                        context.Trace("The {0} entity type was required to contain a value of {1} for column {2} but contained the value {3}!", EntityType, att.Value, att.Key, entity[att.Key].ObjectToStringDebug());
                        return true;
                    }
                }


                var missingMatch = atLeastOneMatches.FirstOrDefault(set => !EntityContainsAtLeastOneMatch(service, entity, set, treatNotContainsAsNull));
                if (missingMatch != null)
                {
                    Reason = new InvalidRequirementReason
                    {
                        Columns = new List<string>(missingMatch.Keys),
                        ColumnReason = InvalidColumnReason.UnspecifiedValue,
                        ContextEntity = EntityType,
                        IsAny = true,
                        RequirementType = ColumnRequirementCheck.ContainsValue
                    };
                    context.Trace("The {0} entity type did not contain the required value for at least one of the following columns: {1}!", EntityType, string.Join(", ", missingMatch.Keys));
                    return true;
                }

                return false;
            }

            private static bool EntityContainsAtLeastOneMatch(IOrganizationService service, Entity entity, Dictionary<string, object> set, bool treatNotContainsAsNull)
            {
                return set.Any(att =>
                    entity.Contains(att.Key) && AttributeComparer.ValuesAreEqual(service, att.Value, entity[att.Key])
                    || treatNotContainsAsNull && !entity.Contains(att.Key) && att.Value == null);
            }

            private bool EntityContainsAtLeastOneMatchChanged(IOrganizationService service, Entity target, Entity preImage, Dictionary<string, object> set)
            {
                return set.Any(att =>
                    target.Contains(att.Key)
                    && AttributeComparer.ValuesAreEqual(service, att.Value, target[att.Key])
                    && ColumnValueHasChanged(service, target, preImage, att.Key));
            }

            private void AssertHasPreImageIfRequired(IOrganizationService service, Entity preImage)
            {
                if (preImage != null
                    || !IsPreImageRequired)
                {
                    return;
                }

                var requiredPreImageColumns = new HashSet<string>();
                // Updated
                requiredPreImageColumns.AddMissing(UpdatedColumns);
                requiredPreImageColumns.AddMissing(UpdatedColumnsAllowNulls);
                requiredPreImageColumns.AddMissing(UpdatedNullColumns);
                requiredPreImageColumns.AddMissing(UpdatedToValues.GetValues(service).Keys);

                // Updated Or
                requiredPreImageColumns.AddMissing(UpdatedOrColumns.SelectMany(c => c));
                requiredPreImageColumns.AddMissing(UpdatedOrColumnsAllowNulls.SelectMany(c => c));
                requiredPreImageColumns.AddMissing(UpdatedNullOrColumns.SelectMany(c => c));
                requiredPreImageColumns.AddMissing(UpdatedToOrValues.GetValues().SelectMany(c => c.Keys));
                if (EntityType == ContextEntity.PreImage || EntityType == ContextEntity.CoalesceTargetPreImage)
                {
                    // Required
                    requiredPreImageColumns.AddMissing(RequiredColumns);
                    requiredPreImageColumns.AddMissing(RequiredColumnsAllowNulls);
                    requiredPreImageColumns.AddMissing(RequiredNullColumns);
                    requiredPreImageColumns.AddMissing(ContainedValues.GetValues(service).Keys);

                    // Required Or
                    requiredPreImageColumns.AddMissing(RequiredOrColumns.SelectMany(c => c));
                    requiredPreImageColumns.AddMissing(RequiredOrColumnsAllowNulls.SelectMany(c => c));
                    requiredPreImageColumns.AddMissing(RequiredNullOrColumns.SelectMany(c => c));
                    requiredPreImageColumns.AddMissing(ContainedOrValues.GetValues().SelectMany(c => c.Keys));
                }

                if (requiredPreImageColumns.Any())
                {
                    throw new InvalidPluginExecutionException($"A pre-image was required but not found!  Expected a pre-image to be registered for this step with the following columns: {string.Join(", ", requiredPreImageColumns)}");
                }

                IsPreImageRequired = false;
            }

            private bool SkipExecutionForUpdate(IExtendedPluginContext context, Entity target, Entity preImage, HashSet<string> allColumns, List<List<string>> atLeastOneColumns, bool checkNotNull)
            {
                var service = context.SystemOrganizationService;
                var type = checkNotNull
                    ? ColumnRequirementCheck.Updated
                    : ColumnRequirementCheck.Changed;
                var columnReason = checkNotNull
                    ? InvalidColumnReason.Null
                    : InvalidColumnReason.Missing;
                foreach (var column in allColumns)
                {
                    if (!target.Contains(column))
                    {
                        Reason = new InvalidRequirementReason
                        {
                            Columns = new List<string> { column },
                            ColumnReason = InvalidColumnReason.Missing,
                            ContextEntity = EntityType,
                            IsAny = false,
                            RequirementType = type
                        };
                        context.Trace("The target did not contain a required update of column {1}!", EntityType, column);
                        return true;
                    }

                    if (checkNotNull && target[column] == null)
                    {
                        Reason = new InvalidRequirementReason
                        {
                            Columns = new List<string> { column },
                            ColumnReason = columnReason,
                            ContextEntity = EntityType,
                            IsAny = false,
                            RequirementType = type
                        };
                        context.Trace("The target did not contain a required update of column {1} to a non-null value!", EntityType, column);
                        return true;
                    }

                    if (!ColumnValueHasChanged(service, target, preImage, column))
                    {
                        context.Trace("The target did not update the column {1} to a non-null value!", EntityType, column);
                        return true;
                    }
                }

                foreach (var set in atLeastOneColumns)
                {
                    var requirementMet = false;
                    foreach (var column in set)
                    {
                        if (target.Contains(column) && (!checkNotNull || target[column] != null) && ColumnValueHasChanged(service, target, preImage, column))
                        {
                            requirementMet = true;
                            break;
                        }
                    }

                    if (!requirementMet)
                    {
                        Reason = new InvalidRequirementReason
                        {
                            Columns = set,
                            ColumnReason = columnReason,
                            ContextEntity = EntityType,
                            IsAny = true,
                            RequirementType = type
                        };
                        context.Trace(
                            checkNotNull
                                ? "The target did not update to a non-null value for at least one of the following columns: {0}!"
                                : "The target did not update at least one of the following columns: {0}!", string.Join(", ", set));
                        return true;
                    }
                }

                return false;
            }

            private bool SkipNonNullExecutionForUpdate(IExtendedPluginContext context, Entity target, Entity preImage, HashSet<string> allNullColumns, List<List<string>> atLeastOneNullColumns)
            {
                var service = context.SystemOrganizationService;
                foreach (var column in allNullColumns)
                {
                    if (!target.Contains(column))
                    {
                        Reason = new InvalidRequirementReason
                        {
                            Columns = new List<string> { column },
                            ColumnReason = InvalidColumnReason.Missing,
                            ContextEntity = EntityType,
                            IsAny = false,
                            RequirementType = ColumnRequirementCheck.Cleared
                        };
                        context.Trace("The target did not contain a required update of column {0} to null!", column);
                        return true;
                    }

                    if (target[column] != null)
                    {
                        Reason = new InvalidRequirementReason
                        {
                            Columns = new List<string> { column },
                            ColumnReason = InvalidColumnReason.Null,
                            ContextEntity = EntityType,
                            IsAny = false,
                            RequirementType = ColumnRequirementCheck.Cleared
                        };
                        context.Trace("The target contained a non-null value for column {0} that was required to be updated to null!", column);
                        return true;
                    }

                    if (!ColumnValueHasChanged(service, target, preImage, column))
                    {
                        Reason = new InvalidRequirementReason
                        {
                            Columns = new List<string> { column },
                            ColumnReason = InvalidColumnReason.UnchangedValue,
                            ContextEntity = EntityType,
                            IsAny = false,
                            RequirementType = ColumnRequirementCheck.Cleared
                        };
                        context.Trace("The target contained a null value for column {0} that was required to be updated to null, but it was already null!", column);
                        return true;
                    }
                }

                foreach (var set in atLeastOneNullColumns)
                {
                    var requirementMet = false;
                    foreach (var column in set)
                    {
                        if (target.Contains(column) && target[column] == null && ColumnValueHasChanged(service, target, preImage, column))
                        {
                            requirementMet = true;
                            break;
                        }
                    }
                
                    if (!requirementMet)
                    {
                        Reason = new InvalidRequirementReason
                        {
                            Columns = set,
                            ColumnReason = InvalidColumnReason.UnchangedValue,
                            ContextEntity = EntityType,
                            IsAny = true,
                            RequirementType = ColumnRequirementCheck.Cleared
                        };
                        context.Trace("The target did not update at least one of the following columns to null: {0}!", string.Join(", ", set));
                        return true;
                    }
                }

                return false;
            }
            private bool SkipValueExecutionForUpdate(IExtendedPluginContext context, Entity target, Entity preImage, Dictionary<string, object> allValueColumns, List<Dictionary<string, object>> atLeastOneValueColumns)
            {
                var service = context.SystemOrganizationService;
                foreach (var att in allValueColumns)
                {
                    if (!target.Contains(att.Key))
                    {
                        Reason = new InvalidRequirementReason
                        {
                            Columns = new List<string> { att.Key },
                            ColumnReason = InvalidColumnReason.Missing,
                            ContextEntity = EntityType,
                            IsAny = false,
                            RequirementType = ColumnRequirementCheck.UpdatedValue
                        };
                        context.Trace("The target did not contain a required update of column {0} to {1}!", att.Key, att.Value.ObjectToStringDebug());
                        return true;
                    }

                    if (!AttributeComparer.ValuesAreEqual(service, att.Value, target[att.Key])) 
                    {
                        Reason = new InvalidRequirementReason
                        {
                            Columns = new List<string> { att.Key },
                            ColumnReason = InvalidColumnReason.UnspecifiedValue,
                            ContextEntity = EntityType,
                            IsAny = false,
                            RequirementType = ColumnRequirementCheck.UpdatedValue
                        };
                        context.Trace("The target contained value {0} for column {1} that was required to be updated to {2}!", target[att.Key].ObjectToStringDebug(), att.Key, att.Value.ObjectToStringDebug());
                        return true;
                    }

                    if (!ColumnValueHasChanged(service, target, preImage, att.Key))
                    {
                        Reason = new InvalidRequirementReason
                        {
                            Columns = new List<string> { att.Key },
                            ColumnReason = InvalidColumnReason.UnchangedValue,
                            ContextEntity = EntityType,
                            IsAny = false,
                            RequirementType = ColumnRequirementCheck.UpdatedValue
                        };
                        context.Trace("The target contained value {0} for column {1} which it was be updated to, but it already had that value!", att.Value.ObjectToStringDebug(), att.Key);
                        return true;
                    }
                }

                var missingMatch = atLeastOneValueColumns.FirstOrDefault(set => EntityContainsAtLeastOneMatchChanged(service, target, preImage, set));
                if (missingMatch != null)
                {
                    Reason = new InvalidRequirementReason
                    {
                        Columns = new List<string>(missingMatch.Keys),
                        ColumnReason = InvalidColumnReason.UnchangedValue,
                        ContextEntity = EntityType,
                        IsAny = true,
                        RequirementType = ColumnRequirementCheck.UpdatedValue
                    };
                    context.Trace("The target did not update at least one of the following columns: {0} to the required value!", missingMatch.Keys.ToCsv());
                    return true;
                }

                return false;
            }

            private bool ColumnValueHasChanged(IOrganizationService service, Entity target, Entity preImage, string column)
            {
                if (!target.Contains(column))
                {
                    return false;
                }

                return !AttributeComparer.ValuesAreEqual(service, target[column], preImage.Contains(column) ? preImage[column] : null);
            }

            private Entity GetEntity(IExtendedPluginContext context)
            {
                Entity entity;
                switch (EntityType)
                {
                    case ContextEntity.CoalesceTargetPostImage:
                        entity = context.CoalesceTargetWithPostEntity<Entity>();
                        break;
                    case ContextEntity.CoalesceTargetPreImage:
                        entity = context.CoalesceTargetWithPreEntity<Entity>();
                        break;
                    case ContextEntity.PostImage:
                        return context.GetPostEntity<Entity>();
                    case ContextEntity.PreImage:
                        return context.GetPreEntity<Entity>();
                    case ContextEntity.Target:
                        entity = context.GetTarget<Entity>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (entity == null )
                {
                    throw new Exception($"A Requirement has been defined for entity of type {EntityType} but the entity type was not found in the context.");
                }

                return entity;
            }
        }

        private class EntityList: EntityListBase
        {
            private Dictionary<string, object> Values { get; set; }

            public Dictionary<string, object> GetValues(IOrganizationService service)
            {
                if (IsDirty)
                {
                    Values = new Dictionary<string, object>();
                    foreach (var entity in Entities)
                    {
                        foreach (var att in entity.Attributes)
                        {
                            if (Values.TryGetValue(att.Key, out var value))
                            {
                                if (!AttributeComparer.ValuesAreEqual(service, value, att.Value))
                                {
                                    throw new Exception($"RequirementValidator Configuration Error!  Multiple values have been defined for column {att.Key}!");
                                }
                            }
                            else
                            {
                                Values.Add(att.Key, att.Value);
                            }
                        }
                    }
                }

                IsDirty = false;
                return Values;
            }
        }

        private class EntityOrList : EntityListBase
        {
            private List<Dictionary<string, object>> Values { get; set; }

            public List<Dictionary<string, object>> GetValues()
            {
                if (IsDirty)
                {
                    Values = new List<Dictionary<string, object>>();
                    foreach (var entity in Entities)
                    {
                        Values.Add(entity.Attributes.ToDictionary(att => att.Key, att => att.Value));
                    }
                }

                IsDirty = false;
                return Values;
            }
        }

        private abstract class EntityListBase
        {
            protected List<Entity> Entities { get; } = new List<Entity>();

            protected bool IsDirty { get; set; } = true;

            public void Add(Entity entity)
            {
                IsDirty = true;
                Entities.Add(entity);
            }
        }
    }
}