using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Data;
using CampusServicesPortal.Modules.Events.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusServicesPortal.Modules.Labs.Services;

public sealed class LabBookingExpiryBackgroundService
    : BackgroundService
{
    private static readonly TimeSpan CheckInterval =
        TimeSpan.FromSeconds(60);

    private readonly IServiceScopeFactory _scopeFactory;

    private readonly ILogger<
        LabBookingExpiryBackgroundService> _logger;

    public LabBookingExpiryBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<LabBookingExpiryBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }


    // =====================================================
    // BACKGROUND LOOP
    // Runs approximately every 60 seconds.
    // =====================================================
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Reservation expiry background service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExpireReservationsAsync(
                    stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while expiring reservation holds.");
            }

            try
            {
                await Task.Delay(
                    CheckInterval,
                    stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation(
            "Reservation expiry background service stopped.");
    }


    // =====================================================
    // EXPIRE LAB + EVENT HOLDS
    // =====================================================
    private async Task ExpireReservationsAsync(
        CancellationToken cancellationToken)
    {
        using var scope =
            _scopeFactory.CreateScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

        var now =
            DateTime.UtcNow;


        // =================================================
        // LAB BOOKINGS
        // =================================================
        var expiredLabBookings =
            await dbContext.LabBookings
                .Where(x =>
                    x.Status ==
                        ReservationStatus.Held
                    &&
                    x.ExpiresAt.HasValue
                    &&
                    x.ExpiresAt.Value <= now)
                .ToListAsync(
                    cancellationToken);


        foreach (var booking in expiredLabBookings)
        {
            booking.Status =
                ReservationStatus.Expired;

            booking.ExpiresAt =
                null;
        }


        // =================================================
        // EVENT REGISTRATIONS
        // =================================================
        var expiredEventRegistrations =
            await dbContext.EventRegistrations
                .Where(x =>
                    x.Status ==
                        EventRegistrationStatus.Held
                    &&
                    x.ExpiresAt.HasValue
                    &&
                    x.ExpiresAt.Value <= now)
                .ToListAsync(
                    cancellationToken);


        foreach (var registration
                 in expiredEventRegistrations)
        {
            registration.Status =
                EventRegistrationStatus.Expired;

            registration.ExpiresAt =
                null;
        }


        // =================================================
        // NOTHING TO UPDATE
        // =================================================
        if (expiredLabBookings.Count == 0 &&
            expiredEventRegistrations.Count == 0)
        {
            return;
        }


        // =================================================
        // SAVE BOTH IN ONE DB SAVE
        // =================================================
        await dbContext.SaveChangesAsync(
            cancellationToken);


        _logger.LogInformation(
            "Expired {LabCount} lab booking hold(s) and " +
            "{EventCount} event registration hold(s).",
            expiredLabBookings.Count,
            expiredEventRegistrations.Count);
    }
}