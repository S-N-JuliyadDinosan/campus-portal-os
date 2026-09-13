namespace CampusServicesPortal.Modules.Hostels.DTOs;

    public class HostelResponse
    {
        public int HostelId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Location { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }

