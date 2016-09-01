using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using Microsoft.Crm.Services.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
