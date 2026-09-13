using CampusServicesPortal.Modules.Hostels.DTOs;
using CampusServicesPortal.Modules.Hostels.Entities;
using CampusServicesPortal.Modules.Hostels.Interfaces;

namespace CampusServicesPortal.Modules.Hostels.Services;

public sealed class HostelService : IHostelService
{
    private readonly IHostelRepository _hostelRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IHostelApplicationRepository _applicationRepository;

    public HostelService(
        IHostelRepository hostelRepository,
        IRoomRepository roomRepository,
        IHostelApplicationRepository applicationRepository)
    {
        _hostelRepository = hostelRepository;
        _roomRepository = roomRepository;
        _applicationRepository = applicationRepository;
    }

    public async Task<List<HostelResponse>> GetAllAsync()
    {
        var hostels = await _hostelRepository.GetAllAsync();

        return hostels
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<HostelResponse?> GetByIdAsync(int hostelId)
    {
        var hostel =
            await _hostelRepository.GetByIdAsync(hostelId);

        return hostel == null
            ? null
            : MapToResponse(hostel);
    }

    public async Task<HostelResponse> CreateAsync(
        CreateHostelRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException(
                "Hostel name is required.");
        }

        var name = request.Name.Trim();

        var existing =
            await _hostelRepository.GetByNameAsync(name);

        if (existing?.IsActive == true)
        {
            throw new InvalidOperationException(
                "A hostel with this name already exists.");
        }

        // Recover records left behind by the previous soft-delete behavior.
        if (existing != null)
        {
            existing.Location = request.Location?.Trim();
            existing.IsActive = true;

            await _hostelRepository.UpdateAsync(existing);

            return MapToResponse(existing);
        }

        var hostel = new Hostel
        {
            Name = name,
            Location = request.Location?.Trim(),
            IsActive = true
        };

        hostel =
            await _hostelRepository.AddAsync(hostel);

        return MapToResponse(hostel);
    }

    public async Task<bool> UpdateAsync(
        int hostelId,
        UpdateHostelRequest request)
    {
        var hostel =
            await _hostelRepository.GetByIdAsync(hostelId);

        if (hostel == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException(
                "Hostel name is required.");
        }

        var name = request.Name.Trim();

        var exists =
            await _hostelRepository.NameExistsAsync(
                name,
                hostelId);

        if (exists)
        {
            throw new InvalidOperationException(
                "A hostel with this name already exists.");
        }

        hostel.Name = name;
        hostel.Location = request.Location?.Trim();
        hostel.IsActive = request.IsActive;

        await _hostelRepository.UpdateAsync(hostel);

        return true;
    }

    public async Task<bool> DeleteAsync(int hostelId)
    {
        var hostel =
            await _hostelRepository.GetByIdAsync(hostelId);

        if (hostel == null)
        {
            return false;
        }

        await _hostelRepository.DeleteAsync(hostel);

        return true;
    }

    public async Task<HostelAvailabilityResponse?> GetAvailabilityAsync(
        int hostelId,
        string academicYear,
        string semester)
    {
        var hostel =
            await _hostelRepository.GetByIdAsync(hostelId);

        if (hostel == null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(academicYear))
        {
            throw new ArgumentException(
                "Academic year is required.");
        }

        if (string.IsNullOrWhiteSpace(semester))
        {
            throw new ArgumentException(
                "Semester is required.");
        }

        academicYear = academicYear.Trim();
        semester = semester.Trim();

        var totalCapacity =
            await _roomRepository
                .GetTotalActiveCapacityByHostelAsync(
                    hostelId);

        var occupied =
            await _applicationRepository
                .GetAssignedCountByHostelAsync(
                    hostelId,
                    academicYear,
                    semester);

        return new HostelAvailabilityResponse
        {
            HostelId = hostelId,
            AcademicYear = academicYear,
            Semester = semester,
            TotalCapacity = totalCapacity,
            Occupied = occupied,
            Available = Math.Max(
                totalCapacity - occupied,
                0)
        };
    }

    private static HostelResponse MapToResponse(
        Hostel hostel)
    {
        return new HostelResponse
        {
            HostelId = hostel.HostelId,
            Name = hostel.Name,
            Location = hostel.Location,
            IsActive = hostel.IsActive,
            CreatedAt = hostel.CreatedAt
        };
    }
}
