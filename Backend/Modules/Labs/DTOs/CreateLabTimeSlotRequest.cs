namespace CampusServicesPortal.Modules.Labs.DTOs
{
    public class CreateLabTimeSlotRequest
    {
        public string DayOfWeek { get; set; } = string.Empty;

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }
}
