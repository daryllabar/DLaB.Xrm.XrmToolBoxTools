using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DLaB.EarlyBoundGeneratorV2.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DLaB.ModelBuilderExtensions.Tests
{
    [TestClass]
    public class JsonTests
    {
        [TestMethod]
        public void MyTest()
        {
            var json = "{\"b\": 2, \"a\": 1, \"c\": 3}";
            var updates = new Dictionary<string, string>
            {
                { "a", "\"Ten\"" },
                { "d", "4" },
                { "array", "[1, \"2\", { \"a\": 1}]" },
                { "bool", "false" },
                { "obj", "{ \"nested\": \"Value\" }" },

    };
            var result = UpdateAndOrderJsonProperties(json, updates);
            Assert.Fail(result); // Expected: {"a": "10", "b": 2, "c": 3, "d": "4"}
        }

        // Pseudocode plan:
        // 1. Parse the input JSON string into a JsonDocument.
        // 2. Convert the root element to a Dictionary<string, JsonElement> for manipulation.
        // 3. Update or add properties as needed in the dictionary.
        // 4. Order the dictionary by property name.
        // 5. Write the dictionary back to a JSON string using Utf8JsonWriter.

        public static string UpdateAndOrderJsonProperties(string json, Dictionary<string, string> updates)
        {
            // 1. Parse the input JSON string
            using (var doc = JsonDocument.Parse(json))
            {
                // 2. Convert to Dictionary for manipulation
                var dict = doc.RootElement.EnumerateObject()
                    .ToDictionary(p => p.Name, p => p.Value.Clone());

                // 3. Update/add properties
                foreach (var kvp in updates)
                {
                    dict[kvp.Key] = JsonDocument.Parse(kvp.Value).RootElement.Clone();
                }

                // 4. Order by property name
                var ordered = dict.OrderBy(kvp => kvp.Key);

                // 5. Write back to JSON string
                using (var stream = new MemoryStream())
                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false }))
                {
                    writer.WriteStartObject();
                    foreach (var kvp in ordered)
                    {
                        writer.WritePropertyName(kvp.Key);
                        kvp.Value.WriteTo(writer);
                    }
                    writer.WriteEndObject();
                    writer.Flush();
                    return System.Text.Encoding.UTF8.GetString(stream.ToArray());
                }
            }
        }
        // Pseudocode plan:
        // 1. Accept a Dictionary<string, string> as input.
        // 2. For each key-value pair, create a JsonProperty or equivalent representation.
        // 3. Collect these into a List<JsonProperty> (or similar, depending on your JSON library).
        // 4. Return the list.

        // Since System.Text.Json does not expose a public JsonProperty constructor, 
        // you can create a list of JsonElement or use a helper class to represent properties for writing.
        // Here's a helper method to create a list of actions that write properties to a Utf8JsonWriter:

        public static List<Action<Utf8JsonWriter>> ToJsonProperties(Dictionary<string, string> dict)
        {
            var list = new List<Action<Utf8JsonWriter>>();
            foreach (var kvp in dict)
            {
                list.Add(writer =>
                {
                    writer.WritePropertyName(kvp.Key);
                    writer.WriteStringValue(kvp.Value);
                });
            }
            return list;
        }
    }
}
