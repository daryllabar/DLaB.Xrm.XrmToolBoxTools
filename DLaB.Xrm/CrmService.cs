using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk;
using System.Collections.Concurrent;
using System.Configuration;

namespace DLaB.Xrm
{
    /// <summary>
    /// Contains all the information necessary for creating an OrganizationServiceProxy connection to the CRM WCF web services.
    /// </summary>
    public class CrmService
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the CRM server URL.
        /// </summary>
        /// <value>
        /// The CRM server URL.
        /// </value>
        public virtual string CrmServerUrl { get; set; }
        
        /// <summary>
        /// Gets or sets the CRM SDK server URL.
        /// </summary>
        /// <value>
        /// The CRM SDK server URL.
        /// </value>
        public virtual string CrmSdkServerUrl { get; set; }
        
        /// <summary>
        /// Gets or sets the CRM organization.
        /// </summary>
        /// <value>
        /// The CRM organization.
        /// </value>
        public virtual string CrmOrganization { get; set; }
        
        /// <summary>
        /// Gets or sets the impersonation user id.  No Impersonation is used if it is Guid.Empty.
        /// </summary>
        /// <value>
        /// The impersonation user id or Guid.Empty if no impersonation is to be used.
        /// </value>
        public virtual Guid ImpersonationUserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable proxy types for the connection.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the connection will be created with proxy types enabled; otherwise, <c>false</c>.
        /// </value>
        public virtual bool EnableProxyTypes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this is an IFD deployement
        /// </summary>
        public virtual bool IsIfdDeployment { get; set; }

        /// <summary>
        /// Gets or sets a value the User Name, used only for IFD deployments
        /// </summary>
        public virtual string IfdUserName { get; set; }

        /// <summary>
        /// Gets or sets a value containing the User's Password, used only for IFD deployments
        /// </summary>
        public virtual string IfdPassword { get; set; }

        #endregion

        #region Non Public Properties

        private static ConcurrentDictionary<string, string> Settings { get; set; }

        private static ConcurrentDictionary<string, System.Reflection.Assembly> _crmEntitiesAssemblies = new ConcurrentDictionary<string, System.Reflection.Assembly>();

        #endregion // Non Public Properties

        #region Constructors

        static CrmService()
        {
            Settings = new ConcurrentDictionary<string, string>();
        }

        /// <summary>
        /// Factory method for creating CrmService objects
        /// </summary>
        /// <returns></returns>
        public static CrmService CreateCrmService()
        {
            var crmService = new CrmService();
            crmService.IsIfdDeployment = bool.Parse(ConfigurationManager.AppSettings.Get("isIfdDeployment") ?? "false");

            if (crmService.IsIfdDeployment)
            {
                crmService.IfdUserName = ConfigurationManager.AppSettings.Get("IfdUserName");
                crmService.IfdPassword = ConfigurationManager.AppSettings.Get("IfdPassword");
            }

            return crmService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrmService"/> class.
        /// </summary>
        public CrmService()
            : this(System.Configuration.ConfigurationManager.AppSettings["OrganizationName"])
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrmService"/> class.
        /// </summary>
        /// <param name="crmServerurl">The CRM serverurl.</param>
        /// <param name="crmSdkserverurl">The CRM sdkserverurl.</param>
        /// <param name="crmOrganization">The CRM organization.</param>
        public CrmService(string crmServerurl, string crmSdkserverurl, string crmOrganization)
        {
            CrmServerUrl = crmServerurl;
            CrmSdkServerUrl = crmSdkserverurl;
            CrmOrganization = crmOrganization;
            EnableProxyTypes = true;
        }

        /// <summary>
        /// Defaults to using the CrmServerUrl and CrmSdkServerUrl stored in the App.Config App Settings
        /// </summary>
        /// <param name="crmOrganization">The CRM organization name.</param>
        public CrmService(string crmOrganization)
            : this(crmOrganization, Guid.Empty)
        {

        }

        /// <summary>
        /// Defaults to using the CrmServerUrl and CrmSdkServerUrl stored in the App.Config App Settings
        /// </summary>
        /// <param name="crmOrganization">The CRM organization name.</param>
        /// <param name="impersonationUserId">The impersonation user id.</param>
        public CrmService(string crmOrganization, Guid impersonationUserId)
            : this(
                System.Configuration.ConfigurationManager.AppSettings["CrmServerUrl"],
                System.Configuration.ConfigurationManager.AppSettings["CrmSdkServerUrl"],
                crmOrganization,
                impersonationUserId)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrmService"/> class.
        /// </summary>
        /// <param name="crmServerurl">The CRM serverurl.</param>
        /// <param name="crmSdkServerUrl">The CRM sdk server url.</param>
        /// <param name="crmOrganization">The CRM organization.</param>
        /// <param name="impersonationUserId">The impersonation user id.</param>
        public CrmService(string crmServerurl,
                                string crmSdkServerUrl,
                                string crmOrganization,
                                Guid impersonationUserId)
            : this(crmServerurl, crmSdkServerUrl, crmOrganization)
        {
            ImpersonationUserId = impersonationUserId;
        }

        #endregion // Constructors

        #region Public Methods

        /// <summary>
        /// Gets the early bound proxy assembly as defined by the AppSettings.CrmEntities.ContextType.
        /// </summary>
        /// <returns></returns>
        public static System.Reflection.Assembly GetEarlyBoundProxyAssembly()
        {
            return GetEarlyBoundProxyAssembly(null);
        }

        /// <summary>
        /// Gets the early bound proxy assembly.
        /// </summary>
        /// <param name="assemblyQualifiedCrmContextTypeName">Name of the assembly qualified CRM context type.</param>
        /// <returns></returns>
        public static System.Reflection.Assembly GetEarlyBoundProxyAssembly(string assemblyQualifiedCrmContextTypeName)
        {
            System.Reflection.Assembly assembly;
            if (assemblyQualifiedCrmContextTypeName == null)
            {
                assemblyQualifiedCrmContextTypeName = ConfigurationManager.AppSettings["CrmEntities.ContextType"];
            }

            if (!_crmEntitiesAssemblies.TryGetValue(assemblyQualifiedCrmContextTypeName, out assembly))
            {
                assembly = Type.GetType(assemblyQualifiedCrmContextTypeName).Assembly;
                _crmEntitiesAssemblies.TryAdd(assemblyQualifiedCrmContextTypeName, assembly);
            }
            return assembly;
        }

        /// <summary>
        /// Creates an OrganizationServiceProxy.  This should always be wrapped in a using statement so the dispose method is called.
        /// </summary>
        /// <returns></returns>
        public static OrganizationServiceProxy CreateOrgProxy()
        {
            return CrmService.CreateOrgProxy(CrmService.CreateCrmService());
        }

        /// <summary>
        /// Creates an OrganizationServiceProxy using the specified settings.  This should always be wrapped in a using statement so the dispose method is called.
        /// </summary>
        /// <param name="settings">The settings .</param>
        /// <returns></returns>
        public static OrganizationServiceProxy CreateOrgProxy(CrmService settings)
        {
            return CreateOrgProxy(settings.CrmServerUrl, settings.CrmSdkServerUrl, settings.CrmOrganization, settings.ImpersonationUserId,
                                  settings.EnableProxyTypes, settings.IsIfdDeployment, settings.IfdUserName, settings.IfdPassword);
        }

        /// <summary>
        /// Creates an OrganizationServiceProxy using the specified settings.  This should always be wrapped in a using statement so the dispose method is called.
        /// </summary>
        /// <param name="crmOrganization">The CRM organization name.</param>
        /// <param name="impersonationUserId">The impersonation user id.</param>
        /// <returns></returns>
        public static OrganizationServiceProxy CreateOrgProxy(string crmOrganization, Guid impersonationUserId = new Guid())
        {
            return CreateOrgProxy(new CrmService(crmOrganization, impersonationUserId));
        }

        /// <summary>
        /// Creates an OrganizationServiceProxy using the specified settings.  This should always be wrapped in a using statement so the dispose method is called.
        /// </summary>
        /// <param name="crmOrganizationUrl">The CRM organization URL.</param>
        /// <param name="crmSdkServerUrl">The CRM Sdk server URL.</param>
        /// <param name="crmOrganization">The CRM organization.</param>
        /// <param name="impersonationUserId">The impersonation user id.</param>
        /// <param name="enableProxyTypes">if set to <c>true</c> [enable proxy types].</param>
        /// <param name="ifdUserName">IFD's User Name </param>
        /// <param name="ifdPassword">IFD User's Password</param>
        /// <param name="isIfdDeployment">Indicates whether this is an IFD Deployment.</param>
        /// <returns></returns>
        public static OrganizationServiceProxy CreateOrgProxy(string crmOrganizationUrl, string crmSdkServerUrl, string crmOrganization, Guid impersonationUserId = new Guid(), bool enableProxyTypes = true, bool isIfdDeployment = false, string ifdUserName = "", string ifdPassword = "")
        {
            crmOrganization = GetCrmOrganizationName(crmSdkServerUrl, crmOrganization);
            Uri orgUri = GetOrganizationServiceUri(crmOrganizationUrl, crmOrganization, isIfdDeployment);
            IServiceConfiguration<IOrganizationService> orgConfigInfo = ServiceConfigurationFactory.CreateConfiguration<IOrganizationService>(orgUri);

            var creds = new ClientCredentials();

            if (isIfdDeployment)
            {
                creds.UserName.UserName = ifdUserName;
                creds.UserName.Password = ifdPassword;
            }

            OrganizationServiceProxy orgService = new OrganizationServiceProxy(orgConfigInfo, creds);
            if (enableProxyTypes)
            {
                ;
                orgService.EnableProxyTypes(GetEarlyBoundProxyAssembly());
            }

            if (impersonationUserId != Guid.Empty)
            {
                orgService.CallerId = impersonationUserId;
            }

            return orgService;
        }

        /// <summary>
        /// Creates a Discovery Service Proxy
        /// </summary>
        /// <returns></returns>
        public static DiscoveryServiceProxy CreateDiscoveryProxy()
        {
            return CreateDiscoveryProxy(CrmService.CreateCrmService());
        }

        /// <summary>
        /// Creates a Discovery Service Proxy
        /// </summary>
        /// <param name="crmService"></param>
        /// <returns></returns>
        public static DiscoveryServiceProxy CreateDiscoveryProxy(CrmService crmService)
        {
            return CreateDiscoveryProxy(crmService.CrmSdkServerUrl, crmService.IsIfdDeployment, crmService.IfdUserName, crmService.IfdPassword);
        }

        /// <summary>
        /// Creates a Discovery Service Proxy object.
        /// </summary>
        /// <param name="crmSdkServerUrl">The discovery service base URL.</param>
        /// <param name="isIfdDeployment">Boolean indicating if this is an IFD.</param>
        /// <param name="ifdUserName">Admin UserName for IFD</param>
        /// <param name="ifdPassword">Admin PWD for IFD</param>
        /// <returns></returns>
        public static DiscoveryServiceProxy CreateDiscoveryProxy(string crmSdkServerUrl, bool isIfdDeployment = false, string ifdUserName = "", string ifdPassword = "")
        {
            Uri orgUri = GetDiscoveryServiceUri(crmSdkServerUrl);
            IServiceConfiguration<IOrganizationService> orgConfigInfo = ServiceConfigurationFactory.CreateConfiguration<IOrganizationService>(orgUri);

            var credentials = new ClientCredentials();

            if (isIfdDeployment)
            {
                credentials.UserName.UserName = ifdUserName;
                credentials.UserName.Password = ifdPassword;
            }

            IServiceConfiguration<IDiscoveryService> discoveryConfiguration =
                ServiceConfigurationFactory.CreateConfiguration<IDiscoveryService>(orgUri);
            SecurityTokenResponse userResponseWrapper = discoveryConfiguration.Authenticate(credentials);
            return new DiscoveryServiceProxy(discoveryConfiguration, userResponseWrapper);
        }

        /// <summary>
        /// Enables proxy types for a workflow IOrganizationServiceFactory.
        /// </summary>
        /// <param name="serviceFactory">The service factory.</param>
        public static void EnableWorkflowProxyTypes(IOrganizationServiceFactory serviceFactory)
        {
            var type = Type.GetType("Microsoft.Crm.Workflow.SynchronousRuntime.WorkflowContext, Microsoft.Crm.Workflow, Version=5.0.0.0");
            type.GetProperty("ProxyTypesAssembly").SetValue(serviceFactory, GetEarlyBoundProxyAssembly(), null);
        }

        #endregion // Public Methods

        #region Non Public Methods

        /// <summary>
        /// Validates if string value is a guid and if so returns the organization name
        /// </summary>
        /// <param name="crmDiscoveryUrl">The CRM discovery URL.</param>
        /// <param name="crmOrganizationId">The CRM organization id.</param>
        /// <returns>
        /// The Organization's URL Name
        /// </returns>
        private static string GetCrmOrganizationName(string crmDiscoveryUrl, string crmOrganizationId)
        {
            Guid orgId;
            string orgName = crmOrganizationId;
            if (Guid.TryParse(crmOrganizationId, out orgId))
            {
                if (!Settings.TryGetValue(crmOrganizationId, out orgName))
                {
                    orgName = GetOrganizationName(crmDiscoveryUrl, orgId);
                    Settings.TryAdd(crmOrganizationId, orgName);
                }
            }
            return orgName;
        }

        private static string GetOrganizationName(string crmDiscoveryUrl, Guid orgId)
        {
            IServiceConfiguration<IDiscoveryService> dinfo = ServiceConfigurationFactory.CreateConfiguration<IDiscoveryService>(GetDiscoveryServiceUri(crmDiscoveryUrl));

            var creds = new ClientCredentials();

            DiscoveryServiceProxy dsp = new DiscoveryServiceProxy(dinfo, creds);
            dsp.Authenticate();
            RetrieveOrganizationsRequest orgRequest = new RetrieveOrganizationsRequest();
            RetrieveOrganizationsResponse orgResponse = dsp.Execute(orgRequest) as RetrieveOrganizationsResponse;

            if (orgResponse == null || orgResponse.Details == null || orgResponse.Details.Count == 0)
            {
                throw new Exception("Organization not found");
            }

            return orgResponse.Details.First(o => o.OrganizationId == orgId).UrlName;
        }

        private static Uri GetDiscoveryServiceUri()
        {
            var crmService = CrmService.CreateCrmService();
            return GetDiscoveryServiceUri(crmService.CrmSdkServerUrl);
        }

        private static Uri GetDiscoveryServiceUri(string serverName)
        {
            return new Uri(string.Format(@"{0}/XRMServices/2011/Discovery.svc", serverName));
        }

        private static Uri GetOrganizationServiceUri(string serverName, string orgUrlName, bool isIfdDeployment = false)
        {
            if (!isIfdDeployment || (isIfdDeployment && !string.IsNullOrWhiteSpace(orgUrlName)))
                return new Uri(String.Format(@"{0}/{1}/XRMServices/2011/Organization.svc", serverName, orgUrlName));

            return new Uri(String.Format(@"{0}/XRMServices/2011/Organization.svc", serverName));
        }

        #endregion // Non Public Methods
    }
}
