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
    }
}
