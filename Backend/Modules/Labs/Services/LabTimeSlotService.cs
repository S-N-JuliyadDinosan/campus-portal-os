using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Modules.Labs.DTOs;
using CampusServicesPortal.Modules.Labs.Entities;
using CampusServicesPortal.Modules.Labs.Interfaces;

namespace CampusServicesPortal.Modules.Labs.Services;

public sealed class LabTimeSlotService : ILabTimeSlotService
{
    private readonly ILabTimeSlotRepository _timeSlotRepository;
    private readonly ILabRepository _labRepository;
    private readonly ILabBookingRepository _bookingRepository;

    public LabTimeSlotService(
        ILabTimeSlotRepository timeSlotRepository,
        ILabRepository labRepository,
        ILabBookingRepository bookingRepository)
    {
        _timeSlotRepository = timeSlotRepository;
        _labRepository = labRepository;
        _bookingRepository = bookingRepository;
    }


    // =====================================================
    // GET TIME SLOTS BY LAB
    // =====================================================
    public async Task<List<LabTimeSlotResponse>> GetByLabIdAsync(
        int labId)
    {
        var lab =
            await _labRepository.GetByIdAsync(labId);

        if (lab == null)
        {
            throw new KeyNotFoundException(
                "Lab not found.");
        }

        var timeSlots =
            await _timeSlotRepository.GetByLabIdAsync(
                labId);

        return timeSlots
            .Select(MapToResponse)
            .ToList();
    }


    // =====================================================
    // GET TIME SLOT BY ID
    // =====================================================
    public async Task<LabTimeSlotResponse?> GetByIdAsync(
        int labTimeSlotId)
    {
        var timeSlot =
            await _timeSlotRepository.GetByIdAsync(
                labTimeSlotId);

        return timeSlot == null
            ? null
            : MapToResponse(timeSlot);
    }


    // =====================================================
    // CREATE TIME SLOT
    // =====================================================
    public async Task<LabTimeSlotResponse> CreateAsync(
        int labId,
        CreateLabTimeSlotRequest request)
    {
        var lab =
            await _labRepository.GetByIdAsync(
                labId);

        if (lab == null)
        {
            throw new KeyNotFoundException(
                "Lab not found.");
        }

        if (!lab.IsActive)
        {
            throw new InvalidOperationException(
                "Cannot create a time slot for an inactive lab.");
        }

        if (!Enum.TryParse<DayOfWeek>(
                request.DayOfWeek,
                true,
                out var day))
        {
            throw new ArgumentException(
                "Invalid day of week.");
        }

        ValidateTimes(
            request.StartTime,
            request.EndTime);

        var exists =
            await _timeSlotRepository
                .TimeSlotExistsAsync(
                    labId,
                    day.ToString(),
                    request.StartTime,
                    request.EndTime);

        if (exists)
        {
            throw new InvalidOperationException(
                "This time slot already exists.");
        }

        var timeSlot = new LabTimeSlot
        {
            LabId = labId,
            DayOfWeek = day,
            StartTime =
                TimeOnly.FromTimeSpan(
                    request.StartTime),
            EndTime =
                TimeOnly.FromTimeSpan(
                    request.EndTime),
            IsActive = true
        };

        timeSlot =
            await _timeSlotRepository.AddAsync(
                timeSlot);

        return MapToResponse(timeSlot);
    }


    // =====================================================
    // UPDATE TIME SLOT
    // =====================================================
    public async Task<bool> UpdateAsync(
        int labTimeSlotId,
        UpdateLabTimeSlotRequest request)
    {
        var timeSlot =
            await _timeSlotRepository.GetByIdAsync(
                labTimeSlotId);

        if (timeSlot == null)
        {
            return false;
        }

        if (!Enum.TryParse<DayOfWeek>(
                request.DayOfWeek,
                true,
                out var day))
        {
            throw new ArgumentException(
                "Invalid day of week.");
        }

        ValidateTimes(
            request.StartTime,
            request.EndTime);

        var exists =
            await _timeSlotRepository
                .TimeSlotExistsAsync(
                    timeSlot.LabId,
                    day.ToString(),
                    request.StartTime,
                    request.EndTime,
                    labTimeSlotId);

        if (exists)
        {
            throw new InvalidOperationException(
                "This time slot already exists.");
        }

        timeSlot.DayOfWeek =
            day;

        timeSlot.StartTime =
            TimeOnly.FromTimeSpan(
                request.StartTime);

        timeSlot.EndTime =
            TimeOnly.FromTimeSpan(
                request.EndTime);

        timeSlot.IsActive =
            request.IsActive;

        await _timeSlotRepository.UpdateAsync(
            timeSlot);

        return true;
    }


    // =====================================================
    // DELETE / DEACTIVATE TIME SLOT
    // =====================================================
    public async Task<bool> DeleteAsync(
        int labTimeSlotId)
    {
        var timeSlot =
            await _timeSlotRepository.GetByIdAsync(
                labTimeSlotId);

        if (timeSlot == null)
        {
            return false;
        }

        if (!timeSlot.IsActive)
        {
            return true;
        }

        timeSlot.IsActive =
            false;

        await _timeSlotRepository.UpdateAsync(
            timeSlot);

        return true;
    }


    // =====================================================
    // GET AVAILABLE TIME SLOTS FOR A DATE
    //
    // BRD RULE: a lab time slot is exclusive. Once one
    // active booking exists for a lab/slot/date, that slot
    // must no longer be returned as available.
    // =====================================================
    public async Task<List<LabTimeSlotResponse>>
        GetAvailableByDateAsync(
            int labId,
            DateOnly date)
    {
        var lab =
            await _labRepository.GetByIdAsync(
                labId);

        if (lab == null)
        {
            throw new KeyNotFoundException(
                "Lab not found.");
        }

        if (!lab.IsActive)
        {
            throw new InvalidOperationException(
                "Selected lab is inactive.");
        }

        var today =
            DateOnly.FromDateTime(DateTime.UtcNow);

        if (date < today)
        {
            return new List<LabTimeSlotResponse>();
        }

        var timeSlots =
            await _timeSlotRepository.GetByLabIdAsync(
                labId);

        var matchingSlots = timeSlots
            .Where(x =>
                x.IsActive &&
                x.DayOfWeek == date.DayOfWeek)
            .ToList();

        var availableSlots =
            new List<LabTimeSlotResponse>();

        foreach (var slot in matchingSlots)
        {
            var alreadyBooked =
                await _bookingRepository.IsTimeSlotBookedAsync(
                    labId,
                    slot.LabTimeSlotId,
                    date);

            if (!alreadyBooked)
            {
                availableSlots.Add(
                    MapToResponse(slot));
            }
        }

        return availableSlots;
    }


    // =====================================================
    // VALIDATE START / END TIMES
    // =====================================================
    private static void ValidateTimes(
        TimeSpan startTime,
        TimeSpan endTime)
    {
        if (startTime < TimeSpan.Zero ||
            startTime >= TimeSpan.FromDays(1))
        {
            throw new ArgumentException(
                "Start time must be a valid time of day.");
        }

        if (endTime <= TimeSpan.Zero ||
            endTime > TimeSpan.FromDays(1))
        {
            throw new ArgumentException(
                "End time must be a valid time of day.");
        }

        if (startTime >= endTime)
        {
            throw new ArgumentException(
                "Start time must be earlier than end time.");
        }
    }


    // =====================================================
    // MAP ENTITY -> RESPONSE
    // =====================================================
    private static LabTimeSlotResponse MapToResponse(
        LabTimeSlot timeSlot)
    {
        return new LabTimeSlotResponse
        {
            LabTimeSlotId =
                timeSlot.LabTimeSlotId,

            LabId =
                timeSlot.LabId,

            DayOfWeek =
                timeSlot.DayOfWeek.ToString(),

            StartTime =
                timeSlot.StartTime.ToTimeSpan(),

            EndTime =
                timeSlot.EndTime.ToTimeSpan(),

            IsActive =
                timeSlot.IsActive
        };
    }
}