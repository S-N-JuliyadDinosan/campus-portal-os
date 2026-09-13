namespace CampusServicesPortal.Modules.Hostels.DTOs;

    public class UpdateRoomRequest
    {
        public string RoomNumber { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public bool IsActive { get; set; }
    }

