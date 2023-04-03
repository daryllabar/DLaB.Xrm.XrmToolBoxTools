using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq.Expressions;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
	
{
    public static partial class Extensions
    {
        // ActiveOnly is a method and _activeOnly Can't be called ActiveOnly 
        // ReSharper disable once InconsistentNaming
        private static readonly bool _activeOnly = new LateBoundQuerySettings(string.Empty).ActiveOnly;

        #region LinkEntity

        #region AddChildLink:  All methods created due to possible bug - http://stackoverflow.com/questions/10722307/why-does-linkentity-addlink-initialize-the-linkfromentityname-with-its-own-lin
            /// <summary>
            /// Adds the new LinkEntity as a child to this LinkEntity, rather than this LinkEntity's LinkFrom Entity
            /// Assumes that the linkFromAttributeName and the linkToAttributeName are the same
            /// </summary>
            /// <param name="link"></param>
            /// <param name="linkToEntityName"></param>
            /// <param name="linkAttributesName"></param>
            /// <returns></returns>
            public static LinkEntity AddChildLink(this LinkEntity link, string linkToEntityName, string linkAttributesName)
            {
                return link.AddChildLink(linkToEntityName, linkAttributesName, linkAttributesName);
            }

            /// <summary>
            /// Adds the new LinkEntity as a child to this LinkEntity, rather than this LinkEntity's LinkFrom Entity
            /// Assumes that the linkFromAttributeName and the linkToAttributeName are the same
            /// </summary>
            /// <param name="link"></param>
            /// <param name="linkToEntityName"></param>
            /// <param name="linkAttributesName"></param>
            /// <param name="joinType"></param>
            /// <returns></returns>
            public static LinkEntity AddChildLink(this LinkEntity link, string linkToEntityName, string linkAttributesName, JoinOperator joinType)
            {
                return link.AddChildLink(linkToEntityName, linkAttributesName, linkAttributesName, joinType);
            }

            /// <summary>
            /// Adds the new LinkEntity as a child to this LinkEntity, rather than this LinkEntity's LinkFrom Entity
            /// </summary>
            /// <param name="link"></param>
            /// <param name="linkToEntityName"></param>
            /// <param name="linkFromAttributeName"></param>
            /// <param name="linkToAttributeName"></param>
            /// <returns></returns>
            public static LinkEntity AddChildLink(this LinkEntity link, string linkToEntityName,
                string linkFromAttributeName, string linkToAttributeName)
            {
                return link.AddChildLink(linkToEntityName, linkFromAttributeName, linkToAttributeName, JoinOperator.Inner);
            }

            /// <summary>
            /// Adds the new LinkEntity as a child to this LinkEntity, rather than this LinkEntity's LinkFrom Entity
            /// </summary>
            /// <param name="link"></param>
            /// <param name="linkToEntityName"></param>
            /// <param name="linkFromAttributeName"></param>
            /// <param name="linkToAttributeName"></param>
            /// <param name="joinType"></param>
            /// <returns></returns>
            public static LinkEntity AddChildLink(this LinkEntity link, string linkToEntityName,
                string linkFromAttributeName, string linkToAttributeName, JoinOperator joinType)
            {
                var child = new LinkEntity(
                    link.LinkToEntityName, linkToEntityName,
                    linkFromAttributeName, linkToAttributeName, joinType);
                link.LinkEntities.Add(child);
                return child;
            }
        #endregion AddChildLink

        #region AddLink
            /// <summary>
            /// Assumes that the linkFromAttributeName and the linkToAttributeName are the same
            /// </summary>
            /// <param name="link">The link.</param>
            /// <param name="linkToEntityName">Name of the link to entity.</param>
            /// <param name="linkAttributesName">Name of the link from and link to attribute.</param>
            /// <returns></returns>
            public static LinkEntity AddLink(this LinkEntity link, string linkToEntityName, string linkAttributesName)
            {
                return link.AddLink(linkToEntityName, linkAttributesName, linkAttributesName);
            }

            /// <summary>
            /// Assumes that the linkFromAttributeName and the linkToAttributeName are the same
            /// </summary>
            /// <param name="link">The link.</param>
            /// <param name="linkToEntityName">Name of the link to entity.</param>
            /// <param name="linkAttributesName">Name of the link from and link to attribute.</param>
            /// <param name="joinType">Type of the join to perform.</param>
            /// <returns></returns>
            public static LinkEntity AddLink(this LinkEntity link, string linkToEntityName, string linkAttributesName, JoinOperator joinType)
            {
                return link.AddLink(linkToEntityName, linkAttributesName, linkAttributesName, joinType);
            }
        #endregion AddLink

        #region AddLink<T>

        #region Same Link Attribute Name

            /// <summary>
            /// Adds the type T as a child linked entity to the LinkEntity, additionally ensuring it is active.
            /// Assumes both entities have the same attribute name to link on.
            /// </summary>
            /// <typeparam name="T">The type of the entity that is to be linked</typeparam>
            /// <param name="link">The link.</param>
            /// <param name="linkAttributesName">Name of the link from and link to attribute.</param>
            /// <returns></returns>
            public static LinkEntity AddLink<T>(this LinkEntity link, string linkAttributesName) where T : Entity
            {
                return link.AddLink<T>(linkAttributesName, linkAttributesName);
            }

            /// <summary>
            /// Adds the type T as a child linked entity to the LinkEntity, additionally ensuring it is active.
            /// Assumes both entities have the same attribute name to link on.
            /// </summary>
            /// <typeparam name="T">The type of the entity that is to be linked</typeparam>
            /// <param name="link">The link.</param>
            /// <param name="linkAttributesName">Name of the link from and link to attribute.</param>
            /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
            /// type are the column names to add</param>
            /// <returns></returns>
            public static LinkEntity AddLink<T>(this LinkEntity link, string linkAttributesName, Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
            {
                return link.AddLink(linkAttributesName, linkAttributesName, anonymousTypeInitializer);
            }

            /// <summary>
            /// Adds the type T as a child linked entity, additionally ensuring it is active.
            /// Assumes both entities have the same attribute name to link on.
            /// </summary>
            /// <typeparam name="T">The type of the entity that is to me linked</typeparam>
            /// <param name="link">The link.</param>
            /// <param name="linkAttributesName">Name of the link from and link to attribute.</param>
            /// <param name="joinType">Type of the join to perform.</param>
            /// <returns></returns>
            public static LinkEntity AddLink<T>(this LinkEntity link, string linkAttributesName, JoinOperator joinType) where T : Entity
            {
                return link.AddLink<T>(linkAttributesName, linkAttributesName, joinType);
            }

            /// <summary>
            /// Adds the type T as a child linked entity, additionally ensuring it is active.
            /// Assumes both entities have the same attribute name to link on.
            /// </summary>
            /// <typeparam name="T">The type of the entity that is to me linked</typeparam>
            /// <param name="link">The link.</param>
            /// <param name="linkAttributesName">Name of the link from and link to attribute.</param>
            /// <param name="joinType">Type of the join to perform.</param>
            /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
            /// type are the column names to add</param>
            /// <returns></returns>
            public static LinkEntity AddLink<T>(this LinkEntity link, string linkAttributesName, JoinOperator joinType, Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
            {
                return link.AddLink(linkAttributesName, linkAttributesName, joinType, anonymousTypeInitializer);
            }
        #endregion Same Link Attribute Name

        #region Different Link Attribute Names
            /// <summary>
            /// Adds the type T as a child linked entity, additionally ensuring it is active.
            /// </summary>
            /// <typeparam name="T">The type of the entity that is to me linked</typeparam>
            /// <param name="link">The link.</param>
            /// <param name="linkFromAttributeName">Name of the link from attribute.</param>
            /// <param name="linkToAttributeName">Name of the link to attribute.</param>
            /// <returns></returns>
            public static LinkEntity AddLink<T>(this LinkEntity link, string linkFromAttributeName, string linkToAttributeName) where T : Entity
            {
                var childLink = link.AddChildLink(EntityHelper.GetEntityLogicalName<T>(), linkFromAttributeName, linkToAttributeName);
                if (_activeOnly)
                {
                    childLink.LinkCriteria.ActiveOnly<T>();
                }
                return childLink;
            }

            /// <summary>
            /// Adds the type T as a child linked entity, additionally ensuring it is active.
            /// </summary>
            /// <typeparam name="T">The type of the entity that is to me linked</typeparam>
            /// <param name="link">The link.</param>
            /// <param name="linkFromAttributeName">Name of the link from attribute.</param>
            /// <param name="linkToAttributeName">Name of the link to attribute.</param>
            /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
            /// type are the column names to add</param>
            /// <returns></returns>
            public static LinkEntity AddLink<T>(this LinkEntity link, string linkFromAttributeName, string linkToAttributeName, Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
            {
                var childLink = link.AddLink<T>(linkFromAttributeName, linkToAttributeName);
                childLink.Columns.AddColumns(anonymousTypeInitializer);
                return childLink;
            }

            /// <summary>
            /// Adds the type T as a child linked entity, additionally ensuring it is active.
            /// </summary>
            /// <typeparam name="T">The type of Entity</typeparam>
            /// <param name="link">The link.</param>
            /// <param name="linkFromAttributeName">Name of the link from attribute.</param>
            /// <param name="linkToAttributeName">Name of the link to attribute.</param>
            /// <param name="joinType">Type of the join.</param>
            /// <returns></returns>
            public static LinkEntity AddLink<T>(this LinkEntity link, string linkFromAttributeName, string linkToAttributeName, JoinOperator joinType) where T : Entity
            {
                var childLink = link.AddChildLink(EntityHelper.GetEntityLogicalName<T>(), linkFromAttributeName, linkToAttributeName, joinType);
                if (_activeOnly)
                {
                    childLink.LinkCriteria.ActiveOnly<T>();
                }
                return childLink;
            }

            /// <summary>
            /// Adds the type T as a child linked entity, additionally ensuring it is active.
            /// </summary>
            /// <typeparam name="T">The type of Entity</typeparam>
            /// <param name="link">The link.</param>
            /// <param name="linkFromAttributeName">Name of the link from attribute.</param>
            /// <param name="linkToAttributeName">Name of the link to attribute.</param>
            /// <param name="joinType">Type of the join.</param>
            /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
            /// type are the column names to add</param>
            /// <returns></returns>
            public static LinkEntity AddLink<T>(this LinkEntity link, string linkFromAttributeName, string linkToAttributeName, JoinOperator joinType, Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
            {
                var childLink = link.AddLink<T>(linkFromAttributeName, linkToAttributeName, joinType);
                childLink.Columns.AddColumns(anonymousTypeInitializer);
                return childLink;
            }
        #endregion Different Link Attribute Names

        #endregion AddLink<T>

        #endregion LinkEntity

        #region QueryExpression

        #region AddLink
        /// <summary>
        /// Adds a LinkEntity to the Query Expression, returning it.
        /// </summary>
        /// <param name="qe">The qe.</param>
        /// <param name="linkToEntityName">Name of the link to entity.</param>
        /// <param name="linkAttributesName">Name of the link attributes.</param>
        /// <returns></returns>
        public static LinkEntity AddLink(this QueryExpression qe, string linkToEntityName, string linkAttributesName)
            {
                return qe.AddLink(linkToEntityName, linkAttributesName, linkAttributesName);
            }

        /// <summary>
        /// Adds a LinkEntity to the Query Expression, returning it.
        /// </summary>
        /// <param name="qe">The qe.</param>
        /// <param name="linkToEntityName">Name of the link to entity.</param>
        /// <param name="linkAttributesName">Name of the link attributes.</param>
        /// <param name="joinType">Type of the join.</param>
        /// <returns></returns>
        public static LinkEntity AddLink(this QueryExpression qe, string linkToEntityName, string linkAttributesName, JoinOperator joinType)
            {
                return qe.AddLink(linkToEntityName, linkAttributesName, linkAttributesName, joinType);
            }
        #endregion AddLink

        #region AddLink<T>

        #region Same Link Attribute Name
            /// <summary>
            /// Adds the type T as a linked entity to the query expression, additionally ensuring it is active.
            /// Assumes both entities have the same attribute name to link on.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="qe"></param>
            /// <param name="linkAttributesName"></param>
            /// <returns></returns>
            public static LinkEntity AddLink<T>(this QueryExpression qe, string linkAttributesName) where T : Entity
            {
                return qe.AddLink<T>(linkAttributesName, linkAttributesName);
            }

            /// <summary>
            /// Adds the type T as a linked entity to the query expression, additionally ensuring it is active.
            /// Assumes both entities have the same attribute name to link on.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="qe"></param>
            /// <param name="linkAttributesName"></param>
            /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
            /// type are the column names to add</param>
            /// <returns></returns>
            public static LinkEntity AddLink<T>(this QueryExpression qe, string linkAttributesName, Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
            {
                return qe.AddLink(linkAttributesName, linkAttributesName, anonymousTypeInitializer);
            }

            /// <summary>
            /// Adds the type T as a linked entity, additionally ensuring it is active.
            /// Assumes both entities have the same attribute name to link on.
            /// </summary>
            /// <typeparam name="T">The type of the entity that is to be linked and asserted active.</typeparam>
            /// <param name="qe">The query expression.</param>
            /// <param name="linkAttributesName">Name of the link attributes.</param>
            /// <param name="joinType">Type of the join to perform.</param>
            /// <returns></returns>
            public static LinkEntity AddLink<T>(this QueryExpression qe, string linkAttributesName, JoinOperator joinType) where T : Entity
            {
                return qe.AddLink<T>(linkAttributesName, linkAttributesName, joinType);
            }

            /// <summary>
            /// Adds the type T as a linked entity, additionally ensuring it is active.
            /// Assumes both entities have the same attribute name to link on.
            /// </summary>
            /// <typeparam name="T">The type of the entity that is to be linked and asserted active.</typeparam>
            /// <param name="qe">The query expression.</param>
            /// <param name="linkAttributesName">Name of the link attributes.</param>
            /// <param name="joinType">Type of the join to perform.</param>
            /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
            /// type are the column names to add</param>
            /// <returns></returns>
            public static LinkEntity AddLink<T>(this QueryExpression qe, string linkAttributesName, JoinOperator joinType, Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
            {
                return qe.AddLink(linkAttributesName, linkAttributesName, joinType, anonymousTypeInitializer);
            }
        #endregion Same Link Attribute Name

        #region Different Link Attribute Names
            /// <summary>
            /// Adds the type T as a linked entity to the query expression, additionally ensuring it is active.
            /// </summary>
            /// <typeparam name="T">The type of the entity that is to be linked and asserted active.</typeparam>
            /// <param name="qe">The query expression.</param>
            /// <param name="linkFromAttributeName">Name of the link from attribute.</param>
            /// <param name="linkToAttributeName">Name of the link to attribute.</param>
            /// <returns></returns>
            public static LinkEntity AddLink<T>(this QueryExpression qe, string linkFromAttributeName, string linkToAttributeName) where T : Entity
            {
                var link = qe.AddLink(EntityHelper.GetEntityLogicalName<T>(), linkFromAttributeName, linkToAttributeName);
                if (_activeOnly)
                {
                    link.LinkCriteria.ActiveOnly<T>();
                }
                return link;
            }

            /// <summary>
            /// Adds the type T as a linked entity to the query expression, additionally ensuring it is active.
            /// </summary>
            /// <typeparam name="T">The type of the entity that is to be linked and asserted active.</typeparam>
            /// <param name="qe">The query expression.</param>
            /// <param name="linkFromAttributeName">Name of the link from attribute.</param>
            /// <param name="linkToAttributeName">Name of the link to attribute.</param>
            /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
            /// type are the column names to add</param>
            /// <returns></returns>
            public static LinkEntity AddLink<T>(this QueryExpression qe, string linkFromAttributeName, string linkToAttributeName, Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
            {
                var link = qe.AddLink(EntityHelper.GetEntityLogicalName<T>(), linkFromAttributeName, linkToAttributeName);
                if (_activeOnly)
                {
                    link.LinkCriteria.ActiveOnly<T>();
                }
                link.Columns.AddColumns(anonymousTypeInitializer);
                return link;
            }

            /// <summary>
            /// Adds the type T as a linked entity, additionally ensuring it is active.
            /// </summary>
            /// <typeparam name="T">The type of the entity that is to be linked and asserted active.</typeparam>
            /// <param name="qe">The query expression.</param>
            /// <param name="linkFromAttributeName">Name of the link from attribute.</param>
            /// <param name="linkToAttributeName">Name of the link to attribute.</param>
            /// <param name="joinType">Type of the join to perform.</param>
            /// <returns></returns>
            public static LinkEntity AddLink<T>(this QueryExpression qe, string linkFromAttributeName, string linkToAttributeName, JoinOperator joinType) where T : Entity
            {
                var link = qe.AddLink(EntityHelper.GetEntityLogicalName<T>(), linkFromAttributeName, linkToAttributeName, joinType);
                if (_activeOnly)
                {
                    link.LinkCriteria.ActiveOnly<T>();
                }
                return link;
            }

            /// <summary>
            /// Adds the type T as a linked entity, additionally ensuring it is active.
            /// </summary>
            /// <typeparam name="T">The type of the entity that is to be linked and asserted active.</typeparam>
            /// <param name="qe">The query expression.</param>
            /// <param name="linkFromAttributeName">Name of the link from attribute.</param>
            /// <param name="linkToAttributeName">Name of the link to attribute.</param>
            /// <param name="joinType">Type of the join to perform.</param>
            /// <param name="anonymousTypeInitializer">An Anonymous Type Initializer where the properties of the anonymous
            /// type are the column names to add</param>
            /// <returns></returns>
            public static LinkEntity AddLink<T>(this QueryExpression qe, string linkFromAttributeName, string linkToAttributeName, JoinOperator joinType, Expression<Func<T, object>> anonymousTypeInitializer) where T : Entity
            {
                var link = qe.AddLink(EntityHelper.GetEntityLogicalName<T>(), linkFromAttributeName, linkToAttributeName, joinType);
                if (_activeOnly)
                {
                    link.LinkCriteria.ActiveOnly<T>();
                }
                link.Columns.AddColumns(anonymousTypeInitializer);
                return link;
            }
        #endregion Different Link Attribute Names

        #endregion AddLink<T>

        #endregion QueryExpression
    }
}
