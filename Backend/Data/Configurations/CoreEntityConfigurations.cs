using CampusServicesPortal.Common.Enums;
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
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicesPortal.Data.Configurations;

public sealed class UserConfiguration :
    IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.UserId);

        builder.Property(x => x.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Role)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(x => x.Email)
            .IsUnique();
    }
}

public sealed class StudentConfiguration :
    IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(x => x.StudentId);

        builder.Property(x => x.IndexNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.UserId)
            .IsUnique();

        builder.HasIndex(x => x.StudentMasterId)
            .IsUnique();

        builder.HasIndex(x => x.IndexNumber)
            .IsUnique();

        builder.HasOne(x => x.User)
            .WithOne(x => x.Student)
            .HasForeignKey<Student>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.StudentMaster)
            .WithOne(x => x.Student)
            .HasForeignKey<Student>(x => x.StudentMasterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Faculty)
            .WithMany(x => x.Students)
            .HasForeignKey(x => x.FacultyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class StudentMasterConfiguration :
    IEntityTypeConfiguration<StudentMaster>
{
    public void Configure(
        EntityTypeBuilder<StudentMaster> builder)
    {
        builder.HasKey(x => x.StudentMasterId);

        builder.ToTable("StudentMasterList");

        builder.Property(x => x.IndexNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.OfficialEmail)
            .HasMaxLength(256);

        builder.HasIndex(x => x.IndexNumber)
            .IsUnique();

        builder.HasOne(x => x.Faculty)
            .WithMany(x => x.StudentMasterRecords)
            .HasForeignKey(x => x.FacultyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class FacultyConfiguration :
    IEntityTypeConfiguration<Faculty>
{
    public void Configure(
        EntityTypeBuilder<Faculty> builder)
    {
        builder.HasKey(x => x.FacultyId);

        builder.Property(x => x.Code)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique();
    }
}

public sealed class HostelConfiguration :
    IEntityTypeConfiguration<Hostel>
{
    public void Configure(
        EntityTypeBuilder<Hostel> builder)
    {
        builder.HasKey(x => x.HostelId);

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();
    }
}

public sealed class RoomConfiguration :
    IEntityTypeConfiguration<Room>
{
    public void Configure(
        EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(x => x.RoomId);

        builder.Property(x => x.RoomNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.HostelId,
            x.RoomNumber
        })
        .IsUnique();

        builder.HasOne(x => x.Hostel)
            .WithMany(x => x.Rooms)
            .HasForeignKey(x => x.HostelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class HostelApplicationConfiguration :
    IEntityTypeConfiguration<HostelApplication>
{
    public void Configure(
        EntityTypeBuilder<HostelApplication> builder)
    {
        builder.HasKey(x => x.HostelApplicationId);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.AcademicYear)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Semester)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PreferredHostel)
            .WithMany(x => x.Applications)
            .HasForeignKey(x => x.PreferredHostelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AssignedRoom)
            .WithMany(x => x.AssignedApplications)
            .HasForeignKey(x => x.AssignedRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ReviewedByUser)
            .WithMany()
            .HasForeignKey(x => x.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class LabConfiguration :
    IEntityTypeConfiguration<Lab>
{
    public void Configure(
        EntityTypeBuilder<Lab> builder)
    {
        builder.HasKey(x => x.LabId);

        builder.Property(x => x.Code)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.LabType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(x => x.Code)
            .IsUnique();
    }
}

public sealed class LabTimeSlotConfiguration :
    IEntityTypeConfiguration<LabTimeSlot>
{
    public void Configure(
        EntityTypeBuilder<LabTimeSlot> builder)
    {
        builder.HasKey(x => x.LabTimeSlotId);

        builder.HasOne(x => x.Lab)
            .WithMany(x => x.TimeSlots)
            .HasForeignKey(x => x.LabId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class LabSeatConfiguration :
    IEntityTypeConfiguration<LabSeat>
{
    public void Configure(
        EntityTypeBuilder<LabSeat> builder)
    {
        builder.HasKey(x => x.LabSeatId);

        builder.Property(x => x.SeatNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.LabId,
            x.SeatNumber
        })
        .IsUnique();

        builder.HasOne(x => x.Lab)
            .WithMany(x => x.Seats)
            .HasForeignKey(x => x.LabId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class LabBookingConfiguration :
    IEntityTypeConfiguration<LabBooking>
{
    public void Configure(
        EntityTypeBuilder<LabBooking> builder)
    {
        builder.HasKey(x => x.LabBookingId);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Lab)
            .WithMany(x => x.Bookings)
            .HasForeignKey(x => x.LabId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.LabTimeSlot)
            .WithMany(x => x.Bookings)
            .HasForeignKey(x => x.LabTimeSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.LabSeat)
            .WithMany(x => x.Bookings)
            .HasForeignKey(x => x.LabSeatId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class VenueConfiguration :
    IEntityTypeConfiguration<Venue>
{
    public void Configure(
        EntityTypeBuilder<Venue> builder)
    {
        builder.HasKey(x => x.VenueId);

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.VenueType)
            .HasConversion<string>()
            .HasMaxLength(30);
    }
}

public sealed class EventConfiguration :
    IEntityTypeConfiguration<Event>
{
    public void Configure(
        EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(x => x.EventId);

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne(x => x.Venue)
            .WithMany(x => x.Events)
            .HasForeignKey(x => x.VenueId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class EventSeatConfiguration :
    IEntityTypeConfiguration<EventSeat>
{
    public void Configure(
        EntityTypeBuilder<EventSeat> builder)
    {
        builder.HasKey(x => x.EventSeatId);

        builder.Property(x => x.SeatNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.EventId,
            x.SeatNumber
        })
        .IsUnique();

        builder.HasOne(x => x.Event)
            .WithMany(x => x.Seats)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class EventRegistrationConfiguration :
    IEntityTypeConfiguration<EventRegistration>
{
    public void Configure(
        EntityTypeBuilder<EventRegistration> builder)
    {
        builder.HasKey(x => x.EventRegistrationId);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        // =====================================================
        // IMPORTANT
        //
        // Do NOT make StudentId + EventId unique.
        //
        // A student may have historical Cancelled / Expired
        // registrations and later register for the same event
        // again.
        //
        // Active duplicate registration is prevented by the
        // EventRegistrationService business rules.
        // =====================================================
        builder.HasIndex(x => new
        {
            x.StudentId,
            x.EventId
        });

        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Event)
            .WithMany(x => x.Registrations)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.EventSeat)
            .WithMany(x => x.Registrations)
            .HasForeignKey(x => x.EventSeatId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class ComplaintCategoryConfiguration :
    IEntityTypeConfiguration<ComplaintCategory>
{
    public void Configure(
        EntityTypeBuilder<ComplaintCategory> builder)
    {
        builder.HasKey(x => x.ComplaintCategoryId);

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}

public sealed class ComplaintConfiguration :
    IEntityTypeConfiguration<Complaint>
{
    public void Configure(
        EntityTypeBuilder<Complaint> builder)
    {
        builder.HasKey(x => x.ComplaintId);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ComplaintCategory)
            .WithMany(x => x.Complaints)
            .HasForeignKey(x => x.ComplaintCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.StatusChangedByUser)
            .WithMany()
            .HasForeignKey(x => x.StatusChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class CertificateTypeConfiguration :
    IEntityTypeConfiguration<CertificateType>
{
    public void Configure(
        EntityTypeBuilder<CertificateType> builder)
    {
        builder.HasKey(x => x.CertificateTypeId);

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}

public sealed class CertificateRequestConfiguration :
    IEntityTypeConfiguration<CertificateRequest>
{
    public void Configure(
        EntityTypeBuilder<CertificateRequest> builder)
    {
        builder.HasKey(x => x.CertificateRequestId);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CertificateType)
            .WithMany(x => x.Requests)
            .HasForeignKey(x => x.CertificateTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ReviewedByUser)
            .WithMany()
            .HasForeignKey(x => x.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class FeeTypeConfiguration :
    IEntityTypeConfiguration<FeeType>
{
    public void Configure(
        EntityTypeBuilder<FeeType> builder)
    {
        builder.HasKey(x => x.FeeTypeId);

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}

public sealed class FeePaymentConfiguration :
    IEntityTypeConfiguration<FeePayment>
{
    public void Configure(
        EntityTypeBuilder<FeePayment> builder)
    {
        builder.HasKey(x => x.FeePaymentId);

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.ReceiptNumber)
            .HasMaxLength(100);

        builder.HasIndex(x => x.ReceiptNumber)
            .IsUnique()
            .HasFilter("[ReceiptNumber] IS NOT NULL");

        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.FeeType)
            .WithMany(x => x.FeePayments)
            .HasForeignKey(x => x.FeeTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AssignedByUser)
            .WithMany()
            .HasForeignKey(x => x.AssignedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.LabBooking)
            .WithMany()
            .HasForeignKey(x => x.LabBookingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class NotificationConfiguration :
    IEntityTypeConfiguration<Notification>
{
    public void Configure(
        EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(x => x.NotificationId);

        builder.Property(x => x.Type)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class SystemSettingConfiguration :
    IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(
        EntityTypeBuilder<SystemSetting> builder)
    {
        builder.HasKey(x => x.SystemSettingId);

        builder.Property(x => x.SettingKey)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.SettingValue)
            .HasMaxLength(1000)
            .IsRequired();

        builder.HasIndex(x => x.SettingKey)
            .IsUnique();

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class RefreshTokenConfiguration :
    IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(
        EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.RefreshTokenId);

        builder.Property(x => x.TokenHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class PasswordResetTokenConfiguration :
    IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(
        EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.HasKey(x => x.PasswordResetTokenId);

        builder.Property(x => x.TokenHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.PasswordResetTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class EmailVerificationTokenConfiguration :
    IEntityTypeConfiguration<EmailVerificationToken>
{
    public void Configure(
        EntityTypeBuilder<EmailVerificationToken> builder)
    {
        builder.HasKey(x => x.EmailVerificationTokenId);

        builder.Property(x => x.TokenHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.EmailVerificationTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}