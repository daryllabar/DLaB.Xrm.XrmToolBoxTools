﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DLaB.ModelBuilderExtensions
{
    internal class CustomTextWriter : TextWriter
    {
        private bool _skipRuntimeVersionComment;
        private bool _skipEntireHeaderComment;
        private readonly bool _makeReferenceTypesNullable;
        private readonly List<string> _invalidStringsForPropertiesNeedingNullableTypes;
        private int _commentSpacesCount;
        private const string CodeGeneratedByATool = "This code was generated by a tool."; // This line is the line before the RuntimeVersion comment
        private const string CommentSpaces = "//------------------------------------------------------------------------------"; // This the start and end of the header comment
        private bool _skipCurrentLine;
        private readonly MemoryStream _bufferStream;
        private readonly StreamWriter _default;
        private readonly string _outputFile;

        public CustomTextWriter(string outputFile, bool skipRuntimeVersionComment, bool skipEntireHeaderComment, bool makeReferenceTypesNullable, List<string> invalidStringsForPropertiesNeedingNullableTypes )
        {
            _outputFile = outputFile;
            _bufferStream = new MemoryStream();
            _default = makeReferenceTypesNullable
                ? new StreamWriter(_bufferStream)
                : new StreamWriter(outputFile);
            _skipRuntimeVersionComment = skipRuntimeVersionComment;
            _skipEntireHeaderComment = skipEntireHeaderComment;
            _makeReferenceTypesNullable = makeReferenceTypesNullable;
            _invalidStringsForPropertiesNeedingNullableTypes = invalidStringsForPropertiesNeedingNullableTypes;
        }

        public override void WriteLine(string s)
        {
            if (_skipEntireHeaderComment)
            {
                if(s == CommentSpaces)
                {
                    _commentSpacesCount++;
                }
                switch (_commentSpacesCount)
                {
                    case 1:
                        _skipCurrentLine = true;
                        return;
                    case 2:
                        _skipEntireHeaderComment = false;
                        _skipCurrentLine = false;
                        return;
                }
            }
            if (_skipRuntimeVersionComment)
            {
                if (_skipCurrentLine)
                {
                    _skipCurrentLine = false;
                    _skipRuntimeVersionComment = false;
                    return;
                }
                _skipCurrentLine = s == CodeGeneratedByATool;
            }

            _default.WriteLine(s);
        }

        public override void Write(string value)
        {
            if (_skipCurrentLine)
            {
                return;
            }

            _default.Write(value);
        }

        public override void Write(char value)
        {
            _default.Write(value);
        }

        public override Encoding Encoding => _default.Encoding;

        /// <summary>Releases the unmanaged resources used by the <see cref="T:System.IO.TextWriter" /> and optionally releases the managed resources.</summary>
        /// <param name="disposing">
        /// <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!_makeReferenceTypesNullable)
            {
                base.Dispose(disposing);
                _default.Dispose();
                return;
            }

            _default.Flush();
            _bufferStream.Flush();
            var text = Encoding.GetString(_bufferStream.ToArray());
            var lines = text.Split(new [] { Environment.NewLine }, StringSplitOptions.None);
            var updatedLines = lines.Select(UpdateLineForNullablePropertyTypes).ToList();
            File.WriteAllLines(_outputFile, updatedLines.Take(updatedLines.Count - 1), Encoding);
            base.Dispose(disposing);
            _default.Dispose();
            _bufferStream.Dispose();
        }

        private string UpdateLineForNullablePropertyTypes(string s)
        {
            var trimmed = s.Trim();

            if (!trimmed.StartsWith("public")
                || _invalidStringsForPropertiesNeedingNullableTypes.Any(trimmed.Contains)) return s;
            var parts = trimmed.Split(' ');
            if (parts.Length == 3)
            {
                s = s.Replace(parts[1] + " " + parts[2], parts[1] + "? " + parts[2]);
            }

            return s;
        }
    }
}
