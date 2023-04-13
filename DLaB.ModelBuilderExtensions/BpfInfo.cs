using System;
using System.Linq;

namespace DLaB.ModelBuilderExtensions
{
    public struct BpfInfo
    {
        private const string Bpf = "bpf";

        public bool IsBpfName { get; }
        public bool IsBpfRelationshipName { get; }
        /// <summary>
        /// The Id of the parsed Bpf
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// All text up to and including the last instance of "bpf_"
        /// </summary>
        public string Prefix { get; }
        /// <summary>
        /// Any Text that appears after the bpf id 
        /// </summary>
        public string Postfix { get; }

        public BpfInfo(string prefix, string id, string postfix)
        {
            IsBpfName = true;
            Id = id.ToUpper();
            Prefix = prefix;
            Postfix = postfix ?? string.Empty;
            IsBpfRelationshipName = !string.IsNullOrWhiteSpace(postfix);
        }

        public static BpfInfo Parse(string name)
        {
            var parts = name.Split('_').ToList();
            var bpfIndex = parts.FindLastIndex(p => p.ToLower() == Bpf);
            if (bpfIndex < 0)
            {
                return new BpfInfo();
            }

            var id = parts[bpfIndex + 1];
            if (!Guid.TryParse(id, out _))
            {
                return new BpfInfo();
            }

            var prefix = string.Join("_", parts.Take(bpfIndex + 1)) + "_";
            var postfix = bpfIndex + 2 < parts.Count
                ? "_" + string.Join("_", parts.Skip(bpfIndex + 2))
                : null;

            return new BpfInfo(prefix, id, postfix);
        }
    }
}
