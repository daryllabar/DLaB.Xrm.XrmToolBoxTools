using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.XrmToolBoxCommon.PropertyInterface
{
    public interface IGlobalOptionSets
    {
        IEnumerable<OptionSetMetadataBase> GlobalOptionSets { get; set; } 
    }
}