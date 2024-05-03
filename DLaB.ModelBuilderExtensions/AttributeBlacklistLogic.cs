using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DLaB.ModelBuilderExtensions
{
    public class AttributeBlacklistLogic
    {
        public HashSet<string> Blacklist { get; }
        public List<string> BlacklistWildCards { get; }
        public Dictionary<string, HashSet<string>> BlacklistByEntity { get; }
        public Dictionary<string, List<string>> BlacklistWildCardsByEntity { get; }


        public AttributeBlacklistLogic(HashSet<string> blacklist)
        {
            Blacklist = new HashSet<string>();
            BlacklistWildCards = new List<string>();
            BlacklistByEntity = new Dictionary<string, HashSet<string>>();
            BlacklistWildCardsByEntity = new Dictionary<string, List<string>>();
            foreach (var item in blacklist) {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }

                var parts = item.Split('.');
                switch (parts.Length)
                {
                    case 1 when item.Contains("*"):
                        BlacklistWildCards.Add(ConvertAsteriskToWildcardSearch(item));
                        break;
                    case 1:
                        Blacklist.Add(item);
                        break;
                    case 2 when parts[1].Contains("*"):
                    {
                        if (!BlacklistWildCardsByEntity.TryGetValue(parts[0], out var entityWildCards))
                        {
                            entityWildCards = new List<string>();
                            BlacklistWildCardsByEntity[parts[0]] = entityWildCards;
                        }
                        entityWildCards.Add(ConvertAsteriskToWildcardSearch(parts[1]));
                        break;
                    }
                    case 2:
                    {
                        if (!BlacklistByEntity.TryGetValue(parts[0], out var entityAttributes))
                        {
                            entityAttributes = new HashSet<string>();
                            BlacklistByEntity[parts[0]] = entityAttributes;
                        }
                        entityAttributes.Add(parts[1]);

                        break;
                    }
                    default:
                        throw new Exception("Attribute Blacklist value was invalid.  Line: " + item);
                }
            }
        }

        public bool IsAllowed(string entityName, string value)
        {
            return !IsBlacklisted(entityName, value);
        }

        private bool IsBlacklisted(string entityName, string value)
        {
            return Blacklist.Contains(value)
                   || BlacklistWildCards.Any(pattern => Regex.Match(value, pattern).Success)
                   || BlacklistByEntity.TryGetValue(entityName, out var entityAttributes) && entityAttributes.Contains(value)
                   || BlacklistWildCardsByEntity.TryGetValue(entityName, out var entityWildCards) && entityWildCards.Any(pattern => Regex.Match(value, pattern).Success);
        }

        private string ConvertAsteriskToWildcardSearch(string value)
        {
            var start = value.StartsWith("*") ? "" : "^";
            var end = value.EndsWith("*") ? "" : "$";
            return start + Regex.Escape(value).Replace("\\*", ".*?") + end;
        }
    }
}
