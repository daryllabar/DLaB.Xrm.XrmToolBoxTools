using System;

namespace DLaB.ModelBuilderExtensions
{
    public struct BpfInfo
    {
        private const string Bpf = "bpf_";

        public bool IsBpfName { get; }
        /// <summary>
        /// All text after the last instance of "bpf_"
        /// </summary>
        public string Postfix { get; }
        /// <summary>
        /// All text up to and including the last instance of "bpf_"
        /// </summary>
        public string Prefix { get; }

        public BpfInfo(string prefix, string postfix)
        {
            IsBpfName = true;
            Postfix = postfix.ToUpper();
            Prefix = prefix;
        }

        public static BpfInfo Parse(string name)
        {
            var searchName = name.ToLower();
            if (!searchName.Contains(Bpf))
            {
                return new BpfInfo();
            }

            var index = searchName.ToLower().LastIndexOf(Bpf, StringComparison.Ordinal) + Bpf.Length;
            var postText = name.Substring(index);
            return Guid.TryParse(postText, out _)
                ? new BpfInfo(name.Substring(0, index), postText)
                : new BpfInfo();
        }
    }
}
