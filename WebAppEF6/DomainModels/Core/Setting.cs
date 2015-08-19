using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.DomainModels.Core
{
    public class Setting
    {
        public Setting (){
            CreateDate = DateTime.Now;
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public string SettingValue { get; set; }
        public string SettingValueType { get; set; }
        public string Desc { get; set; }

        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }

    }

    public class SettingHistory 
    {

        public int ID { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }

        public string SID { get; set; }
        public string SName { get; set; }
        public string SValue { get; set; }
        public string SValueType { get; set; }
        public string SDesc { get; set; }
        public string SCreateBy { get; set; }
        public DateTime SCreateDate { get; set; }

        public SettingHistory()
        {
            CreateDate = DateTime.Now;
        }

        public SettingHistory(Setting setting) : this(){
            this.SID = setting.ID;
            this.SName = setting.Name;
            this.SValue = setting.SettingValue;
            this.SValueType = setting.SettingValueType;
            this.SDesc = setting.Desc;
            this.SCreateBy = setting.CreateBy;
            this.SCreateDate = setting.CreateDate;
        }

    }

    public enum SettingName {
        
        PointRule
        
    }
}
