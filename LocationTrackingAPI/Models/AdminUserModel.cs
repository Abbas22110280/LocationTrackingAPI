using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocationTrackingAPI.Models
{
    public class AdminUserModel
    {
        public int UserID { get; set; }
        
        public string UserName { get; set; }
        
        public string UserPassword { get; set; }
        public int ApplyDonetion { get; set; }
        public int RoleID { get; set; }
        public int isActive { get; set; }

        public string DeviceId { get; set; }
        public string DeviceCompany { get; set; }
        public string ModelName { get; set; }
        public string PlatformId { get; set; }
        public string DeviceVersion { get; set; }

        public int AllowMinDistance { get; set; }
    }
}