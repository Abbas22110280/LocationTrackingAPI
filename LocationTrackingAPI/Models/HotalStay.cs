using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace LocationTrackingAPI.Models
{
    public class HotalStay
    {
        public int Id { get; set; }
        public string StartLatitude { get; set; }
        public string StartLongitude { get; set; }
        public string MyStartAddress { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string VisitName { get; set; }
        public string Remarks { get; set; }
        public string Purpose { get; set; }
        public string ImagePath { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public string DeviceId { get; set; }
        public int Mode { get; set; }

        public double HotelOwnArrangements { get; set; }

        public double Taki { get; set; }
        public double Auto { get; set; }

        public string VchNo { get; set; }

    }
}