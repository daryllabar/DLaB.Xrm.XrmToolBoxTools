using Microsoft.Xrm.Sdk;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm.Sandbox.Serialization
#else
namespace Source.DLaB.Xrm.Sandbox.Serialization
#endif
	
{
    /// <summary>
    /// Extensions for the DLaB.Xrm.Sandbox.Serialization namespace
    /// </summary>
    public static class Extensions
    {
        #region string
#if !NET
        /// <summary>
        /// Deserializes the entity from a string xml value to a specific entity type.
        /// </summary>
        /// <param name="xml">The xml to deserialize.</param>
        /// <returns></returns>
        public static SerializableEntity DeserializeSerializedEntity(this string xml)
        {
            return xml.DeserializeDataObject<SerializableEntity>();
        }

        /// <summary>
        /// Deserializes the entity from a string xml value to an IExtensibleDataObject
        /// </summary>
        /// <param name="xml">The xml to deserialize.</param>
        /// <returns></returns>
        public static T DeserializeSerializedEntity<T>(this string xml) where T : Entity
        {
            var entity = DeserializeSerializedEntity(xml);
            return ((Entity) entity).AsEntity<T>();
        }
#endif
#endregion string
    }
}
