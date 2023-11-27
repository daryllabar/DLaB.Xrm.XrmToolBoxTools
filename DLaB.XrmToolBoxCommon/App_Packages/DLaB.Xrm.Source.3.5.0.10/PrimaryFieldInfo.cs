using System.Collections.Generic;

#if DLAB_UNROOT_NAMESPACE || DLAB_XRM
namespace DLaB.Xrm
#else
namespace Source.DLaB.Xrm
#endif
	
{
    /// <summary>
    /// Contains information about the given Primary Name Field for an Entity
    /// </summary>
    public class PrimaryFieldInfo
    {
        /// <summary>
        /// Gets or sets the name of the attribute.
        /// </summary>
        /// <value>
        /// The name of the attribute.
        /// </value>
        public string AttributeName { get; set; }
        /// <summary>
        /// Gets or sets the maximum length.
        /// </summary>
        /// <value>
        /// The maximum length.
        /// </value>
        public int MaximumLength { get; set; }

        /// <summary>
        /// Gets the base attributes that make up the actual real name, so "firstname" and "lastname" for contact.
        /// </summary>
        /// <value>
        /// The base attributes.
        /// </value>
        public List<string> BaseAttributes { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryFieldInfo"/> class.
        /// </summary>
        public PrimaryFieldInfo()
        {
            MaximumLength = 100;
            ReadOnly = false;
            IsAttributeOf = false;
            BaseAttributes = new List<string>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the name field is [read only].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [read only]; otherwise, <c>false</c>.
        /// </value>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the name field is an attribute of another entity, and therefore, not created via standard Early Bound Generation.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is attribute of; otherwise, <c>false</c>.
        /// </value>
        public bool IsAttributeOf { get; set; }
    }
}
