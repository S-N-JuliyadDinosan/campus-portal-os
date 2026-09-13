using CampusServicesPortal.Modules.Certificates.Entities;
using CampusServicesPortal.Modules.Complaints.Entities;
using CampusServicesPortal.Modules.Fees.Entities;
using CampusServicesPortal.Modules.Hostels.Entities;
using CampusServicesPortal.Modules.Labs.Entities;
using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Modules.Identity.Entities;
using CampusServicesPortal.Modules.Students.Entities;
using CampusServicesPortal.Modules.SystemSettings.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Data.SeedData;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext dbContext)
    {
        if (!await dbContext.Faculties.AnyAsync())
        {
            dbContext.Faculties.AddRange(
                new Faculty { Code = "CST", Name = "Computing and Technology" },
                new Faculty { Code = "BST", Name = "Applied Sciences" });
            await dbContext.SaveChangesAsync();
        }

        if (!await dbContext.Hostels.AnyAsync())
        {
            var mainHostel = new Hostel
            {
                Name = "Main Hostel",
                Location = "North Campus",
                IsActive = true
            };

            dbContext.Hostels.Add(mainHostel);
            await dbContext.SaveChangesAsync();

            dbContext.Rooms.AddRange(
                new Room
                {
                    HostelId = mainHostel.HostelId,
                    RoomNumber = "A101",
                    Capacity = 2,
                    IsActive = true
                },
                new Room
                {
                    HostelId = mainHostel.HostelId,
                    RoomNumber = "A102",
                    Capacity = 2,
                    IsActive = true
                });
            await dbContext.SaveChangesAsync();
        }

        if (!await dbContext.Labs.AnyAsync())
        {
            var computerLab = new Lab
            {
                Code = "CS-LAB-01",
                Name = "Computer Lab 01",
                LabType = LabType.Computer,
                Capacity = 30,
                IsActive = true
            };

            var scienceLab = new Lab
            {
                Code = "SCI-LAB-01",
                Name = "Science Lab 01",
                LabType = LabType.Science,
                Capacity = 25,
                IsActive = true
            };

            dbContext.Labs.AddRange(computerLab, scienceLab);
            await dbContext.SaveChangesAsync();

            dbContext.LabTimeSlots.AddRange(
                new LabTimeSlot
                {
                    LabId = computerLab.LabId,
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeOnly(9, 0),
                    EndTime = new TimeOnly(10, 30),
                    IsActive = true
                },
                new LabTimeSlot
                {
                    LabId = computerLab.LabId,
                    DayOfWeek = DayOfWeek.Wednesday,
                    StartTime = new TimeOnly(13, 0),
                    EndTime = new TimeOnly(14, 30),
                    IsActive = true
                },
                new LabTimeSlot
                {
                    LabId = scienceLab.LabId,
                    DayOfWeek = DayOfWeek.Tuesday,
                    StartTime = new TimeOnly(10, 0),
                    EndTime = new TimeOnly(11, 30),
                    IsActive = true
                });

            for (var i = 1; i <= 30; i++)
            {
                dbContext.LabSeats.Add(new LabSeat
                {
                    LabId = computerLab.LabId,
                    SeatNumber = $"PC-{i:00}",
                    IsActive = true
                });
            }

            await dbContext.SaveChangesAsync();
        }

        // Development bootstrap account so protected Admin APIs can be tested
        // immediately in Swagger. Change/remove this before production use.
        if (!await dbContext.Users.AnyAsync(x => x.Role == "Admin"))
        {
            var admin = new User
            {
                Email = "admin@campus.local",
                Role = "Admin",
                EmailVerified = true,
                IsActive = true
            };
            admin.PasswordHash = new PasswordHasher<User>()
                .HashPassword(admin, "Admin@123");
            dbContext.Users.Add(admin);
        }

        if (!await dbContext.ComplaintCategories.AnyAsync())
        {
            dbContext.ComplaintCategories.AddRange(
                new ComplaintCategory { Name = "Hostel" },
                new ComplaintCategory { Name = "Maintenance" },
                new ComplaintCategory { Name = "Academic" });
        }

        if (!await dbContext.CertificateTypes.AnyAsync())
        {
            dbContext.CertificateTypes.AddRange(
                new CertificateType { Name = "Bonafide Certificate" },
                new CertificateType { Name = "Transcript" },
                new CertificateType { Name = "Completion Letter" });
        }

        if (!await dbContext.FeeTypes.AnyAsync())
        {
            dbContext.FeeTypes.AddRange(
                new FeeType { Name = "Semester Fee" },
                new FeeType { Name = "Exam Fee" },
                new FeeType { Name = "Lab Fine" });
        }

        if (!await dbContext.SystemSettings.AnyAsync())
        {
            dbContext.SystemSettings.Add(
                new SystemSetting
                {
                    SettingKey = "ReservationHoldMinutes",
                    SettingValue = "15",
                    Description = "Shared hold period for lab and event reservations."
                });
        }

        await dbContext.SaveChangesAsync();
    }
}
