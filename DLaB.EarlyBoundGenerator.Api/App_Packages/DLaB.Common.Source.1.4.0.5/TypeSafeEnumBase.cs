using System;

#if DLAB_UNROOT_COMMON_NAMESPACE
namespace DLaB.Common
#else
namespace Source.DLaB.Common
#endif
{
    /// <summary>
    /// Base class to Create Enums that are typed to something besides int, and allowed to be exapnded by other code bases
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public abstract class TypeSafeEnumBase<T>
    {
        /// <summary>
        /// The text name of the Enum.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }
        /// <summary>
        /// The value of the Enum.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeSafeEnumBase{T}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        protected TypeSafeEnumBase(string name, T value)
        {
            name.ThrowIfNull("name");
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="TypeSafeEnumBase{T}"/> to the given type T.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator T(TypeSafeEnumBase<T> t)
        {
            return t.Value;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Value == null ? String.Empty : Value.ToString();
        }
    }

    /*
     * public class CompanyEnum : TypeSafeEnumBase<Guid>
     * {
     *     // Keep this private to prevent anyone else from creating a value.  If this is public, may have to deal with overriding Equal and HashString to 
     *     // ensure that Values are compared, and not just references
     *     private CompanyEnum(string itemName, Guid itemId)
     *         : base(itemName, itemId)
     *     {
     *         //base class implemention is sufficient
     *     }
     *
     *     public static CompanyEnum AbcCorp = new CompanyEnum("Abc Corp", new Guid("A9A3FAC6-499C-401D-8E58-5BA38B73CA21"));
     *
     *
     *     public static CompanyEnum DefCorp = new CompanyEnum("DefCorp", new Guid("0D58B7F1-89D5-4EA6-A0E6-67D795AA7CE1"));
     * }

     */


}
