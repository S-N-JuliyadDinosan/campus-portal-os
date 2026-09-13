namespace CampusServicesPortal.Modules.Labs.DTOs
{
    public class LabTimeSlotResponse
    {
        public int LabTimeSlotId { get; set; }

        public int LabId { get; set; }

        public string DayOfWeek { get; set; } = string.Empty;

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public bool IsActive { get; set; }
    }
}
