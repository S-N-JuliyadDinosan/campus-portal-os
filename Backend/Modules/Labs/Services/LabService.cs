using CampusServicesPortal.Common.Enums;
using CampusServicesPortal.Modules.Labs.DTOs;
using CampusServicesPortal.Modules.Labs.Entities;
using CampusServicesPortal.Modules.Labs.Interfaces;

namespace CampusServicesPortal.Modules.Labs.Services;

public sealed class LabService : ILabService
{
    private readonly ILabRepository _labRepository;

    public LabService(ILabRepository labRepository)
    {
        _labRepository = labRepository;
    }

    public async Task<List<LabResponse>> GetAllAsync()
    {
        var labs = await _labRepository.GetAllAsync();

        return labs
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<LabResponse?> GetByIdAsync(int labId)
    {
        var lab = await _labRepository.GetByIdAsync(labId);

        return lab == null
            ? null
            : MapToResponse(lab);
    }

    public async Task<LabResponse> CreateAsync(
        CreateLabRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            throw new ArgumentException(
                "Lab code is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException(
                "Lab name is required.");
        }

        if (request.Capacity <= 0)
        {
            throw new ArgumentException(
                "Lab capacity must be greater than zero.");
        }

        if (!Enum.TryParse<LabType>(
                request.LabType,
                true,
                out var labType))
        {
            throw new ArgumentException(
                "Invalid lab type.");
        }

        var code = request.Code.Trim();

        var exists =
            await _labRepository.CodeExistsAsync(code);

        if (exists)
        {
            throw new InvalidOperationException(
                "A lab with this code already exists.");
        }

        var lab = new Lab
        {
            Code = code,
            Name = request.Name.Trim(),
            LabType = labType,
            Capacity = request.Capacity,
            IsActive = true
        };

        lab = await _labRepository.AddAsync(lab);

        return MapToResponse(lab);
    }

    public async Task<bool> UpdateAsync(
        int labId,
        UpdateLabRequest request)
    {
        var lab =
            await _labRepository.GetByIdAsync(labId);

        if (lab == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            throw new ArgumentException(
                "Lab code is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException(
                "Lab name is required.");
        }

        if (request.Capacity <= 0)
        {
            throw new ArgumentException(
                "Lab capacity must be greater than zero.");
        }

        if (!Enum.TryParse<LabType>(
                request.LabType,
                true,
                out var labType))
        {
            throw new ArgumentException(
                "Invalid lab type.");
        }

        var code = request.Code.Trim();

        var exists =
            await _labRepository.CodeExistsAsync(
                code,
                labId);

        if (exists)
        {
            throw new InvalidOperationException(
                "A lab with this code already exists.");
        }

        lab.Code = code;
        lab.Name = request.Name.Trim();
        lab.LabType = labType;
        lab.Capacity = request.Capacity;
        lab.IsActive = request.IsActive;

        await _labRepository.UpdateAsync(lab);

        return true;
    }

    public async Task<bool> DeleteAsync(int labId)
    {
        var lab =
            await _labRepository.GetByIdAsync(labId);

        if (lab == null)
        {
            return false;
        }

        await _labRepository.DeleteAsync(lab);

        return true;
    }

    private static LabResponse MapToResponse(Lab lab)
    {
        return new LabResponse
        {
            LabId = lab.LabId,
            Code = lab.Code,
            Name = lab.Name,
            LabType = lab.LabType.ToString(),
            Capacity = lab.Capacity,
            IsActive = lab.IsActive,
            CreatedAt = lab.CreatedAt
        };
    }
}
