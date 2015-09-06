using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLaB.XrmToolboxCommon
{
    /// <summary>
    /// Useful for puting in winform ObjectCollections
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectCollectionItem<T>
    {
        public string DisplayName { get; set; }
        public T Value { get; set; }

        public ObjectCollectionItem() { }

        public ObjectCollectionItem(string displayName, T value)
        {
            DisplayName = displayName;
            Value = value;
        }
    }
}
