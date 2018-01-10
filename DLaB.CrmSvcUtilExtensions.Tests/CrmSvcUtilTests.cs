using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using DLaB.CrmSvcUtilExtensions.Entity;
using DLaB.CrmSvcUtilExtensions.OptionSet;
using Microsoft.Crm.Services.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Source.DLaB.Common;
using CustomCodeGenerationService = DLaB.CrmSvcUtilExtensions.Action.CustomCodeGenerationService;

namespace DLaB.CrmSvcUtilExtensions.Tests
{
    [TestClass]
    public class CrmSvcUtilTests
    {
        [TestMethod]
        public void CreateTestFile()
        {
            if (!ConfigHelper.GetAppSettingOrDefault("TestFileCreation", false))
            {
                return;
            }

            var factory = new ServiceFactory();
            using (var tmp = TempDir.Create())
            {
                var fileName = Path.Combine(tmp.Name, Guid.NewGuid() + ".txt");
                try
                {

                    //factory.Add<ICustomizeCodeDomService>(new CustomizeCodeDomService(new Dictionary<string, string>
                    //{
                    //    { "url", @"https://allegient.api.crm.dynamics.com/XRMServices/2011/Organization.svc"},
                    //    { "namespace", @"Test.Xrm.Entities"},
                    //    { "out", fileName },
                    //    {"servicecontextname", "CrmContext"},
                    //    {"codecustomization", "DLaB.CrmSvcUtilExtensions.Entity.CustomizeCodeDomService,DLaB.CrmSvcUtilExtensions"},
                    //    {"codegenerationservice", "DLaB.CrmSvcUtilExtensions.Entity.CustomCodeGenerationService,DLaB.CrmSvcUtilExtensions" },
                    //    {"codewriterfilter", "DLaB.CrmSvcUtilExtensions.Entity.CodeWriterFilterService,DLaB.CrmSvcUtilExtensions"},
                    //    {"metadataproviderservice:", "DLaB.CrmSvcUtilExtensions.Entity.MetadataProviderService,DLaB.CrmSvcUtilExtensions"},
                    //    {"namingservice", "DLaB.CrmSvcUtilExtensions.NamingService,DLaB.CrmSvcUtilExtensions"},
                    //    {"username", "dlabar@allegient.com"},
                    //    {"password", "*********"}
                    //}));

                    // factory.Add<ICodeGenerationService>(new CustomCodeGenerationService(factory.GetService<ICodeGenerationService>()));
                    // factory.Add<ICodeWriterFilterService>(new CodeWriterFilterService(factory.GetService<ICodeWriterFilterService>()));
                    // factory.Add<INamingService>(new NamingService(factory.GetService<INamingService>()));

                    factory.GetService<ICodeGenerationService>().
                            Write(factory.GetMetadata(), "CS", fileName, "DLaB.CrmSvcUtilExtensions.UnitTest", factory.ServiceProvider);
                }
                catch (Exception ex)
                {
                    // Line for adding a debug breakpoint
                    var message = ex.Message;
                }
            }
        }

        [TestMethod]
        public void UsingXrmClient_Should_GenerateAccountStateEnum()
        {
            ConfigProvider.Instance["UseXrmClient"] = "true";
            ConfigProvider.Instance["TestFileCreation"] = "true";


            CreateTestFile();
        }
    }
}
