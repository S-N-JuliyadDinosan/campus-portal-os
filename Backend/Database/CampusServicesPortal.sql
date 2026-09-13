/* Campus Services Portal - SQL Server schema generated from EF Core InitialCreate migration. */
SET NOCOUNT ON;
SET XACT_ABORT ON;

IF DB_ID(N'CampusServicesPortalDb') IS NULL
BEGIN
    CREATE DATABASE [CampusServicesPortalDb];
END;
GO
USE [CampusServicesPortalDb];
GO

IF OBJECT_ID(N'[CertificateTypes]', N'U') IS NULL
BEGIN
    CREATE TABLE [CertificateTypes] (
        [CertificateTypeId] int IDENTITY(1,1) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Description] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_CertificateTypes] PRIMARY KEY ([CertificateTypeId])
    );
END;
GO

IF OBJECT_ID(N'[ComplaintCategories]', N'U') IS NULL
BEGIN
    CREATE TABLE [ComplaintCategories] (
        [ComplaintCategoryId] int IDENTITY(1,1) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Description] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_ComplaintCategories] PRIMARY KEY ([ComplaintCategoryId])
    );
END;
GO

IF OBJECT_ID(N'[Faculties]', N'U') IS NULL
BEGIN
    CREATE TABLE [Faculties] (
        [FacultyId] int IDENTITY(1,1) NOT NULL,
        [Code] nvarchar(30) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Faculties] PRIMARY KEY ([FacultyId])
    );
END;
GO

IF OBJECT_ID(N'[FeeTypes]', N'U') IS NULL
BEGIN
    CREATE TABLE [FeeTypes] (
        [FeeTypeId] int IDENTITY(1,1) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Description] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_FeeTypes] PRIMARY KEY ([FeeTypeId])
    );
END;
GO

IF OBJECT_ID(N'[Hostels]', N'U') IS NULL
BEGIN
    CREATE TABLE [Hostels] (
        [HostelId] int IDENTITY(1,1) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [Location] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Hostels] PRIMARY KEY ([HostelId])
    );
END;
GO

IF OBJECT_ID(N'[Labs]', N'U') IS NULL
BEGIN
    CREATE TABLE [Labs] (
        [LabId] int IDENTITY(1,1) NOT NULL,
        [Code] nvarchar(30) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [LabType] nvarchar(30) NOT NULL,
        [Capacity] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Labs] PRIMARY KEY ([LabId])
    );
END;
GO

IF OBJECT_ID(N'[Users]', N'U') IS NULL
BEGIN
    CREATE TABLE [Users] (
        [UserId] int IDENTITY(1,1) NOT NULL,
        [Email] nvarchar(256) NOT NULL,
        [PasswordHash] nvarchar(500) NOT NULL,
        [Role] nvarchar(30) NOT NULL,
        [EmailVerified] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [SecurityStamp] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
    );
END;
GO

IF OBJECT_ID(N'[Venues]', N'U') IS NULL
BEGIN
    CREATE TABLE [Venues] (
        [VenueId] int IDENTITY(1,1) NOT NULL,
        [Name] nvarchar(150) NOT NULL,
        [VenueType] nvarchar(30) NOT NULL,
        [Location] nvarchar(max) NOT NULL,
        [Capacity] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Venues] PRIMARY KEY ([VenueId])
    );
END;
GO

IF OBJECT_ID(N'[StudentMasterList]', N'U') IS NULL
BEGIN
    CREATE TABLE [StudentMasterList] (
        [StudentMasterId] int IDENTITY(1,1) NOT NULL,
        [IndexNumber] nvarchar(50) NOT NULL,
        [FullName] nvarchar(200) NOT NULL,
        [OfficialEmail] nvarchar(256) NULL,
        [FacultyId] int NOT NULL,
        [IntakeYear] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_StudentMasterList] PRIMARY KEY ([StudentMasterId]),
        CONSTRAINT [FK_StudentMasterList_Faculties_FacultyId] FOREIGN KEY ([FacultyId]) REFERENCES [Faculties] ([FacultyId]) ON DELETE NO ACTION
    );
END;
GO

IF OBJECT_ID(N'[Rooms]', N'U') IS NULL
BEGIN
    CREATE TABLE [Rooms] (
        [RoomId] int IDENTITY(1,1) NOT NULL,
        [HostelId] int NOT NULL,
        [RoomNumber] nvarchar(30) NOT NULL,
        [Capacity] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Rooms] PRIMARY KEY ([RoomId]),
        CONSTRAINT [FK_Rooms_Hostels_HostelId] FOREIGN KEY ([HostelId]) REFERENCES [Hostels] ([HostelId]) ON DELETE NO ACTION
    );
END;
GO

IF OBJECT_ID(N'[LabSeats]', N'U') IS NULL
BEGIN
    CREATE TABLE [LabSeats] (
        [LabSeatId] int IDENTITY(1,1) NOT NULL,
        [LabId] int NOT NULL,
        [SeatNumber] nvarchar(30) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_LabSeats] PRIMARY KEY ([LabSeatId]),
        CONSTRAINT [FK_LabSeats_Labs_LabId] FOREIGN KEY ([LabId]) REFERENCES [Labs] ([LabId]) ON DELETE NO ACTION
    );
END;
GO

IF OBJECT_ID(N'[LabTimeSlots]', N'U') IS NULL
BEGIN
    CREATE TABLE [LabTimeSlots] (
        [LabTimeSlotId] int IDENTITY(1,1) NOT NULL,
        [LabId] int NOT NULL,
        [DayOfWeek] int NOT NULL,
        [StartTime] time NOT NULL,
        [EndTime] time NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_LabTimeSlots] PRIMARY KEY ([LabTimeSlotId]),
        CONSTRAINT [FK_LabTimeSlots_Labs_LabId] FOREIGN KEY ([LabId]) REFERENCES [Labs] ([LabId]) ON DELETE NO ACTION
    );
END;
GO

IF OBJECT_ID(N'[EmailVerificationTokens]', N'U') IS NULL
BEGIN
    CREATE TABLE [EmailVerificationTokens] (
        [EmailVerificationTokenId] int IDENTITY(1,1) NOT NULL,
        [UserId] int NOT NULL,
        [TokenHash] nvarchar(500) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [UsedAt] datetime2 NULL,
        CONSTRAINT [PK_EmailVerificationTokens] PRIMARY KEY ([EmailVerificationTokenId]),
        CONSTRAINT [FK_EmailVerificationTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
    );
END;
GO

IF OBJECT_ID(N'[PasswordResetTokens]', N'U') IS NULL
BEGIN
    CREATE TABLE [PasswordResetTokens] (
        [PasswordResetTokenId] int IDENTITY(1,1) NOT NULL,
        [UserId] int NOT NULL,
        [TokenHash] nvarchar(500) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [UsedAt] datetime2 NULL,
        CONSTRAINT [PK_PasswordResetTokens] PRIMARY KEY ([PasswordResetTokenId]),
        CONSTRAINT [FK_PasswordResetTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
    );
END;
GO

IF OBJECT_ID(N'[RefreshTokens]', N'U') IS NULL
BEGIN
    CREATE TABLE [RefreshTokens] (
        [RefreshTokenId] int IDENTITY(1,1) NOT NULL,
        [UserId] int NOT NULL,
        [TokenHash] nvarchar(500) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [RevokedAt] datetime2 NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([RefreshTokenId]),
        CONSTRAINT [FK_RefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
    );
END;
GO

IF OBJECT_ID(N'[SystemSettings]', N'U') IS NULL
BEGIN
    CREATE TABLE [SystemSettings] (
        [SystemSettingId] int IDENTITY(1,1) NOT NULL,
        [SettingKey] nvarchar(150) NOT NULL,
        [SettingValue] nvarchar(1000) NOT NULL,
        [Description] nvarchar(max) NULL,
        [UpdatedByUserId] int NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_SystemSettings] PRIMARY KEY ([SystemSettingId]),
        CONSTRAINT [FK_SystemSettings_Users_UpdatedByUserId] FOREIGN KEY ([UpdatedByUserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
    );
END;
GO

IF OBJECT_ID(N'[Events]', N'U') IS NULL
BEGIN
    CREATE TABLE [Events] (
        [EventId] int IDENTITY(1,1) NOT NULL,
        [VenueId] int NOT NULL,
        [CreatedByUserId] int NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Description] nvarchar(max) NULL,
        [StartAt] datetime2 NOT NULL,
        [EndAt] datetime2 NOT NULL,
        [Capacity] int NOT NULL,
        [UsesReservedSeating] bit NOT NULL,
        [IsPublished] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Events] PRIMARY KEY ([EventId]),
        CONSTRAINT [FK_Events_Users_CreatedByUserId] FOREIGN KEY ([CreatedByUserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Events_Venues_VenueId] FOREIGN KEY ([VenueId]) REFERENCES [Venues] ([VenueId]) ON DELETE NO ACTION
    );
END;
GO

IF OBJECT_ID(N'[Students]', N'U') IS NULL
BEGIN
    CREATE TABLE [Students] (
        [StudentId] int IDENTITY(1,1) NOT NULL,
        [UserId] int NOT NULL,
        [StudentMasterId] int NOT NULL,
        [FacultyId] int NOT NULL,
        [IndexNumber] nvarchar(50) NOT NULL,
        [FullName] nvarchar(200) NOT NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [Address] nvarchar(max) NULL,
        [DeactivatedAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Students] PRIMARY KEY ([StudentId]),
        CONSTRAINT [FK_Students_Faculties_FacultyId] FOREIGN KEY ([FacultyId]) REFERENCES [Faculties] ([FacultyId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Students_StudentMasterList_StudentMasterId] FOREIGN KEY ([StudentMasterId]) REFERENCES [StudentMasterList] ([StudentMasterId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Students_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
    );
END;
GO

IF OBJECT_ID(N'[EventSeats]', N'U') IS NULL
BEGIN
    CREATE TABLE [EventSeats] (
        [EventSeatId] int IDENTITY(1,1) NOT NULL,
        [EventId] int NOT NULL,
        [SeatNumber] nvarchar(30) NOT NULL,
        [SectionName] nvarchar(max) NULL,
        [RowLabel] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_EventSeats] PRIMARY KEY ([EventSeatId]),
        CONSTRAINT [FK_EventSeats_Events_EventId] FOREIGN KEY ([EventId]) REFERENCES [Events] ([EventId]) ON DELETE NO ACTION
    );
END;
GO

IF OBJECT_ID(N'[CertificateRequests]', N'U') IS NULL
BEGIN
    CREATE TABLE [CertificateRequests] (
        [CertificateRequestId] int IDENTITY(1,1) NOT NULL,
        [CertificateTypeId] int NOT NULL,
        [StudentId] int NOT NULL,
        [ReviewedByUserId] int NULL,
        [Reason] nvarchar(max) NULL,
        [Status] nvarchar(30) NOT NULL,
        [ReviewNote] nvarchar(max) NULL,
        [ReviewedAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_CertificateRequests] PRIMARY KEY ([CertificateRequestId]),
        CONSTRAINT [FK_CertificateRequests_CertificateTypes_CertificateTypeId] FOREIGN KEY ([CertificateTypeId]) REFERENCES [CertificateTypes] ([CertificateTypeId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CertificateRequests_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([StudentId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CertificateRequests_Users_ReviewedByUserId] FOREIGN KEY ([ReviewedByUserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
    );
END;
GO

IF OBJECT_ID(N'[Complaints]', N'U') IS NULL
BEGIN
    CREATE TABLE [Complaints] (
        [ComplaintId] int IDENTITY(1,1) NOT NULL,
        [StudentId] int NOT NULL,
        [ComplaintCategoryId] int NOT NULL,
        [IsAnonymous] bit NOT NULL,
        [StatusChangedByUserId] int NULL,
        [Description] nvarchar(max) NOT NULL,
        [Status] nvarchar(30) NOT NULL,
        [ResolutionNote] nvarchar(max) NULL,
        [ResolvedAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Complaints] PRIMARY KEY ([ComplaintId]),
        CONSTRAINT [FK_Complaints_ComplaintCategories_ComplaintCategoryId] FOREIGN KEY ([ComplaintCategoryId]) REFERENCES [ComplaintCategories] ([ComplaintCategoryId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Complaints_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([StudentId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Complaints_Users_StatusChangedByUserId] FOREIGN KEY ([StatusChangedByUserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
    );
END;
GO

IF OBJECT_ID(N'[HostelApplications]', N'U') IS NULL
BEGIN
    CREATE TABLE [HostelApplications] (
        [HostelApplicationId] int IDENTITY(1,1) NOT NULL,
        [StudentId] int NOT NULL,
        [PreferredHostelId] int NOT NULL,
        [AssignedRoomId] int NULL,
        [ReviewedByUserId] int NULL,
        [AcademicYear] nvarchar(30) NOT NULL,
        [Semester] nvarchar(30) NOT NULL,
        [SpecialRequirements] nvarchar(max) NULL,
        [Status] nvarchar(30) NOT NULL,
        [ReviewNote] nvarchar(max) NULL,
        [ReviewedAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_HostelApplications] PRIMARY KEY ([HostelApplicationId]),
        CONSTRAINT [FK_HostelApplications_Hostels_PreferredHostelId] FOREIGN KEY ([PreferredHostelId]) REFERENCES [Hostels] ([HostelId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelApplications_Rooms_AssignedRoomId] FOREIGN KEY ([AssignedRoomId]) REFERENCES [Rooms] ([RoomId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelApplications_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([StudentId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HostelApplications_Users_ReviewedByUserId] FOREIGN KEY ([ReviewedByUserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
    );
END;
GO

IF OBJECT_ID(N'[LabBookings]', N'U') IS NULL
BEGIN
    CREATE TABLE [LabBookings] (
        [LabBookingId] int IDENTITY(1,1) NOT NULL,
        [StudentId] int NOT NULL,
        [LabId] int NOT NULL,
        [LabTimeSlotId] int NOT NULL,
        [LabSeatId] int NULL,
        [BookingDate] date NOT NULL,
        [Status] nvarchar(30) NOT NULL,
        [ExpiresAt] datetime2 NULL,
        [ConfirmedAt] datetime2 NULL,
        [CancelledAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_LabBookings] PRIMARY KEY ([LabBookingId]),
        CONSTRAINT [FK_LabBookings_LabSeats_LabSeatId] FOREIGN KEY ([LabSeatId]) REFERENCES [LabSeats] ([LabSeatId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LabBookings_LabTimeSlots_LabTimeSlotId] FOREIGN KEY ([LabTimeSlotId]) REFERENCES [LabTimeSlots] ([LabTimeSlotId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LabBookings_Labs_LabId] FOREIGN KEY ([LabId]) REFERENCES [Labs] ([LabId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LabBookings_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([StudentId]) ON DELETE NO ACTION
    );
END;
GO

IF OBJECT_ID(N'[Notifications]', N'U') IS NULL
BEGIN
    CREATE TABLE [Notifications] (
        [NotificationId] int IDENTITY(1,1) NOT NULL,
        [StudentId] int NOT NULL,
        [Type] nvarchar(80) NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        [IsRead] bit NOT NULL,
        [ReadAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY ([NotificationId]),
        CONSTRAINT [FK_Notifications_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([StudentId]) ON DELETE NO ACTION
    );
END;
GO

IF OBJECT_ID(N'[EventRegistrations]', N'U') IS NULL
BEGIN
    CREATE TABLE [EventRegistrations] (
        [EventRegistrationId] int IDENTITY(1,1) NOT NULL,
        [EventId] int NOT NULL,
        [StudentId] int NOT NULL,
        [EventSeatId] int NULL,
        [Status] nvarchar(30) NOT NULL,
        [ExpiresAt] datetime2 NULL,
        [RegisteredAt] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_EventRegistrations] PRIMARY KEY ([EventRegistrationId]),
        CONSTRAINT [FK_EventRegistrations_EventSeats_EventSeatId] FOREIGN KEY ([EventSeatId]) REFERENCES [EventSeats] ([EventSeatId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_EventRegistrations_Events_EventId] FOREIGN KEY ([EventId]) REFERENCES [Events] ([EventId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_EventRegistrations_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([StudentId]) ON DELETE NO ACTION
    );
END;
GO

IF OBJECT_ID(N'[FeePayments]', N'U') IS NULL
BEGIN
    CREATE TABLE [FeePayments] (
        [FeePaymentId] int IDENTITY(1,1) NOT NULL,
        [StudentId] int NOT NULL,
        [FeeTypeId] int NOT NULL,
        [AssignedByUserId] int NULL,
        [LabBookingId] int NULL,
        [BillingPeriod] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        [Amount] decimal(18,2) NOT NULL,
        [Status] nvarchar(30) NOT NULL,
        [DueDate] datetime2 NOT NULL,
        [AssignedAt] datetime2 NOT NULL,
        [PaidAt] datetime2 NULL,
        [PaymentMethod] nvarchar(max) NULL,
        [PaymentReference] nvarchar(max) NULL,
        [ReceiptNumber] nvarchar(100) NULL,
        [FineReason] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_FeePayments] PRIMARY KEY ([FeePaymentId]),
        CONSTRAINT [FK_FeePayments_FeeTypes_FeeTypeId] FOREIGN KEY ([FeeTypeId]) REFERENCES [FeeTypes] ([FeeTypeId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeePayments_LabBookings_LabBookingId] FOREIGN KEY ([LabBookingId]) REFERENCES [LabBookings] ([LabBookingId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeePayments_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([StudentId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FeePayments_Users_AssignedByUserId] FOREIGN KEY ([AssignedByUserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CertificateRequests_CertificateTypeId' AND object_id = OBJECT_ID(N'[CertificateRequests]'))
    CREATE INDEX [IX_CertificateRequests_CertificateTypeId] ON [CertificateRequests] ([CertificateTypeId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CertificateRequests_ReviewedByUserId' AND object_id = OBJECT_ID(N'[CertificateRequests]'))
    CREATE INDEX [IX_CertificateRequests_ReviewedByUserId] ON [CertificateRequests] ([ReviewedByUserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CertificateRequests_StudentId' AND object_id = OBJECT_ID(N'[CertificateRequests]'))
    CREATE INDEX [IX_CertificateRequests_StudentId] ON [CertificateRequests] ([StudentId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_CertificateTypes_Name' AND object_id = OBJECT_ID(N'[CertificateTypes]'))
    CREATE UNIQUE INDEX [IX_CertificateTypes_Name] ON [CertificateTypes] ([Name]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ComplaintCategories_Name' AND object_id = OBJECT_ID(N'[ComplaintCategories]'))
    CREATE UNIQUE INDEX [IX_ComplaintCategories_Name] ON [ComplaintCategories] ([Name]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Complaints_ComplaintCategoryId' AND object_id = OBJECT_ID(N'[Complaints]'))
    CREATE INDEX [IX_Complaints_ComplaintCategoryId] ON [Complaints] ([ComplaintCategoryId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Complaints_StatusChangedByUserId' AND object_id = OBJECT_ID(N'[Complaints]'))
    CREATE INDEX [IX_Complaints_StatusChangedByUserId] ON [Complaints] ([StatusChangedByUserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Complaints_StudentId' AND object_id = OBJECT_ID(N'[Complaints]'))
    CREATE INDEX [IX_Complaints_StudentId] ON [Complaints] ([StudentId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EmailVerificationTokens_UserId' AND object_id = OBJECT_ID(N'[EmailVerificationTokens]'))
    CREATE INDEX [IX_EmailVerificationTokens_UserId] ON [EmailVerificationTokens] ([UserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EventRegistrations_EventId' AND object_id = OBJECT_ID(N'[EventRegistrations]'))
    CREATE INDEX [IX_EventRegistrations_EventId] ON [EventRegistrations] ([EventId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EventRegistrations_EventSeatId' AND object_id = OBJECT_ID(N'[EventRegistrations]'))
    CREATE INDEX [IX_EventRegistrations_EventSeatId] ON [EventRegistrations] ([EventSeatId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EventRegistrations_StudentId_EventId' AND object_id = OBJECT_ID(N'[EventRegistrations]'))
    CREATE INDEX [IX_EventRegistrations_StudentId_EventId] ON [EventRegistrations] ([StudentId], [EventId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Events_CreatedByUserId' AND object_id = OBJECT_ID(N'[Events]'))
    CREATE INDEX [IX_Events_CreatedByUserId] ON [Events] ([CreatedByUserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Events_VenueId' AND object_id = OBJECT_ID(N'[Events]'))
    CREATE INDEX [IX_Events_VenueId] ON [Events] ([VenueId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_EventSeats_EventId_SeatNumber' AND object_id = OBJECT_ID(N'[EventSeats]'))
    CREATE UNIQUE INDEX [IX_EventSeats_EventId_SeatNumber] ON [EventSeats] ([EventId], [SeatNumber]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Faculties_Code' AND object_id = OBJECT_ID(N'[Faculties]'))
    CREATE UNIQUE INDEX [IX_Faculties_Code] ON [Faculties] ([Code]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_FeePayments_AssignedByUserId' AND object_id = OBJECT_ID(N'[FeePayments]'))
    CREATE INDEX [IX_FeePayments_AssignedByUserId] ON [FeePayments] ([AssignedByUserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_FeePayments_FeeTypeId' AND object_id = OBJECT_ID(N'[FeePayments]'))
    CREATE INDEX [IX_FeePayments_FeeTypeId] ON [FeePayments] ([FeeTypeId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_FeePayments_LabBookingId' AND object_id = OBJECT_ID(N'[FeePayments]'))
    CREATE INDEX [IX_FeePayments_LabBookingId] ON [FeePayments] ([LabBookingId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_FeePayments_ReceiptNumber' AND object_id = OBJECT_ID(N'[FeePayments]'))
    CREATE UNIQUE INDEX [IX_FeePayments_ReceiptNumber] ON [FeePayments] ([ReceiptNumber]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_FeePayments_StudentId' AND object_id = OBJECT_ID(N'[FeePayments]'))
    CREATE INDEX [IX_FeePayments_StudentId] ON [FeePayments] ([StudentId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_FeeTypes_Name' AND object_id = OBJECT_ID(N'[FeeTypes]'))
    CREATE UNIQUE INDEX [IX_FeeTypes_Name] ON [FeeTypes] ([Name]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HostelApplications_AssignedRoomId' AND object_id = OBJECT_ID(N'[HostelApplications]'))
    CREATE INDEX [IX_HostelApplications_AssignedRoomId] ON [HostelApplications] ([AssignedRoomId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HostelApplications_PreferredHostelId' AND object_id = OBJECT_ID(N'[HostelApplications]'))
    CREATE INDEX [IX_HostelApplications_PreferredHostelId] ON [HostelApplications] ([PreferredHostelId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HostelApplications_ReviewedByUserId' AND object_id = OBJECT_ID(N'[HostelApplications]'))
    CREATE INDEX [IX_HostelApplications_ReviewedByUserId] ON [HostelApplications] ([ReviewedByUserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_HostelApplications_StudentId' AND object_id = OBJECT_ID(N'[HostelApplications]'))
    CREATE INDEX [IX_HostelApplications_StudentId] ON [HostelApplications] ([StudentId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_LabBookings_LabId' AND object_id = OBJECT_ID(N'[LabBookings]'))
    CREATE INDEX [IX_LabBookings_LabId] ON [LabBookings] ([LabId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_LabBookings_LabSeatId' AND object_id = OBJECT_ID(N'[LabBookings]'))
    CREATE INDEX [IX_LabBookings_LabSeatId] ON [LabBookings] ([LabSeatId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_LabBookings_LabTimeSlotId' AND object_id = OBJECT_ID(N'[LabBookings]'))
    CREATE INDEX [IX_LabBookings_LabTimeSlotId] ON [LabBookings] ([LabTimeSlotId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_LabBookings_StudentId' AND object_id = OBJECT_ID(N'[LabBookings]'))
    CREATE INDEX [IX_LabBookings_StudentId] ON [LabBookings] ([StudentId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Labs_Code' AND object_id = OBJECT_ID(N'[Labs]'))
    CREATE UNIQUE INDEX [IX_Labs_Code] ON [Labs] ([Code]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_LabSeats_LabId_SeatNumber' AND object_id = OBJECT_ID(N'[LabSeats]'))
    CREATE UNIQUE INDEX [IX_LabSeats_LabId_SeatNumber] ON [LabSeats] ([LabId], [SeatNumber]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_LabTimeSlots_LabId' AND object_id = OBJECT_ID(N'[LabTimeSlots]'))
    CREATE INDEX [IX_LabTimeSlots_LabId] ON [LabTimeSlots] ([LabId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Notifications_StudentId' AND object_id = OBJECT_ID(N'[Notifications]'))
    CREATE INDEX [IX_Notifications_StudentId] ON [Notifications] ([StudentId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PasswordResetTokens_UserId' AND object_id = OBJECT_ID(N'[PasswordResetTokens]'))
    CREATE INDEX [IX_PasswordResetTokens_UserId] ON [PasswordResetTokens] ([UserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RefreshTokens_UserId' AND object_id = OBJECT_ID(N'[RefreshTokens]'))
    CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Rooms_HostelId_RoomNumber' AND object_id = OBJECT_ID(N'[Rooms]'))
    CREATE UNIQUE INDEX [IX_Rooms_HostelId_RoomNumber] ON [Rooms] ([HostelId], [RoomNumber]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_StudentMasterList_FacultyId' AND object_id = OBJECT_ID(N'[StudentMasterList]'))
    CREATE INDEX [IX_StudentMasterList_FacultyId] ON [StudentMasterList] ([FacultyId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_StudentMasterList_IndexNumber' AND object_id = OBJECT_ID(N'[StudentMasterList]'))
    CREATE UNIQUE INDEX [IX_StudentMasterList_IndexNumber] ON [StudentMasterList] ([IndexNumber]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Students_FacultyId' AND object_id = OBJECT_ID(N'[Students]'))
    CREATE INDEX [IX_Students_FacultyId] ON [Students] ([FacultyId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Students_IndexNumber' AND object_id = OBJECT_ID(N'[Students]'))
    CREATE UNIQUE INDEX [IX_Students_IndexNumber] ON [Students] ([IndexNumber]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Students_StudentMasterId' AND object_id = OBJECT_ID(N'[Students]'))
    CREATE UNIQUE INDEX [IX_Students_StudentMasterId] ON [Students] ([StudentMasterId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Students_UserId' AND object_id = OBJECT_ID(N'[Students]'))
    CREATE UNIQUE INDEX [IX_Students_UserId] ON [Students] ([UserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SystemSettings_SettingKey' AND object_id = OBJECT_ID(N'[SystemSettings]'))
    CREATE UNIQUE INDEX [IX_SystemSettings_SettingKey] ON [SystemSettings] ([SettingKey]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_SystemSettings_UpdatedByUserId' AND object_id = OBJECT_ID(N'[SystemSettings]'))
    CREATE INDEX [IX_SystemSettings_UpdatedByUserId] ON [SystemSettings] ([UpdatedByUserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Users_Email' AND object_id = OBJECT_ID(N'[Users]'))
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
GO

IF OBJECT_ID(N'[__EFMigrationsHistory]', N'U') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260811035929_InitialCreate')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId],[ProductVersion]) VALUES (N'20260811035929_InitialCreate',N'8.0.8');
GO

-- Minimal master data required by the BRD.
IF NOT EXISTS (SELECT 1 FROM [Faculties])
BEGIN
    INSERT INTO [Faculties] ([Code],[Name],[IsActive],[CreatedAt],[UpdatedAt]) VALUES
    (N'CST',N'Computing and Technology',1,SYSUTCDATETIME(),NULL),
    (N'BST',N'Applied Sciences',1,SYSUTCDATETIME(),NULL);
END;
GO

IF NOT EXISTS (SELECT 1 FROM [Hostels])
BEGIN
    INSERT INTO [Hostels] ([Name],[Location],[IsActive],[CreatedAt],[UpdatedAt])
    VALUES (N'Main Hostel',N'North Campus',1,SYSUTCDATETIME(),NULL);

    DECLARE @HostelId int = SCOPE_IDENTITY();
    INSERT INTO [Rooms] ([HostelId],[RoomNumber],[Capacity],[IsActive],[CreatedAt],[UpdatedAt]) VALUES
    (@HostelId,N'A101',2,1,SYSUTCDATETIME(),NULL),
    (@HostelId,N'A102',2,1,SYSUTCDATETIME(),NULL);
END;
GO

IF NOT EXISTS (SELECT 1 FROM [Labs])
BEGIN
    INSERT INTO [Labs] ([Code],[Name],[LabType],[Capacity],[IsActive],[CreatedAt],[UpdatedAt])
    VALUES (N'CS-LAB-01',N'Computer Lab 01',N'Computer',30,1,SYSUTCDATETIME(),NULL);
    DECLARE @ComputerLabId int = SCOPE_IDENTITY();

    INSERT INTO [Labs] ([Code],[Name],[LabType],[Capacity],[IsActive],[CreatedAt],[UpdatedAt])
    VALUES (N'SCI-LAB-01',N'Science Lab 01',N'Science',25,1,SYSUTCDATETIME(),NULL);
    DECLARE @ScienceLabId int = SCOPE_IDENTITY();

    INSERT INTO [LabTimeSlots] ([LabId],[DayOfWeek],[StartTime],[EndTime],[IsActive],[CreatedAt],[UpdatedAt]) VALUES
    (@ComputerLabId,1,'09:00:00','10:30:00',1,SYSUTCDATETIME(),NULL),
    (@ComputerLabId,3,'13:00:00','14:30:00',1,SYSUTCDATETIME(),NULL),
    (@ScienceLabId,2,'10:00:00','11:30:00',1,SYSUTCDATETIME(),NULL);

    DECLARE @SeatNo int = 1;
    WHILE @SeatNo <= 30
    BEGIN
        INSERT INTO [LabSeats] ([LabId],[SeatNumber],[IsActive],[CreatedAt],[UpdatedAt])
        VALUES (@ComputerLabId, N'PC-' + RIGHT(N'00' + CONVERT(nvarchar(2), @SeatNo), 2), 1, SYSUTCDATETIME(), NULL);
        SET @SeatNo += 1;
    END;
END;
GO

IF NOT EXISTS (SELECT 1 FROM [ComplaintCategories])
BEGIN
    INSERT INTO [ComplaintCategories] ([Name],[Description],[IsActive],[CreatedAt],[UpdatedAt]) VALUES
    (N'Hostel',NULL,1,SYSUTCDATETIME(),NULL),
    (N'Maintenance',NULL,1,SYSUTCDATETIME(),NULL),
    (N'Academic',NULL,1,SYSUTCDATETIME(),NULL);
END;
GO

IF NOT EXISTS (SELECT 1 FROM [CertificateTypes])
BEGIN
    INSERT INTO [CertificateTypes] ([Name],[Description],[IsActive],[CreatedAt]) VALUES
    (N'Bonafide Certificate',NULL,1,SYSUTCDATETIME()),
    (N'Transcript',NULL,1,SYSUTCDATETIME()),
    (N'Completion Letter',NULL,1,SYSUTCDATETIME());
END;
GO

IF NOT EXISTS (SELECT 1 FROM [FeeTypes])
BEGIN
    INSERT INTO [FeeTypes] ([Name],[Description],[IsActive],[CreatedAt],[UpdatedAt]) VALUES
    (N'Semester Fee',NULL,1,SYSUTCDATETIME(),NULL),
    (N'Exam Fee',NULL,1,SYSUTCDATETIME(),NULL),
    (N'Lab Fine',NULL,1,SYSUTCDATETIME(),NULL);
END;
GO

IF NOT EXISTS (SELECT 1 FROM [SystemSettings])
BEGIN
    INSERT INTO [SystemSettings] ([SettingKey],[SettingValue],[Description],[CreatedAt],[UpdatedAt])
    VALUES (N'ReservationHoldMinutes',N'15',N'Legacy hold period retained for backward compatibility.',SYSUTCDATETIME(),NULL);
END;
GO
