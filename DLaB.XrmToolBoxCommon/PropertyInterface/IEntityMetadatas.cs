using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Metadata;

namespace DLaB.XrmToolboxCommon.PropertyInterface
{
    public interface IEntityMetadatas
    {
        IEnumerable<EntityMetadata> EntityMetadatas { get; set; }
    }
}
