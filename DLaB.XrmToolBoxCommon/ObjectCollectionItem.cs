using System.Diagnostics;

namespace DLaB.XrmToolBoxCommon
{
    /// <summary>
    /// Useful for putting in winform ObjectCollections
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("Name: {DisplayName}, Value: {Value}")]
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
