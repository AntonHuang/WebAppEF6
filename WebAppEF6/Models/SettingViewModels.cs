using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class SettingViewModel
    {
        public string ID { get;  set; }
        public string Name { get;  set; }
        public string Value { get;  set; }
        public string ValueType { get;  set; }
        public string Desc { get; set; }
    }

    public class PointRuleViewModel
    {
        public decimal Level0SelfRate { get; set; }
        public decimal Level0SonRate { get; set; }
        public decimal Level0GrandsonRate { get; set; }

        public decimal Level1SelfRate { get; set; }
        public decimal Level1SonRate { get; set; }
        public decimal Level1GrandsonRate { get; set; }

        public int AvailableAfter { get; set; }
    }

}
