using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocationTrackingAPI.Models
{
    public class ResponseConversion
    {
        public string status { get; set; }
        public string message { get; set; }

        public object userData { get; set; }
    }
}