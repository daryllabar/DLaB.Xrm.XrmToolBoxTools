using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.XrmToolboxCommon.PropertyInterface
{
    public interface IGlobalOptionSets
    {
        IEnumerable<OptionSetMetadataBase> GlobalOptionSets { get; set; } 
    }
}