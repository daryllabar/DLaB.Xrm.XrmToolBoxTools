using System;
using System.Collections;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
	
{
    /// <summary>
    /// A QueryExpression Typed to the Entity that is being received 
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class TypedQueryExpression<TEntity> where TEntity : Entity
    {
        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>
        /// The query.
        /// </value>
        public QueryExpression Query { get; set; }

        #region Pass Through QueryExpression Calls

        /// <summary>
        /// The QueryExpression's Distinct
        /// </summary>
        /// <value>
        /// <c>true</c> if distinct; otherwise, <c>false</c>.
        /// </value>
        public bool Distinct => Query.Distinct;
        /// <summary>
        /// The QueryExpression's NoLock
        /// </summary>
        /// <value>
        /// <c>true</c> if [no lock]; otherwise, <c>false</c>.
        /// </value>
        public bool NoLock => Query.NoLock;
        /// <summary>
        /// The QueryExpression's Page Info
        /// </summary>
        /// <value>
        /// The page information.
        /// </value>
        public PagingInfo PageInfo => Query.PageInfo;
        /// <summary>
        /// The QueryExpression's LinkEntities
        /// </summary>
        /// <value>
        /// The link entities.
        /// </value>
        public DataCollection<LinkEntity> LinkEntities => Query.LinkEntities;
        /// <summary>
        /// The QueryExpression's Criteria
        /// </summary>
        /// <value>
        /// The criteria.
        /// </value>
        public FilterExpression Criteria => Query.Criteria;
        /// <summary>
        /// The QueryExpression's Orders
        /// </summary>
        /// <value>
        /// The orders.
        /// </value>
        public DataCollection<OrderExpression> Orders => Query.Orders;
        /// <summary>
        /// The QueryExpression's EntityName
        /// </summary>
        /// <value>
        /// The name of the entity.
        /// </value>
        public string EntityName => Query.EntityName;
        /// <summary>
        /// The QueryExpression's ColumnSet
        /// </summary>
        /// <value>
        /// The column set.
        /// </value>
        public ColumnSet ColumnSet => Query.ColumnSet;
        /// <summary>
        /// The QueryExpression's TopCount
        /// </summary>
        /// <value>
        /// The top count.
        /// </value>
        public int? TopCount => Query.TopCount;

        /// <summary>
        /// Adds the order.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="orderType">Type of the order.</param>
        public void AddOrder(string attributeName, OrderType orderType)
        {
            Query.AddOrder(attributeName, orderType);
        }

        /// <summary>
        /// Adds the link.
        /// </summary>
        /// <param name="linkToEntityName">Name of the link to entity.</param>
        /// <param name="linkFromAttributeName">Name of the link from attribute.</param>
        /// <param name="linkToAttributeName">Name of the link to attribute.</param>
        /// <returns></returns>
        public LinkEntity AddLink(string linkToEntityName, string linkFromAttributeName, string linkToAttributeName)
        {
            return Query.AddLink(linkToEntityName, linkFromAttributeName, linkToAttributeName);
        }

        /// <summary>
        /// Adds the link.
        /// </summary>
        /// <param name="linkToEntityName">Name of the link to entity.</param>
        /// <param name="linkFromAttributeName">Name of the link from attribute.</param>
        /// <param name="linkToAttributeName">Name of the link to attribute.</param>
        /// <param name="joinOperator">The join operator.</param>
        /// <returns></returns>
        public LinkEntity AddLink(string linkToEntityName, string linkFromAttributeName, string linkToAttributeName, JoinOperator joinOperator)
        {
            return Query.AddLink(linkToEntityName, linkFromAttributeName, linkToAttributeName, joinOperator);
        }

        #endregion Pass Through QueryExpression Calls

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedQueryExpression{TEntity}" /> class.
        /// </summary>
        /// <param name="qe">The qe.</param>
        public TypedQueryExpression(QueryExpression qe)
        {
            Query = qe;
        }

        #endregion Constructors

        #region Type Conversions

        /// <summary>
        /// Implements the operator implicit QueryExpression.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static implicit operator QueryExpression(TypedQueryExpression<TEntity> container)
        {
            return container?.Query;
        }

        #endregion Type Conversions

        #region AddLink

        /// <summary>
        /// Adds the link.
        /// </summary>
        /// <param name="linkToEntityName">Name of the link to entity.</param>
        /// <param name="linkAttributesName">Name of the link attributes.</param>
        /// <returns></returns>
        public LinkEntity AddLink(string linkToEntityName, string linkAttributesName)
        {
            return Query.AddLink(linkToEntityName, linkAttributesName, linkAttributesName);
        }

        /// <summary>
        /// Adds the link.
        /// </summary>
        /// <param name="linkToEntityName">Name of the link to entity.</param>
        /// <param name="linkAttributesName">Name of the link attributes.</param>
        /// <param name="joinType">Type of the join.</param>
        /// <returns></returns>
        public LinkEntity AddLink(string linkToEntityName, string linkAttributesName, JoinOperator joinType)
        {
            return Query.AddLink(linkToEntityName, linkAttributesName, linkAttributesName, joinType);
        }

        #endregion AddLink

        #region AddLink<T>

        #region Same Link Attribute Name
        /// <summary>
        /// Adds the type T as a linked entity to the query expression, additionally ensuring it is active.
        /// Assumes both entities have the same attribute name to link on.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="linkAttributesName"></param>
        /// <returns></returns>
        public LinkEntity AddLink<T>(string linkAttributesName) where T : Entity
        {
            return Query.AddLink<T>(linkAttributesName, linkAttributesName);
        }

        /// <summary>
        /// Adds the type T as a linked entity to the query expression, additionally ensuring it is active.
        /// Assumes both entities have the same attribute name to link on.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="linkAttributesName"></param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <returns></returns>
        public LinkEntity AddLink<T>(string linkAttributesName, Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
        {
            return Query.AddLink(linkAttributesName, linkAttributesName, anonymousTypeInitializer);
        }

        /// <summary>
        /// Adds the type T as a linked entity, additionally ensuring it is active.
        /// Assumes both entities have the same attribute name to link on.
        /// </summary>
        /// <typeparam name="T">The type of the entity that is to be linked and asserted active.</typeparam>
        /// <param name="linkAttributesName">Name of the link attributes.</param>
        /// <param name="joinType">Type of the join to perform.</param>
        /// <returns></returns>
        public LinkEntity AddLink<T>(string linkAttributesName, JoinOperator joinType) where T : Entity
        {
            return Query.AddLink<T>(linkAttributesName, linkAttributesName, joinType);
        }

        /// <summary>
        /// Adds the type T as a linked entity, additionally ensuring it is active.
        /// Assumes both entities have the same attribute name to link on.
        /// </summary>
        /// <typeparam name="T">The type of the entity that is to be linked and asserted active.</typeparam>
        /// <param name="linkAttributesName">Name of the link attributes.</param>
        /// <param name="joinType">Type of the join to perform.</param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <returns></returns>
        public LinkEntity AddLink<T>(string linkAttributesName, JoinOperator joinType, Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
        {
            return Query.AddLink(linkAttributesName, linkAttributesName, joinType, anonymousTypeInitializer);
        }
        #endregion Same Link Attribute Name

        #region Different Link Attribute Names
        /// <summary>
        /// Adds the type T as a linked entity to the query expression, additionally ensuring it is active.
        /// </summary>
        /// <typeparam name="T">The type of the entity that is to be linked and asserted active.</typeparam>
        /// <param name="linkFromAttributeName">Name of the link from attribute.</param>
        /// <param name="linkToAttributeName">Name of the link to attribute.</param>
        /// <returns></returns>
        public LinkEntity AddLink<T>(string linkFromAttributeName, string linkToAttributeName) where T : Entity
        {
            return Query.AddLink<T>(linkFromAttributeName, linkToAttributeName);
        }

        /// <summary>
        /// Adds the type T as a linked entity to the query expression, additionally ensuring it is active.
        /// </summary>
        /// <typeparam name="T">The type of the entity that is to be linked and asserted active.</typeparam>
        /// <param name="linkFromAttributeName">Name of the link from attribute.</param>
        /// <param name="linkToAttributeName">Name of the link to attribute.</param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <returns></returns>
        public LinkEntity AddLink<T>(string linkFromAttributeName, string linkToAttributeName, Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
        {
            return Query.AddLink(linkFromAttributeName, linkToAttributeName, anonymousTypeInitializer);
        }

        /// <summary>
        /// Adds the type T as a linked entity, additionally ensuring it is active.
        /// </summary>
        /// <typeparam name="T">The type of the entity that is to be linked and asserted active.</typeparam>
        /// <param name="linkFromAttributeName">Name of the link from attribute.</param>
        /// <param name="linkToAttributeName">Name of the link to attribute.</param>
        /// <param name="joinType">Type of the join to perform.</param>
        /// <returns></returns>
        public LinkEntity AddLink<T>(string linkFromAttributeName, string linkToAttributeName, JoinOperator joinType) where T : Entity
        {
            return Query.AddLink<T>(linkFromAttributeName, linkToAttributeName, joinType);
        }

        /// <summary>
        /// Adds the type T as a linked entity, additionally ensuring it is active.
        /// </summary>
        /// <typeparam name="T">The type of the entity that is to be linked and asserted active.</typeparam>
        /// <param name="linkFromAttributeName">Name of the link from attribute.</param>
        /// <param name="linkToAttributeName">Name of the link to attribute.</param>
        /// <param name="joinType">Type of the join to perform.</param>
        /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
        /// type are the column names to add</param>
        /// <returns></returns>
        public LinkEntity AddLink<T>(string linkFromAttributeName, string linkToAttributeName, JoinOperator joinType, Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
        {
            return Query.AddLink(linkFromAttributeName, linkToAttributeName, joinType, anonymousTypeInitializer);
        }
        #endregion Different Link Attribute Names

        #endregion AddLink<T>

        #region QueryExpression Extensions Methods

        /// <summary>
        /// Depending on the Type of T, adds the correct is active criteria Statement
        /// Note: Does not work for Linked Entities
        /// </summary>
        public TypedQueryExpression<TEntity> ActiveOnly()
        {
            Query.Criteria.ActiveOnly<TEntity>();
            return this;
        }

        /// <summary>
        /// Sets the Count and Page number of the query to return just the first entity.
        /// </summary>
        /// <returns></returns>
        public TypedQueryExpression<TEntity> First()
        {
            Query.First();
            return this;
        }

        /// <summary>
        /// Returns a SQL-ish representation of the QueryExpression's Criteria
        /// </summary>
        /// <returns></returns>
        public string GetSqlStatement()
        {
            return Query.GetSqlStatement();
        }

        /// <summary>
        /// Adds a Condition expression to the filter expression to force the statecode to be a specfic value.
        /// </summary>
        /// <param name="entityStateEnum">The entity state enum.</param>
        /// <returns></returns>
        public TypedQueryExpression<TEntity> StateIs(object entityStateEnum)
        {
            Query.Criteria.StateIs(entityStateEnum);
            return this;
        }

        /// <summary>
        /// Updates the Query Expression to only return only the first entity that matches the query expression expression criteria.
        /// Shortcut for setting the Query's PageInfo.Count and PageInfo.PageNumber to 1.
        /// </summary>
        /// <param name="count">The count of entities to restrict the result of the query to.</param>
        public TypedQueryExpression<TEntity> Take(int count)
        {
            Query.Take(count);
            return this;
        }

        #region Where

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="values">list of values to be compared with</param>
        public TypedQueryExpression<TEntity> Where(string attributeName, ConditionOperator conditionOperator, params object[] values)
        {
            Query.WhereEqual(new ConditionExpression(attributeName, conditionOperator, values));
            return this;
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="entityName">Name of the entity of attribute on which condition is to be defined on</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="values">list of values to be compared with</param>
        public TypedQueryExpression<TEntity> Where(string entityName, string attributeName, ConditionOperator conditionOperator, params object[] values)
        {
            Query.WhereEqual(new ConditionExpression(entityName, attributeName, conditionOperator, values));
            return this;
        }

#if !XRM_2013 && !XRM_2015 && !XRM_2016
        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="entityName">Name of the entity of attribute on which condition is to be defined on</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="compareColumns">Boolean flag to define condition on attributes instead of condition on constant value(s)</param>
        /// <param name="values">list of values or attributes(if compareColumns is true) to be compared with</param>
        public TypedQueryExpression<TEntity> Where(string entityName, string attributeName, ConditionOperator conditionOperator, bool compareColumns, object[] values)
        {
            Query.WhereEqual(new ConditionExpression(entityName, attributeName, conditionOperator, compareColumns, values));
            return this;
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="entityName">Name of the entity of attribute on which condition is to be defined on</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="compareColumns">Boolean flag to define condition on attributes instead of condition on constant value(s)</param>
        /// <param name="value">value or attributes(if compareColumns is true) to be compared with</param>
        public TypedQueryExpression<TEntity> Where(string entityName, string attributeName, ConditionOperator conditionOperator, bool compareColumns, object value)
        {
            Query.WhereEqual(new ConditionExpression(entityName, attributeName, conditionOperator, compareColumns, value));
            return this;
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="compareColumns">Boolean flag to define condition on attributes instead of condition on constant value(s)</param>
        /// <param name="value">value or attributes(if compareColumns is true) to be compared with</param>
        public TypedQueryExpression<TEntity> Where(string attributeName, ConditionOperator conditionOperator, bool compareColumns, object value)
        {
            Query.WhereEqual(new ConditionExpression(attributeName, conditionOperator, compareColumns, value));
            return this;
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="compareColumns">Boolean flag to define condition on attributes instead of condition on constant value(s)</param>
        /// <param name="values">list of values or attributes(if compareColumns is true) to be compared with</param>
        public TypedQueryExpression<TEntity> Where(string attributeName, ConditionOperator conditionOperator, bool compareColumns, object[] values)
        {
            Query.WhereEqual(new ConditionExpression(attributeName, conditionOperator, compareColumns, values));
            return this;
        }
#endif

        /// <summary>
        /// Adds the column name, condition operator and value, as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="value">value to be compared with</param>
        public TypedQueryExpression<TEntity> Where(string attributeName, ConditionOperator conditionOperator, object value)
        {
            Query.WhereEqual(new ConditionExpression(attributeName, conditionOperator, value));
            return this;
        }

        /// <summary>
        /// Adds the column name, condition operator and value, as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="entityName">Name of the entity of attribute on which condition is to be defined on</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="value">value to be compared with</param>)
        public TypedQueryExpression<TEntity> Where(string entityName,
            string attributeName,
            ConditionOperator conditionOperator,
            object value)
        {
            Query.WhereEqual(new ConditionExpression(entityName, attributeName, conditionOperator, value));
            return this;
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        public TypedQueryExpression<TEntity> Where(string attributeName, ConditionOperator conditionOperator)
        { 
            Query.WhereEqual(new ConditionExpression(attributeName, conditionOperator));
            return this;
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="entityName">Name of the entity of attribute on which condition is to be defined on</param>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        public TypedQueryExpression<TEntity> Where(string entityName,
            string attributeName,
            ConditionOperator conditionOperator)
        {
            Query.WhereEqual(new ConditionExpression(entityName, attributeName, conditionOperator));
            return this;
        }

        /// <summary>
        /// Adds the column name and condition operator as a Condition Expression to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="attributeName">Name of the attribute on which condition is to be defined on</param>
        /// <param name="conditionOperator">operator that has to be applied</param>
        /// <param name="values">list of values to be compared with</param>
        /// <remarks>Need to handle collections differently. esp. Guid arrays.</remarks>
        public TypedQueryExpression<TEntity> Where(string attributeName,
            ConditionOperator conditionOperator,
            ICollection values)
        {
            Query.WhereEqual(new ConditionExpression(attributeName, conditionOperator, values));
            return this;
        }

        #endregion Where


        /// <summary>
        /// Adds the column name and value pairs to the criteria of the given QueryExpression
        /// </summary>
        /// <param name="columnNameAndValuePairs">List of pairs that look like this:
        /// (string name of the column, value of the column) ie. "name", "John Doe" </param>
        public TypedQueryExpression<TEntity> WhereEqual(params object[] columnNameAndValuePairs)
        {
            Query.WhereEqual(columnNameAndValuePairs);
            return this;
        }

        /// <summary>
        /// Adds an In Condition to the QueryExpression Criteria
        /// </summary>
        /// <param name="columnName">The name of the column to perform the in against.</param>
        /// <param name="values">The list of values to search for being in the column name.</param>
        public TypedQueryExpression<TEntity> WhereIn(string columnName, params object[] values)
        {
            Query.WhereIn(columnName, values);
            return this;
        }


        #endregion QueryExpression Extension Methods
    }
}
