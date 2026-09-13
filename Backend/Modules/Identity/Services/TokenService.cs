using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CampusServicesPortal.Common.Security;
using CampusServicesPortal.Modules.Identity.Entities;
using CampusServicesPortal.Modules.Identity.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CampusServicesPortal.Modules.Identity.Services;

public sealed class TokenService : ITokenService
{
    private const string SigningKeyId =
        "CampusServicesPortal-HS256-Key";

    private readonly JwtOptions _options;

    public TokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }


    // =========================================================
    // ACCESS TOKEN
    // =========================================================
    public string CreateAccessToken(
        User user,
        int? studentId)
    {
        var claims = new List<Claim>
        {
            new(
                ClaimTypes.NameIdentifier,
                user.UserId.ToString()),

            new(
                "userId",
                user.UserId.ToString()),

            new(
                ClaimTypes.Email,
                user.Email),

            new(
                ClaimTypes.Role,
                user.Role),

            new(
                "securityStamp",
                user.SecurityStamp)
        };


        if (studentId.HasValue)
        {
            claims.Add(
                new Claim(
                    "studentId",
                    studentId.Value.ToString()));
        }


        // =====================================================
        // SAME KEY + SAME KEY ID AS Program.cs
        // =====================================================
        var securityKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _options.Key))
            {
                KeyId = SigningKeyId
            };


        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);


        var token =
            new JwtSecurityToken(
                issuer:
                    _options.Issuer,

                audience:
                    _options.Audience,

                claims:
                    claims,

                notBefore:
                    DateTime.UtcNow,

                expires:
                    DateTime.UtcNow.AddMinutes(
                        _options.AccessTokenMinutes),

                signingCredentials:
                    credentials);


        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }


    // =========================================================
    // REFRESH TOKEN
    // =========================================================
    public string CreateRefreshToken()
    {
        return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));
    }


    // =========================================================
    // 6 DIGIT EMAIL VERIFICATION OTP
    // =========================================================
    public string CreateOneTimeToken()
    {
        var otp =
            RandomNumberGenerator.GetInt32(
                0,
                1_000_000);

        return otp.ToString("D6");
    }


    // =========================================================
    // HASH TOKEN
    // =========================================================
    public string HashToken(
        string rawToken)
    {
        if (string.IsNullOrWhiteSpace(
            rawToken))
        {
            throw new ArgumentException(
                "Token cannot be empty.",
                nameof(rawToken));
        }


        var bytes =
            SHA256.HashData(
                Encoding.UTF8.GetBytes(
                    rawToken.Trim()));


        return Convert.ToHexString(
            bytes);
    }
}