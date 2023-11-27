using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Sandbox.Serialization
#else
namespace Source.DLaB.Xrm.Sandbox.Serialization
#endif
	
{
    /// <summary>
    /// Sandbox Serializable Entity Collection
    /// </summary>
    [DataContract(Name = "EntityCollection", Namespace = "http://schemas.microsoft.com/xrm/2011/Contracts")]
    public class SerializableEntityCollection : IExtensibleDataObject
    {
        /// <summary>
        /// Gets or sets the entities.
        /// </summary>
        /// <value>
        /// The entities.
        /// </value>
        [DataMember]
        public List<SerializableEntity> Entities { get; set; }

        /// <summary>
        /// Gets or sets the name of the entity.
        /// </summary>
        /// <value>
        /// The name of the entity.
        /// </value>
        [DataMember]
        public string EntityName { get; set; }

        /// <summary>
        /// Gets or sets the minimum active row version.
        /// </summary>
        /// <value>
        /// The minimum active row version.
        /// </value>
        [DataMember]
        public string MinActiveRowVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [more records].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [more records]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool MoreRecords { get; set; }

        /// <summary>
        /// Gets or sets the paging cookie.
        /// </summary>
        /// <value>
        /// The paging cookie.
        /// </value>
        [DataMember]
        public string PagingCookie { get; set; }

        /// <summary>
        /// Gets or sets the total record count.
        /// </summary>
        /// <value>
        /// The total record count.
        /// </value>
        [DataMember]
        public int TotalRecordCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [total record count limit exceeded].
        /// </summary>
        /// <value>
        /// <c>true</c> if [total record count limit exceeded]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool TotalRecordCountLimitExceeded { get; set; }

        /// <summary>
        /// Gets or sets the extension data.
        /// </summary>
        /// <value>
        /// The extension data.
        /// </value>
        public ExtensionDataObject ExtensionData { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableEntityCollection"/> class.
        /// </summary>
        public SerializableEntityCollection()
        {
            Entities = new List<SerializableEntity>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableEntityCollection"/> class.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public SerializableEntityCollection(EntityCollection entities) : this()
        {
            foreach (var entity in entities.Entities)
            {
                Entities.Add(new SerializableEntity(entity));
            }
            EntityName = entities.EntityName;
            ExtensionData = entities.ExtensionData;
            MinActiveRowVersion = entities.MinActiveRowVersion;
            MoreRecords = entities.MoreRecords;
            PagingCookie = entities.PagingCookie;
            TotalRecordCount = entities.TotalRecordCount;
            TotalRecordCountLimitExceeded = entities.TotalRecordCountLimitExceeded;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="SerializableEntityCollection"/> to <see cref="EntityCollection"/>.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator EntityCollection(SerializableEntityCollection collection)
        {
            if (collection == null)
            {
                return null;
            }
            var xrmCollection = new EntityCollection
            {
                ExtensionData = collection.ExtensionData,
                EntityName = collection.EntityName,
                MinActiveRowVersion = collection.MinActiveRowVersion,
                MoreRecords = collection.MoreRecords,
                PagingCookie = collection.PagingCookie,
                TotalRecordCount = collection.TotalRecordCount,
                TotalRecordCountLimitExceeded = collection.TotalRecordCountLimitExceeded
            };
            xrmCollection.Entities.AddRange(collection.Entities.Select(v => (Entity)v));

            return xrmCollection;
        }
    }
}
