namespace CampusServicesPortal.Common.Enums;

public enum HostelApplicationStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    RoomAssigned = 4,
    Cancelled = 5
}

public enum ReservationStatus
{
    Held = 1,
    Confirmed = 2,
    Cancelled = 3,
    Expired = 4
}

public enum ComplaintStatus
{
    Pending = 1,
    InProgress = 2,
    Resolved = 3
}

public enum CertificateRequestStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    ReadyForCollection = 4,
    Collected = 5
}

public enum FeePaymentStatus
{
    Outstanding = 1,
    Paid = 2,
    Cancelled = 3,
    Waived = 4
}

public enum LabType
{
    Computer = 1,
    Science = 2
}

public enum VenueType
{
    EventHall = 1,
    OpenSpace = 2
}
