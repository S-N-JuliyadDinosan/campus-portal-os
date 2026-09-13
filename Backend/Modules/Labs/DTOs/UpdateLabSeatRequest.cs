namespace CampusServicesPortal.Modules.Labs.DTOs
{
    public class UpdateLabSeatRequest
    {
        public string SeatNumber { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
