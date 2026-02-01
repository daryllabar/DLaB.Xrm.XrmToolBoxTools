using System;

namespace DLaB.ModelBuilderExtensions
{
    /// <summary>
    /// Allows for Skipping of Specific Naming Service Methods
    /// </summary>
    [Flags]
    public enum NamingServiceMethods
    {
        None = 0,
        GetNameForAttribute = 1 << 0,
        GetNameForEntity = 1 << 1,
        GetNameForEntitySet = 1 << 2,
        GetNameForMessagePair = 1 << 3,
        GetNameForOption = 1 << 4,
        GetNameForOptionSet = 1 << 5,
        GetNameForRelationship = 1 << 6,
        GetNameForRequestField = 1 << 7,
        GetNameForResponseField = 1 << 8,
        GetNameForServiceContext = 1 << 9
    }
}
