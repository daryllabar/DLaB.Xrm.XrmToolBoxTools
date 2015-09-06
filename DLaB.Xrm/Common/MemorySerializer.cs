using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace DLaB.Xrm.Common
{
    /// <summary>
    /// Serializes and Deserializes objects to and from an in memory byte[]
    /// </summary>
    public class MemorySerializer
    {
        /// <summary>
        /// Deserializes the specified serialized value.
        /// </summary>
        /// <typeparam name="T">Must be type that is declared</typeparam>
        /// <param name="serializedValue">The serialized value.</param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] serializedValue)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using(MemoryStream stream = new MemoryStream(serializedValue)){
                return (T)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static byte[] Serialize(object value)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, value);
                return stream.ToArray();
            }
        }
    }
}
