namespace CampusServicesPortal.Modules.Hostels.DTOs;

    public class CreateHostelApplicationRequest
    {
        public int StudentId { get; set; }

        public int PreferredHostelId { get; set; }

        public string AcademicYear { get; set; } = string.Empty;

        public string Semester { get; set; } = string.Empty;

        public string? SpecialRequirements { get; set; }
    }

