using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.VSSolutionAccelerator.Tests
{
    public static class Extensions
    {
        [DebuggerHidden]
        public static void ExistsLineContaining(this Assert assert, string[] lines, string value, string message = null)
        {
            Assert.IsTrue(lines.Count(l => l.Contains(value)) >= 1, message);
        }

        [DebuggerHidden]
        public static void NotExistsLineContaining(this Assert assert, string[] lines, string value, string message = null)
        {
            Assert.IsFalse(lines.Any(l => l.Contains(value)), message);
        }

        [DebuggerHidden]
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

        [DebuggerHidden]
        public static void LinesAreEqual(this Assert assert, string expected, IEnumerable<string> actual)
        {
            assert.LinesAreEqual(expected.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries), actual);
        }
    }
}
