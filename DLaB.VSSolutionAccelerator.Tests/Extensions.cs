using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.VSSolutionAccelerator.Tests
{
    public static class Extensions
    {
        [DebuggerStepThrough]
        public static void HasLineContaining(this string[] lines, string value, string message = null)
        {
            Assert.IsTrue(lines.Count(l => l.Contains(value)) == 1, message);
        }

        [DebuggerStepThrough]
        public static void IsMissingLineContaining(this string[] lines, string value, string message = null)
        {
            Assert.IsFalse(lines.Any(l => l.Contains(value)), message);
        }

        public static void LinesAreEqual(this Assert assert, string[] expected, IEnumerable<string> actual)
        {
            var splitActual = string.Join(Environment.NewLine, actual).Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.IsTrue(i < splitActual.Length, "Actual is missing lines!");
                if (expected[i].Contains("SolutionGuid = "))
                {
                    Assert.IsTrue(splitActual[i].Contains("SolutionGuid = "), "Expected the Solution Guid, instead found " + splitActual);
                }
                else
                {
                    Assert.AreEqual(expected[i], splitActual[i]);
                }
            }
        }

        public static void LinesAreEqual(this Assert assert, string expected, IEnumerable<string> actual)
        {
            assert.LinesAreEqual(expected.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries), actual);
        }
    }
}
