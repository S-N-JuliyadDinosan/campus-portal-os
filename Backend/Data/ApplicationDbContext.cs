using CampusServicesPortal.Common.Entities;
using CampusServicesPortal.Modules.Certificates.Entities;
using CampusServicesPortal.Modules.Complaints.Entities;
using CampusServicesPortal.Modules.Events.Entities;
using CampusServicesPortal.Modules.Fees.Entities;
using CampusServicesPortal.Modules.Hostels.Entities;
using CampusServicesPortal.Modules.Identity.Entities;
using CampusServicesPortal.Modules.Labs.Entities;
using CampusServicesPortal.Modules.Notifications.Entities;
using CampusServicesPortal.Modules.Students.Entities;
using CampusServicesPortal.Modules.SystemSettings.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Data;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<EmailVerificationToken> EmailVerificationTokens =>
        Set<EmailVerificationToken>();

    public DbSet<Student> Students => Set<Student>();
    public DbSet<StudentMaster> StudentMasterList => Set<StudentMaster>();
    public DbSet<Faculty> Faculties => Set<Faculty>();

    public DbSet<Hostel> Hostels => Set<Hostel>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<HostelApplication> HostelApplications =>
        Set<HostelApplication>();

    public DbSet<Lab> Labs => Set<Lab>();
    public DbSet<LabTimeSlot> LabTimeSlots => Set<LabTimeSlot>();
    public DbSet<LabSeat> LabSeats => Set<LabSeat>();
    public DbSet<LabBooking> LabBookings => Set<LabBooking>();

    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventSeat> EventSeats => Set<EventSeat>();
    public DbSet<EventRegistration> EventRegistrations =>
        Set<EventRegistration>();

    public DbSet<ComplaintCategory> ComplaintCategories =>
        Set<ComplaintCategory>();
    public DbSet<Complaint> Complaints => Set<Complaint>();

    public DbSet<CertificateType> CertificateTypes => Set<CertificateType>();
    public DbSet<CertificateRequest> CertificateRequests =>
        Set<CertificateRequest>();

    public DbSet<FeeType> FeeTypes => Set<FeeType>();
    public DbSet<FeePayment> FeePayments => Set<FeePayment>();

    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
