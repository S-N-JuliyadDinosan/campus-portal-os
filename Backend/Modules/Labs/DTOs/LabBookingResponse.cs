namespace CampusServicesPortal.Modules.Labs.DTOs
{
    public class LabBookingResponse
    {
        public int LabBookingId { get; set; }

        public int StudentId { get; set; }

        public int LabId { get; set; }

        public string? LabName { get; set; }

        public int LabTimeSlotId { get; set; }

        public string? DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int? LabSeatId { get; set; }

        public string? SeatNumber { get; set; }

        public DateOnly BookingDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime? ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
