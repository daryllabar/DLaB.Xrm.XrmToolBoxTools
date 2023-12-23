using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DLaB.ModelBuilderExtensions.Tests
{
    public static class TestProject
    {
        public static string GetResourceText<T>(string fullNamespaceName)
        {
            return typeof(T).Assembly.GetResourceText(fullNamespaceName);
        }

        public static string GetResourceText(string fullNamespaceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            return asm.GetResourceText(fullNamespaceName);
        }

        private static string GetResourceText(this Assembly asm, string fullNamespaceName)
        {
            var resources = asm.GetManifestResourceNames();
            if (!resources.Contains(fullNamespaceName))
            {
                var assemblyTitle = asm.ManifestModule.Name;
                if (assemblyTitle.EndsWith(".dll"))
                {
                    assemblyTitle = assemblyTitle.Substring(0, assemblyTitle.Length - 4);
                }

                var relativeName = assemblyTitle + "." + fullNamespaceName;
                if (!resources.Contains(relativeName))
                {
                    throw new Exception($"No resource found with name {fullNamespaceName} or {relativeName}.  Current resource names: {string.Join(", ", resources)}.");
                }

                fullNamespaceName = relativeName;
            }

            using (var stream = asm.GetManifestResourceStream(fullNamespaceName))
            {
                return stream == null
                    ? string.Empty
                    : new StreamReader(stream).ReadToEnd();
            }
        }
    }
}
