namespace CampusServicesPortal.Modules.Labs.DTOs
{
    public class LabResponse
    {
        public int LabId { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string LabType { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
