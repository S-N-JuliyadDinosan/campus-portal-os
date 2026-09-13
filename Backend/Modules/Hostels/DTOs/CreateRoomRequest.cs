namespace CampusServicesPortal.Modules.Hostels.DTOs;

    public class CreateRoomRequest
    {
        public string RoomNumber { get; set; } = string.Empty;

        public int Capacity { get; set; }
    }

