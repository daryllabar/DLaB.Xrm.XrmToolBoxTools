using XrmToolBox.Extensibility.Interfaces;

namespace DLaB.XrmToolBoxCommon
{
    public class DLaBPluginControlBase : XrmToolBox.Extensibility.PluginControlBase, IGitHubPlugin, IPayPalPlugin, IHelpPlugin
    {
        #region XrmToolBox Menu Interfaces

        #region IGitHubPlugin

        public virtual string RepositoryName => "DLaB.Xrm.XrmToolBoxTools";
        public virtual string UserName => "daryllabar";

        #endregion IGitHubPlugin

        #region IPayPalPlugin

        public virtual string DonationDescription => "Support Development for the DLaB XTB Tools!";
        public virtual string EmailAccount => "d3labar@hotmail.com";

        #endregion IPayPalPlugin

        #region IHelpPlugin

        public virtual string HelpUrl => "https://github.com/daryllabar/DLaB.Xrm.XrmToolBoxTools/";

        #endregion IHelpPlugin

        #endregion XrmToolBox Menu Interfaces
    }
}
