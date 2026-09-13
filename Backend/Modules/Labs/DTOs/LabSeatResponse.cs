namespace CampusServicesPortal.Modules.Labs.DTOs
{
    public class LabSeatResponse
    {
        public int LabSeatId { get; set; }

        public int LabId { get; set; }

        public string SeatNumber { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
