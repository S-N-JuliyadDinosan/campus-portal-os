namespace CampusServicesPortal.Modules.Hostels.DTOs;

    public class HostelAvailabilityResponse
    {
        public int HostelId { get; set; }

        public string AcademicYear { get; set; } = string.Empty;

        public string Semester { get; set; } = string.Empty;

        public int TotalCapacity { get; set; }

        public int Occupied { get; set; }

        public int Available { get; set; }
    }

