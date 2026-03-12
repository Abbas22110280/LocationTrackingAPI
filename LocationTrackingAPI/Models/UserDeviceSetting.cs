using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocationTrackingAPI.Models
{
    public class UserDeviceSetting
    {
        public string GpsStatus { get; set; }
        public bool LocationServiceEnabled { get; set; }
        public string PermissionStatus { get; set; }
        public string Timestamp { get; set; }
        public string UserId { get; set; }
        public string DeviceId { get; set; }
    }
}