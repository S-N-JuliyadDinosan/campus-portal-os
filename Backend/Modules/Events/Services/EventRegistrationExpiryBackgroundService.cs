using CampusServicesPortal.Modules.Events.Interfaces.Repositories;

namespace CampusServicesPortal.Modules.Events.Services;

public sealed class EventRegistrationExpiryBackgroundService
    : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly ILogger<
        EventRegistrationExpiryBackgroundService> _logger;

    private static readonly TimeSpan CheckInterval =
        TimeSpan.FromMinutes(1);


    public EventRegistrationExpiryBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<EventRegistrationExpiryBackgroundService> logger)
    {
        _scopeFactory =
            scopeFactory;

        _logger =
            logger;
    }


    // =====================================================
    // BACKGROUND LOOP
    // =====================================================

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Event registration expiry background service started.");


        // Run once immediately when application starts.
        await ExpireOldHoldsAsync(
            stoppingToken);


        using var timer =
            new PeriodicTimer(
                CheckInterval);


        try
        {
            while (
                await timer.WaitForNextTickAsync(
                    stoppingToken))
            {
                await ExpireOldHoldsAsync(
                    stoppingToken);
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            // Normal application shutdown.
        }


        _logger.LogInformation(
            "Event registration expiry background service stopped.");
    }


    // =====================================================
    // EXPIRE HELD REGISTRATIONS
    // =====================================================

    private async Task ExpireOldHoldsAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            using var scope =
                _scopeFactory.CreateScope();


            var repository =
                scope.ServiceProvider
                    .GetRequiredService<
                        IEventRegistrationRepository>();


            var expiredCount =
                await repository
                    .ExpireHeldRegistrationsAsync(
                        DateTime.UtcNow,
                        cancellationToken);


            if (expiredCount > 0)
            {
                _logger.LogInformation(
                    "{ExpiredCount} expired event registration hold(s) were marked as Expired.",
                    expiredCount);
            }
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            // Application is stopping.
        }
        catch (Exception ex)
        {
            // Do not crash the entire API because
            // one background expiry cycle failed.

            _logger.LogError(
                ex,
                "Error while expiring event registration holds.");
        }
    }
}