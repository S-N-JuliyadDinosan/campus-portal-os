namespace CampusServicesPortal.Modules.Hostels.DTOs;

    public class CreateHostelRequest
    {
        public string Name { get; set; } = string.Empty;

        public string? Location { get; set; }
    }
