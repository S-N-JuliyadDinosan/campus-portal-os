namespace CampusServicesPortal.Modules.Hostels.DTOs;

    public class RoomResponse
    {
        public int RoomId { get; set; }

        public int HostelId { get; set; }

        public string RoomNumber { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }

