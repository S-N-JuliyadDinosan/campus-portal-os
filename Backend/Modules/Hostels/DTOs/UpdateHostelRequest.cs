namespace CampusServicesPortal.Modules.Hostels.DTOs;

    public class UpdateHostelRequest
    {
        public string Name { get; set; } = string.Empty;

        public string? Location { get; set; }

        public bool IsActive { get; set; }
    }

