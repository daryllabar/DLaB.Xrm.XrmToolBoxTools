using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DLaB.Log
{
    public class LoggerTextWriter: TextWriter
    {
        private readonly List<char> _cache = new List<char>();
        private static readonly char NewLineEnd = Environment.NewLine.ToCharArray().Last();
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);
        private DateTime _lastLogTime = DateTime.MinValue;

        public override Encoding Encoding => Encoding.Default;


        public override void Write(char value)
        {
            base.Write(value);
            _cache.Add(value);
            if (value == NewLineEnd)
            {
                var line = new string(_cache.Take(_cache.Count - Environment.NewLine.Length).ToArray());
                _cache.Clear();

                if (DateTime.UtcNow - _lastLogTime >= OneSecond)
                {
                    Logger.AddDetail(line);
                    _lastLogTime = DateTime.UtcNow;
                }
            }
        }

        public void FlushLogger()
        {
            if (_cache.Count > 0)
            {
                Logger.AddDetail(new string(_cache.ToArray()));
                _cache.Clear();
                _lastLogTime = DateTime.UtcNow;
            }
        }
    }
}
