using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLaB.EarlyBoundGenerator.Settings
{
    [Serializable]
    public class Argument
    {
        public CreationType SettingType { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public Argument()
        {
        }
    
        public Argument(CreationType settingType, string name, string value)
        {
            SettingType = settingType;
            Name = name;
            Value = value;
        }
    }
}
