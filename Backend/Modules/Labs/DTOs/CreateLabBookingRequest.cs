namespace CampusServicesPortal.Modules.Labs.DTOs
{
    public class CreateLabBookingRequest
    {
        public int StudentId { get; set; }

        public int LabId { get; set; }

        public int LabTimeSlotId { get; set; }

        public int? LabSeatId { get; set; }

        public DateOnly BookingDate { get; set; }
    }
}
