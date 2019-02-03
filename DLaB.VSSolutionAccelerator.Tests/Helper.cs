using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.VSSolutionAccelerator.Tests
{
    public static class Helper
    {
        public static void AssertLinesAreEqual(string expected, IEnumerable<string> actual)
        {
            var splitExpected = expected.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var splitActual = string.Join(Environment.NewLine, actual).Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < splitExpected.Length; i++)
            {
                Assert.IsTrue(i < splitActual.Length, "Actual is missing lines!");
                if (splitExpected[i].Contains("SolutionGuid = "))
                {
                    Assert.IsTrue(splitActual[i].Contains("SolutionGuid = "), "Expected the Solution Guid, instead found " + splitActual);
                }
                else
                {
                    Assert.AreEqual(splitExpected[i], splitActual[i]);
                }
            }
        }
    }
}
