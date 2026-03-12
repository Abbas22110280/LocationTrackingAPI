using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocationTrackingAPI.Models
{
    public class DistanceRequest
    {
        public string FromPostalCode { get; set; }
        public string ToPostalCode { get; set; }
        
    }

    public class DistanceResponse
    {
        public string FromPostalCode { get; set; }
        public string ToPostalCode { get; set; }
        public string FromLatitude { get; set; }
        public string FromLongitude { get; set; }

        public string ToLatitude { get; set; }
        public string ToLongitude { get; set; }
        public double TotalDistance { get; set; }
    }

    public class Matrix
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        
    }
}