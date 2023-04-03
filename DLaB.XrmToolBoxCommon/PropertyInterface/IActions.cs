using Microsoft.Xrm.Sdk;
using System.Collections.Generic;

namespace DLaB.XrmToolBoxCommon.PropertyInterface
{
    public interface IActions
    {
        IEnumerable<Entity> SdkMessages { get; set; }
    }
}
