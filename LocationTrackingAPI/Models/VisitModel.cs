namespace LocationTrackingAPI.Models
{
    public class VisitModel
    {
        public int Identity { get; set; }

        public int SchoolId { get; set; }
        public string SchoolName { get; set; }

        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int Pincode { get; set; }
        public string SchoolAddress { get; set; }
        public string MyStartAddress { get; set; }
        public string MyEndAddress { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
        public int VisitType { get; set; }
        public int Purpose { get; set; }
        public int VisitEnd { get; set; }

        public double? StartLatitude { get; set; }
        public double? StartLongitude { get; set; }
        public double? EndLatitude { get; set; }
        public double? EndLongitude { get; set; }

        public string Remarks { get; set; }

        public int CreatedBy { get; set; }
        public string VchNo { get; set; }

        public string DeviceId { get; set; }

        public double? approxDestinationLatitude { get; set; }
        public double? approxDestinationLongitude { get; set; }
        public double? approxDistance { get; set; }
        public double approxActualTravelDistance { get; set; }

        public bool IsLocationFoundOnMap { get; set; }
        public bool punchOutsideLocation { get; set; }

        public int Category { get; set; }
        public string UnitName { get; set; }

        public double? TravelRate { get; set; }

        public double? TravelClaimAmount { get; set; }

        public string StationCovered { get; set; }

        public int Mode { get; set; }

        
    }
}