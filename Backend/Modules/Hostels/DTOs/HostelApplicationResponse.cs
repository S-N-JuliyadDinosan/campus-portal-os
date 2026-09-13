namespace CampusServicesPortal.Modules.Hostels.DTOs;

    public class HostelApplicationResponse
    {
        public int HostelApplicationId { get; set; }

        public int StudentId { get; set; }

        public int PreferredHostelId { get; set; }

        public string? PreferredHostelName { get; set; }

        public int? AssignedRoomId { get; set; }

        public string? AssignedRoomNumber { get; set; }

        public int? ReviewedByUserId { get; set; }

        public string Status { get; set; } = string.Empty;

        public string AcademicYear { get; set; } = string.Empty;

        public string Semester { get; set; } = string.Empty;

        public DateTime RequestedAt { get; set; }

        public DateTime? ReviewedAt { get; set; }
    }

