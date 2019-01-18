using System;
using System.Collections.Generic;
using System.Text;
using XrmToolBox.Extensibility;

namespace DLaB.XrmToolBoxCommon.Editors
{
    public interface IGetPluginControl<out T> : IGetPluginControl where T : PluginControlBase
    {
        new T GetPluginControl();
    }

    public interface IGetPluginControl
    {
        PluginControlBase GetPluginControl();
    }
}
