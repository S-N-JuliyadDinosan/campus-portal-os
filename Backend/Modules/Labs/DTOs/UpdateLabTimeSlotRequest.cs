namespace CampusServicesPortal.Modules.Labs.DTOs
{
    public class UpdateLabTimeSlotRequest
    {
        public string DayOfWeek { get; set; } = string.Empty;

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public bool IsActive { get; set; }
    }
}
