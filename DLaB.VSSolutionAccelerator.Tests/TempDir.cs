using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.VSSolutionAccelerator.Tests
{
    public interface ITempDir : IDisposable
    {
        string Name
        {
            get;
        }
    }

    public class TempDir : ITempDir
    {
        public string Name { get; }

        private static int _tempDirectoryCount = 0;

        /// <summary>
        /// Create a temp directory named after your test in the %temp%\uTest\xxx directory
        /// which is deleted and all sub directories when the ITempDir object is disposed.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ITempDir Create()
        {
            var skip = 1;
            while (skip < 10)
            {
                var stack = new StackTrace(skip++);
                var sf = stack.GetFrame(0);
                var method = sf.GetMethod();
                if (method.GetCustomAttributes(typeof(TestMethodAttribute), false).Length > 0)
                {
                    var name = sf.GetMethod().Name;
                    if (name.Length > 50)
                    {
                        name = name.Substring(0, 50) + name.GetHashCode();
                    }
                    return new TempDir(name);
                }
            }

            skip = 1;
            while (skip < 10)
            {
                var stack = new StackTrace(skip++);
                var sf = stack.GetFrame(0);
                var method = sf.GetMethod();
                if (method.GetCustomAttributes(typeof(TestInitializeAttribute), false).Length > 0)
                {
                    var name = Path.GetFileNameWithoutExtension(sf.GetFileName());
                    name += System.Threading.Interlocked.Increment(ref _tempDirectoryCount);
                    if (name.Length > 50)
                    {
                        name = name.Substring(0, 50) + name.GetHashCode();
                    }
                    return new TempDir(name);
                }
            }
            throw new InvalidOperationException("Unable to find a containing TestMethodAttribute or TestInitializeAttribute in the stack");
        }

        public TempDir(string dirName)
        {
            if (string.IsNullOrEmpty(dirName))
            {
                throw new ArgumentException("dirName");
            }
            Name = Path.Combine(Path.GetTempPath(), "VSSolutionAcceleratorTest", dirName);
            Directory.CreateDirectory(Name);
        }

        public void Dispose()
        {

            if (Name.Length < 10)
            {
                throw new InvalidOperationException($"Directory name {Name} seems to be invalid. Do not delete recursively your hard disc.");
            }

            // delete all files in temp directory
            foreach (var file in Directory.EnumerateFiles(Name, "*.*", SearchOption.AllDirectories))
            {
                // Long file name support
                File.Delete(@"\\?\" + file);
            }

            // and then the directory
            Directory.Delete(Name, true);
        }
    }
}
