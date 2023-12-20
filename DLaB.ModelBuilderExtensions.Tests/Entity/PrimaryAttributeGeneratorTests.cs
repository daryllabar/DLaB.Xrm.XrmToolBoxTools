using System;
using DLaB.ModelBuilderExtensions.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace DLaB.ModelBuilderExtensions.Tests.Entity
{
    [TestClass]
    public class PrimaryAttributeGeneratorTests
    {
        [TestMethod]
        public void GenerateAlternateKeyValue_SortsKeys()
        {
            var keys = new [] {
                new EntityKeyMetadata { KeyAttributes = new[] { "C", "b", "A", "d" } },
                new EntityKeyMetadata { KeyAttributes = new[] { "2", "1", "3" } },
                new EntityKeyMetadata { KeyAttributes = new[] { "First", "Second", "Third" } }};

            var expected = "1,2,3|A,b,C,d|First,Second,Third";
            
            foreach(var indexList in GetPer(keys.Length)) { 
                var fromIndex = 0;
                var testKeys = new EntityKeyMetadata[keys.Length];
                var testOrder = string.Empty;
                
                foreach (var index in indexList)
                {
                    testKeys[index] = keys[fromIndex++];
                    testOrder = keys[index].KeyAttributes.Aggregate(testOrder, (current, key) => current + key) + "|";
                    
                }

                Console.WriteLine(testOrder);
                Assert.AreEqual(expected, PrimaryAttributeGenerator.GenerateAlternateKeyValue(testKeys));
            }
        }

        private static void Swap(int[] list, int a, int b)
        {
            if (a == b)
            {
                return;
            }
            (list[b], list[a]) = (list[a], list[b]);
        }

        public static List<int[]> GetPer(int count)
        {
            var list = Enumerable.Range(0, count).ToArray();
            var x = list.Length - 1;
            var result = new List<int[]>();
            GetPer(result, list, 0, x);
            return result;
        }

        private static void GetPer(List<int[]> result, int[] list, int k, int m)
        {
            if (k == m)
            {
                result.Add((int[])list.Clone());
            }
            else
            {
                for (int i = k; i <= m; i++)
                {
                    Swap(list, k, i);
                    GetPer(result, list, k + 1, m);
                    Swap(list, k, i);
                }
            }
        }
    }
}
