using System.Collections.Generic;
using System.Reflection;
using System;
using System.Text.Json;
using static DLaB.EarlyBoundGeneratorV2.Settings.EarlyBoundGeneratorConfig;
using DLaB.Common;

namespace DLaB.EarlyBoundGeneratorV2.Settings
{
    public static class Extensions
    {
        public static void SetJsonArrayProperty(this Dictionary<string, JsonProperty> properties, string name, string value)
        {
            name = name.LowerFirstChar();
            value = value.FormatValueForConfig().Trim();
            if (value.Length == 0)
            {
                if (properties.ContainsKey(name))
                {
                    properties.Remove(name);
                }

                return;
            }

            value = value.Replace(@"\", @"\\").Replace(@"""", @"\""");
            var parts = value.Split('|');
            if (name == BuilderSettingsJsonNames.MessageNamesFilter)
            {
                // THIS IS A FIX FOR A CURRENT BUG WHERE EACH NAME NEEDS A * IN ORDER TO WORK!
                for (var i = 0; i < parts.Length; i++)
                {
                    if (!parts[i].Contains("*"))
                    {
                        parts[i] += "*";
                    }
                }
            }

            properties[name] = CreateJsonProperty(name, $"[\"{string.Join("\", \"", parts)}\"]");
        }

        private static JsonProperty CreateJsonProperty(string name, string json)
        {
            return CreateJsonProperty(name, JsonDocument.Parse(json).RootElement);
        }

        private static JsonProperty CreateJsonObjectProperty(string name, string value)
        {
            return CreateJsonProperty(name, JsonDocument.Parse("{\"value\": " + value + "}").RootElement.GetProperty("value"));
        }

        private static JsonProperty CreateJsonProperty(string name, JsonElement element)
        {
            var jsonPropertyConstructor = typeof(JsonProperty).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                new[] { typeof(JsonElement), typeof(string) }, null);
            if (jsonPropertyConstructor == null)
            {
                throw new NullReferenceException("Unable to lookup the non-public JsonProperty Constructor JsonProperty(JsonElement, string)!");
            }

            var property = (JsonProperty)jsonPropertyConstructor.Invoke(new object[]
            {
                element,
                name
            });
            return property;
        }

        public static void SetJsonPropertyIfPopulated(this Dictionary<string, JsonProperty> properties, string name, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }
            properties.SetJsonProperty(name,value);
        }

        public static void SetJsonProperty(this Dictionary<string, JsonProperty> properties, string name, string value)
        {
            name = name.LowerFirstChar();
            value = value.Replace(@"\", @"\\").Replace(@"""", @"\""");
            properties[name] = CreateJsonProperty(name, $"\"{value}\"");
        }

        public static void SetJsonProperty(this Dictionary<string, JsonProperty> properties, string name, bool value)
        {
            name = name.LowerFirstChar();
            properties[name] = CreateJsonObjectProperty(name, value.ToString().ToLower());
        }

        public static void AddProperty(this Utf8JsonWriter writer, string key, string value, bool keepWhiteSpace = false)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            writer.WriteString(key.LowerFirstChar(), value);
        }

        public static void AddProperty(this Utf8JsonWriter writer, string key, bool value)
        {
            writer.WriteBoolean(key.LowerFirstChar(), value);
        }

        public static void AddPropertyArray(this Utf8JsonWriter writer, string key, string value)
        {
            var values = FormatValueForConfig(value).Split('|');
            if (values.Length == 0 || values.Length == 1 && string.IsNullOrWhiteSpace(values[0]))
            {
                return;
            }
            writer.WritePropertyName(key.LowerFirstChar());
            writer.WriteStartArray();
            foreach (var splitValue in values)
            {
                writer.WriteStringValue(splitValue.Trim());
            }
            writer.WriteEndArray();
        }

        public static void AddPropertyDictionaryStringString(this Utf8JsonWriter writer, string key, string value, bool lowerCaseValues = true)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }
            
            writer.WritePropertyName(key.LowerFirstChar());
            writer.WriteStartObject();
            var dictionary = value.GetDictionary<string, string>(new ConfigKeyValueSplitInfo
            {
                ConvertValuesToLower = lowerCaseValues
            });
            foreach (var kvp in dictionary)
            {
                writer.WriteString(kvp.Key, kvp.Value);
            }
            writer.WriteEndObject();
        }

        public static void AddPropertyDictionaryStringHashString(this Utf8JsonWriter writer, string key, string value, bool lowerCaseValues = true)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            writer.WritePropertyName(key.LowerFirstChar());
            writer.WriteStartObject();
            var dictionary = value.GetDictionaryHash<string, string>(new ConfigKeyValuesSplitInfo
            {
                ConvertValuesToLower = lowerCaseValues
            });
            foreach (var kvp in dictionary)
            {
                writer.WritePropertyName(kvp.Key);
                writer.WriteStartArray();
                foreach (var hashValue in kvp.Value)
                {
                    writer.WriteStringValue(hashValue);
                }
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }

        public static string LowerFirstChar(this string value)
        {
            return char.ToLower(value[0]) + value.Substring(1);
        }

        public static string FormatValueForConfig(this string value, bool keepWhiteSpace = false)
        {
            value = value ?? string.Empty;
            return keepWhiteSpace
                ? value
                : value.Replace(" ", "")
                    .Replace("\n", "");
        }
    }
}
