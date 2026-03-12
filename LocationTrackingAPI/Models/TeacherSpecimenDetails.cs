using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocationTrackingAPI.Models
{
    public class TeacherSpecimenDetails
    {
        public int TSID { get; set; }
        public List<SpecimenTeachers> teachList { get; set; }
        public List<SpecimenStocks> stkList { get; set; }

    }

    public class SpecimenTeachers
    {
        public int Teacher_ID { get; set; }
        public int Institute_ID { get; set; }
        public string LastVisit { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string subject { get; set; }
        public string Designation { get; set; }
    }

    public class SpecimenStocks
    {
        public int ItemID { get; set; }
        public string ItemTitle { get; set; }
        public int Balance { get; set; }
        public int Stock_Item_Group_ID { get; set; }
        public string Stock_Item_Group { get; set; }
        public int Stock_Item_Sub_Group_ID { get; set; }
        public string Stock_Item_Sub_Group { get; set; }

        

    }
}