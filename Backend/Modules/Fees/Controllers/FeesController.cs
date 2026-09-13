using CampusService.Modules.Fees.DTOs;
using CampusService.Modules.Fees.Interfaces;
using CampusServicesPortal.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusService.Modules.Fees.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeesController(
    IFeeService feeService,
    ICurrentUserService currentUserService)
    : ControllerBase
{
    private readonly IFeeService _feeService = feeService;

    private readonly ICurrentUserService _currentUserService =
        currentUserService;


    // ==========================================
    // FEE TYPES
    // ==========================================

    [HttpGet("types")]
    public async Task<IActionResult> GetFeeTypes()
    {
        var result =
            await _feeService.GetFeeTypesAsync();

        return Ok(result);
    }


    [HttpGet("types/{id:int}")]
    public async Task<IActionResult> GetFeeType(int id)
    {
        var result =
            await _feeService.GetFeeTypeByIdAsync(id);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Fee type not found."
            });
        }

        return Ok(result);
    }


    [HttpPost("types")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateFeeType(
        [FromBody] FeeTypeCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result =
            await _feeService.CreateFeeTypeAsync(dto);

        return CreatedAtAction(
            nameof(GetFeeType),
            new
            {
                id = result.FeeTypeId
            },
            result);
    }


    [HttpPut("types/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateFeeType(
        int id,
        [FromBody] FeeTypeUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result =
            await _feeService.UpdateFeeTypeAsync(
                id,
                dto);

        if (!result)
        {
            return NotFound(new
            {
                message = "Fee type not found."
            });
        }

        return Ok(new
        {
            message =
                "Fee type updated successfully."
        });
    }


    [HttpDelete("types/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteFeeType(
        int id)
    {
        var result =
            await _feeService.DeleteFeeTypeAsync(id);

        if (!result)
        {
            return NotFound(new
            {
                message = "Fee type not found."
            });
        }

        return Ok(new
        {
            message =
                "Fee type deleted successfully."
        });
    }


    // ==========================================
    // FEE PAYMENTS
    // ==========================================

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllFees()
    {
        var result =
            await _feeService.GetAllFeesAsync();

        return Ok(result);
    }


    [HttpGet("{id:int}")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> GetFee(
        int id)
    {
        var result =
            await _feeService.GetFeeByIdAsync(id);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Fee payment not found."
            });
        }

        // Student can only see own fee.
        if (User.IsInRole("Student"))
        {
            if (!_currentUserService.StudentId.HasValue)
            {
                return Unauthorized(new
                {
                    message =
                        "Student account information is missing."
                });
            }

            if (result.StudentId !=
                _currentUserService.StudentId.Value)
            {
                return Forbid();
            }
        }

        return Ok(result);
    }


    // ==========================================
    // CURRENT STUDENT FEES
    // ==========================================

    [HttpGet("me")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyFees()
    {
        if (!_currentUserService.StudentId.HasValue)
        {
            return Unauthorized(new
            {
                message =
                    "Student account information is missing."
            });
        }

        var result =
            await _feeService.StudentFeesAsync(
                _currentUserService.StudentId.Value);

        return Ok(result);
    }


    [HttpGet("me/unpaid")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyUnpaidFees()
    {
        if (!_currentUserService.StudentId.HasValue)
        {
            return Unauthorized(new
            {
                message =
                    "Student account information is missing."
            });
        }

        var result =
            await _feeService.GetUnpaidFeesAsync(
                _currentUserService.StudentId.Value);

        return Ok(result);
    }


    [HttpGet("me/paid")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyPaidFees()
    {
        if (!_currentUserService.StudentId.HasValue)
        {
            return Unauthorized(new
            {
                message =
                    "Student account information is missing."
            });
        }

        var result =
            await _feeService.GetPaidFeesAsync(
                _currentUserService.StudentId.Value);

        return Ok(result);
    }


    // ==========================================
    // STUDENT FEES BY ID
    // Student = own data only
    // Admin = any student
    // ==========================================

    [HttpGet("student/{studentId:int}")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> GetStudentFees(
        int studentId)
    {
        if (!CanAccessStudent(studentId))
        {
            return Forbid();
        }

        var result =
            await _feeService.StudentFeesAsync(
                studentId);

        return Ok(result);
    }


    [HttpGet("student/{studentId:int}/unpaid")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> GetUnpaidFees(
        int studentId)
    {
        if (!CanAccessStudent(studentId))
        {
            return Forbid();
        }

        var result =
            await _feeService.GetUnpaidFeesAsync(
                studentId);

        return Ok(result);
    }


    [HttpGet("student/{studentId:int}/paid")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<IActionResult> GetPaidFees(
        int studentId)
    {
        if (!CanAccessStudent(studentId))
        {
            return Forbid();
        }

        var result =
            await _feeService.GetPaidFeesAsync(
                studentId);

        return Ok(result);
    }


    // ==========================================
    // ASSIGN FEE
    // ==========================================

    [HttpPost("assign")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignFee(
        [FromBody] FeeAssignDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (dto.Amount <= 0)
        {
            return BadRequest(new
            {
                message =
                    "Amount must be greater than zero."
            });
        }

        // FIX:
        // TokenService does not use a "sub" claim here.
        // CurrentUserService correctly reads the UserId.
        var assignedByUserId =
            _currentUserService.UserId;

        var result =
            await _feeService.AssignFeeAsync(
                dto,
                assignedByUserId);

        if (!result)
        {
            return BadRequest(new
            {
                message =
                    "Provide either StudentId or FacultyId, not both."
            });
        }

        return Ok(new
        {
            message =
                "Fee assigned successfully."
        });
    }


    // ==========================================
    // FAKE / DEMO PAYMENT
    // ==========================================

    [HttpPost("{id:int}/simulate-payment")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> SimulatePayment(
        int id)
    {
        if (!_currentUserService.StudentId.HasValue)
        {
            return Unauthorized(new
            {
                message =
                    "Student account information is missing."
            });
        }

        var payment =
            await _feeService.SimulatePaymentAsync(
                id,
                _currentUserService.StudentId.Value);

        if (payment == null)
        {
            return NotFound(new
            {
                message =
                    "Fee payment not found."
            });
        }

        return Ok(new
        {
            message =
                "Demo payment completed successfully.",

            feePaymentId =
                payment.FeePaymentId,

            status =
                payment.Status.ToString(),

            paidAt =
                payment.PaidAt,

            paymentMethod =
                payment.PaymentMethod,

            paymentReference =
                payment.PaymentReference,

            receiptNumber =
                payment.ReceiptNumber
        });
    }


    // ==========================================
    // PAYMENT STATUS
    // ==========================================

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] FeePaymentUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result =
            await _feeService.UpdatePaymentStatusAsync(
                id,
                dto);

        if (!result)
        {
            return BadRequest(new
            {
                message =
                    "Invalid fee or status."
            });
        }

        return Ok(new
        {
            message =
                "Fee payment status updated successfully."
        });
    }


    // ==========================================
    // DELETE FEE
    // ==========================================

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteFee(
        int id)
    {
        var result =
            await _feeService.DeleteFeeAsync(id);

        if (!result)
        {
            return NotFound(new
            {
                message =
                    "Fee payment not found."
            });
        }

        return Ok(new
        {
            message =
                "Fee payment deleted successfully."
        });
    }


    // ==========================================
    // PRIVATE SECURITY HELPER
    // ==========================================

    private bool CanAccessStudent(
        int studentId)
    {
        if (User.IsInRole("Admin"))
        {
            return true;
        }

        if (!_currentUserService.StudentId.HasValue)
        {
            return false;
        }

        return _currentUserService.StudentId.Value
            == studentId;
    }
}