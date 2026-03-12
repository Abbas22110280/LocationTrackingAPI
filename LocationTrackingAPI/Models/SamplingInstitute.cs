using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocationTrackingAPI.Models
{
    public class SamplingInstitute
    {
        public int Insitute_id { get; set; }
        public string CODE { get; set; }
        public string Name { get; set; }
        public string NameWithCode { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Locality { get; set; }
        public string Insitute_Board { get; set; }
        public string Insitute_Category { get; set; }
        public string Insitute_Level { get; set; }
        public string State { get; set; }
        public string District { get; set; }
        public string LastVisit { get; set; }
        public int LedgerID { get; set; }
        
        public string LedgerName { get; set; }
        public int CountryID { get; set; }
        
        public string CountryName { get; set; }
        public int StateID { get; set; }
        
        public string StateName { get; set; }
        public int DistrictID { get; set; }
        
        public string DistrictName { get; set; }
        public int LocalityID { get; set; }
        
        public string LocalityName { get; set; }
        public int LevelID { get; set; }
        public string Level { get; set; }
        public int MediumID { get; set; }
        public string Medium { get; set; }
        public int BoardID { get; set; }
        public string Board { get; set; }
        public int CategoryID { get; set; }
        public string Category { get; set; }
        public int GradeID { get; set; }
        public string Grade { get; set; }
        public string PrefixUrl { get; set; }

        public string InstitutePinCode { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}