using System;
using System.Diagnostics;

namespace DLaB.OutlookTimesheetCalculator
{
    [DebuggerDisplay("{Name}")]
    public class Task
    {
        private string _name;
        public string Name { get { return _name; } set { _name = value; SetRegExProperty(); } }
        public bool IsBillable { get; set; }
        public Guid Project { get; set; }
        public string Regex { get; set; }
        public bool IsRegExTemp { get; set; }

        private void SetRegExProperty()
        {
            string regEx;
            if (Name.Contains("*"))
            {
                // Encode special regular Expression Characters
                regEx = Name;
                foreach (char chr in "\\^$+?.(){}[]")
                {
                    regEx = regEx.Replace(chr.ToString(), "\\" + chr);
                }

                // Handle starting of Reg Ex
                if (Name.StartsWith("*"))
                {
                    if (Name.Length == 1)
                    {
                        Regex = ".";
                        return;
                    }
                    else
                    {
                        regEx = regEx.Substring(1, regEx.Length - 1);
                    }
                }
                else
                {
                    // ^ denotes start of string
                    regEx = "^" + regEx;
                }

                Regex = regEx.Replace("*", ".*");
            }
        }
    }
}
