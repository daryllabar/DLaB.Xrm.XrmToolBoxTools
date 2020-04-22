using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Crm.Services.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.CrmSvcUtilExtensions.Tests
{
    [TestClass]
    public class CrmSvcUtilTests
    {
        [TestMethod]
        public void CreateTestEntityFile()
        {
            var factory = new ServiceFactory();
            var customizeDom = new CrmSvcUtilExtensions.Entity.CustomizeCodeDomService();
            var codeGen =      new CrmSvcUtilExtensions.Entity.CustomCodeGenerationService(factory.GetService<ICodeGenerationService>());
            var filter =       new CrmSvcUtilExtensions.Entity.CodeWriterFilterService(factory.GetService<ICodeWriterFilterService>());

            TestFileCreation(factory, customizeDom, codeGen, filter);
        }

        [TestMethod]
        public void CreateTestOptionSetFile()
        {
            var factory = new ServiceFactory();
            var customizeDom = new OptionSet.CustomizeCodeDomService();
            var codeGen =      new OptionSet.CustomCodeGenerationService(factory.GetService<ICodeGenerationService>());
            var filter =       new OptionSet.CodeWriterFilterService(factory.GetService<ICodeWriterFilterService>());

            TestFileCreation(factory, customizeDom, codeGen, filter);
        }

        [TestMethod]
        public void CreateTestActionFile()
        {
            var factory = new ServiceFactory();
            var customizeDom = new Action.CustomizeCodeDomService();
            var codeGen =      new Action.CustomCodeGenerationService(factory.GetService<ICodeGenerationService>());
            var filter =       new Action.CodeWriterFilterService(factory.GetService<ICodeWriterFilterService>());

            TestFileCreation(factory, customizeDom, codeGen, filter);
        }

        private static void TestFileCreation(ServiceFactory factory, ICustomizeCodeDomService customizeDom, ICodeGenerationService codeGen, ICodeWriterFilterService filter)
        {
            if (!Debugger.IsAttached && !ConfigHelper.GetAppSettingOrDefault("TestFileCreation", false))
            {
                return;
            }

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

                    factory.Add(customizeDom);
                    factory.Add(codeGen);
                    factory.Add(filter);
                    factory.Add<INamingService>(new NamingService(factory.GetService<INamingService>()));

                    factory.GetService<ICodeGenerationService>().Write(factory.GetMetadata(), "CS", fileName, "DLaB.CrmSvcUtilExtensions.UnitTest", factory.ServiceProvider);
                }
                catch (Exception ex)
                {
                    // Line for adding a debug breakpoint
                    var message = ex.Message;
                    if (message != null)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
