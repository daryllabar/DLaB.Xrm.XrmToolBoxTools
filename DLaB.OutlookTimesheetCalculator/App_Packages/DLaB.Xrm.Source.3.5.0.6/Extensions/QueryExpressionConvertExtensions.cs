using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
        #region ConditionExpression

        /// <summary>
        /// Gets the SQL like statement of the ConditionExpression
        /// </summary>
        /// <param name="ce">The ConditionExpression</param>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public static string GetStatement(this ConditionExpression ce, string entityName)
        {
            string op;
            string condition;
            var entityAttribute = (ce.EntityName ?? entityName) + "." + ce.AttributeName;
            switch (ce.Operator)
            {
                case ConditionOperator.Null:
                case ConditionOperator.NotNull:
                    op = ce.Operator == ConditionOperator.Null ? "IS NULL" : "IS NOT NULL";
                    condition = $"{entityAttribute} {op} ";
                    break;
                case ConditionOperator.In:
                case ConditionOperator.NotIn:
                    op = ce.Operator == ConditionOperator.In ? "IN" : "NOT IN";
                    condition = $"{entityAttribute} {op} ({ce.Values.Select(GetDisplayValue).ToCsv()}) ";
                    break;
                case ConditionOperator.Between:
                    if (ce.Values.Count == 2)
                    {
                        condition = $"{ce.AttributeName} {ce.Operator} {GetDisplayValue(GetValue(ce, 0))} AND {GetDisplayValue(GetValue(ce, 1))}";
                    }
                    else
                    {
                        throw new Exception("Between Operator should have two values to be between");
                    }
                    break;
                case ConditionOperator.Equal:
                    condition = $"{entityAttribute} = {GetDisplayValue(GetValue(ce, 0))} ";
                    break;
                case ConditionOperator.GreaterThan:
                    condition = $"{entityAttribute} > {GetDisplayValue(GetValue(ce, 0))} ";
                    break;
                case ConditionOperator.GreaterEqual:
                    condition = $"{entityAttribute} >= {GetDisplayValue(GetValue(ce, 0))} ";
                    break;
                case ConditionOperator.LessThan:
                    condition = $"{entityAttribute} < {GetDisplayValue(GetValue(ce, 0))} ";
                    break;
                case ConditionOperator.LessEqual:
                    condition = $"{entityAttribute} <= {GetDisplayValue(GetValue(ce, 0))} ";
                    break;
                case ConditionOperator.EqualUserId:
                    condition = "Is Current User";
                    break;
                case ConditionOperator.EqualBusinessId:
                    condition = "Is Current Business Unit";
                    break;
                case ConditionOperator.NotEqualUserId:
                    condition = "Is Not Current User";
                    break;
                case ConditionOperator.NotEqualBusinessId:
                    condition = "Is Not Current Business Unit";
                    break;
                case ConditionOperator.Last7Days:
                case ConditionOperator.LastFiscalPeriod:
                case ConditionOperator.LastFiscalYear:
                case ConditionOperator.LastMonth:
                case ConditionOperator.LastWeek:
                case ConditionOperator.LastYear:
                case ConditionOperator.Next7Days:
                case ConditionOperator.NextFiscalPeriod:
                case ConditionOperator.NextFiscalYear:
                case ConditionOperator.NextMonth:
                case ConditionOperator.NextWeek:
                case ConditionOperator.NextYear:
                case ConditionOperator.ThisFiscalPeriod:
                case ConditionOperator.ThisFiscalYear:
                case ConditionOperator.ThisMonth:
                case ConditionOperator.ThisWeek:
                case ConditionOperator.ThisYear:
                case ConditionOperator.Today:
                case ConditionOperator.Tomorrow:
                case ConditionOperator.Yesterday:
                    condition = $"{entityAttribute} IS IN {ce.Operator} ";
                    break;
                default:
                    condition = $"{ce.AttributeName} {ce.Operator} {GetDisplayValue(GetValue(ce, 0))} ";
                    break;
            }
            return condition;
        }

        private static object GetValue(ConditionExpression ce, int index)
        {
            if (ce.Values.Count <= index)
            {
                return "Invalid Index!  Requested " + index + " but count was " + ce.Values.Count;
            }
            return ce.Values[index];
        }

        private static string GetDisplayValue(object value)
        {
            if (value == null) { return "<null>"; }
            var type = value.GetType();

            if (typeof(String).IsAssignableFrom(type))
            {
                if (DateTime.TryParse(value as String, out DateTime localTime))
                {
                    value = localTime.ToUniversalTime().ToString(CultureInfo.InvariantCulture);
                }
                return "'" + value + "'";
            }

            if (typeof(Guid).IsAssignableFrom(type))
            {
                return "'" + value + "'";
            }

            if (typeof(DateTime).IsAssignableFrom(type))
            {
                var dateTimeValue = (DateTime)value;
                if (dateTimeValue.Kind != DateTimeKind.Utc)
                {
                    dateTimeValue = dateTimeValue.ToUniversalTime();
                }
                return "'" + dateTimeValue + "'";
            }
            
            return value.ToString();
        }

        #endregion ConditionExpression

        #region FilterExpression

        /// <summary>
        /// Returns a SQL-ish Representation of the filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public static string GetStatement(this FilterExpression filter, string entityName)
        {
            var conditions = filter.Conditions.Select(condition => GetStatement(condition, entityName)).ToList();

            if (filter.Conditions.Count > 0 && filter.Filters.Count > 0)
            {
                conditions[conditions.Count - 1] += Environment.NewLine;
            }

            conditions.AddRange(filter.Filters.Select(child => child.GetStatement(entityName) + Environment.NewLine));

            var join = filter.FilterOperator.ToString().PadLeft(3, ' ') + " ";
            var statement = String.Join(join, conditions.ToArray());
            if (String.IsNullOrWhiteSpace(statement)) { return string.Empty; }

            return "( " + statement + ") ";
        }

        #endregion FilterExpression

        #region QueryExpression

        /// <summary>
        /// Returns a SQL-ish representation of the QueryExpression's Criteria
        /// </summary>
        /// <param name="qe"></param>
        /// <returns></returns>
        public static string GetSqlStatement(this QueryExpression qe)
        {
            var sb = new StringBuilder();

            string top = string.Empty;
            if (qe.PageInfo != null && qe.PageInfo.Count > 0)
            {
                top = "TOP " + qe.PageInfo.Count + " ";
            }

            var allLinkedEntities = WalkAllLinkedEntities(qe).ToList();
            var cols = new List<string>();
            foreach (var link in allLinkedEntities.Where(l => l.Columns.Columns.Any()))
            {
                string linkName = link.EntityAlias ?? link.LinkToEntityName ?? "unspecified";
                linkName += ".";
                cols.AddRange(link.Columns.Columns.Select(c => linkName + c));
            }

            // Select Statement
            sb.Append("SELECT " + top);
            if (qe.ColumnSet != null)
            {
                string tableName = qe.EntityName + ".";
                if (qe.ColumnSet.AllColumns)
                {
                    sb.Append(cols.Count > 0 ? tableName + "*, " : tableName + "* ");
                }
                else
                {
                    cols.AddRange(qe.ColumnSet.Columns.Select(c => tableName + c));
                }
            }
            sb.AppendLine(cols.ToCsv());


            // From Statement
            sb.AppendLine("FROM " + qe.EntityName);
            foreach (var link in allLinkedEntities)
            {
                var aliasName = String.IsNullOrWhiteSpace(link.EntityAlias) ? link.LinkToEntityName : link.EntityAlias;
                sb.AppendFormat("{0} JOIN {1} on {2}.{3} = {4}.{5}{6}",
                        link.JoinOperator.ToString().ToUpper(),
                        link.LinkToEntityName + (String.IsNullOrWhiteSpace(link.EntityAlias) ? String.Empty : link.LinkToEntityName + " as " + link.EntityAlias),
                        link.LinkFromEntityName, link.LinkFromAttributeName,
                        aliasName, link.LinkToAttributeName, Environment.NewLine);
                if (link.LinkCriteria != null)
                {
                    var statement = link.LinkCriteria.GetStatement(aliasName);
                    if (!String.IsNullOrWhiteSpace(statement))
                    {

                        sb.AppendLine("    AND " + statement);
                    }
                }
            }

            sb.AppendLine("WHERE");
            sb.AppendLine(qe.Criteria.GetStatement(qe.EntityName));

            // Order By Statement
            if (qe.Orders.Any())
            {
                sb.AppendLine("ORDER BY");
                var orders = qe.Orders.Select(order => order.AttributeName + " " + order.OrderType);
                sb.Append(String.Join("," + Environment.NewLine, orders));
            }

            return sb.ToString();
        }

        #endregion QueryExpression

        #region Helper Functions

        /// <summary>
        /// Enumerates over all LinkEntities in the QueryExpression, in a depth-first search
        /// </summary>
        /// <param name="qe"></param>
        /// <returns></returns>
        private static IEnumerable<LinkEntity> WalkAllLinkedEntities(QueryExpression qe)
        {
            foreach (var link in qe.LinkEntities)
            {
                yield return link;
                foreach (var child in WalkAllLinkedEntities(link))
                {
                    yield return child;
                }
            }
        }

        /// <summary>
        /// Enumerates over all LinkEntities in the LinkEntity, in a depth-first search
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        private static IEnumerable<LinkEntity> WalkAllLinkedEntities(LinkEntity link)
        {
            foreach (var child in link.LinkEntities)
            {
                yield return child;
                foreach (var grandchild in WalkAllLinkedEntities(child))
                {
                    yield return grandchild;
                }
            }
        }

        #endregion Helper Functions
    }
}
