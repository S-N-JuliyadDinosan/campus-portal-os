using System.Text;

using CampusServicesPortal.Common.Extensions;
using CampusServicesPortal.Common.Middleware;
using CampusServicesPortal.Common.Security;
using CampusServicesPortal.Data;

using CampusServicesPortal.Modules.Identity;
using CampusServicesPortal.Modules.Students;

using CampusServicesPortal.Modules.Events.Interfaces.Repositories;
using CampusServicesPortal.Modules.Events.Interfaces.Services;
using CampusServicesPortal.Modules.Events.Repositories;
using CampusServicesPortal.Modules.Events.Services;

using CampusService.Modules.Fees.Interfaces;
using CampusService.Modules.Fees.Repositories;
using CampusService.Modules.Fees.Services;

using CampusServicesPortal.Modules.Certificates;
using CampusServicesPortal.Modules.Dashboards;
using CampusServicesPortal.Modules.Hostels;
using CampusServicesPortal.Modules.Labs;

using CampusServicesPortal.Modules.Complaints;

using CampusServicesPortal.Modules.Notifications.Interfaces.Repositories;
using CampusServicesPortal.Modules.Notifications.Interfaces.Services;
using CampusServicesPortal.Modules.Notifications.Repositories;
using CampusServicesPortal.Modules.Notifications.Services;

using CampusServicesPortal.Modules.SystemSettings.Interfaces.Repositories;
using CampusServicesPortal.Modules.SystemSettings.Interfaces.Services;
using CampusServicesPortal.Modules.SystemSettings.Repositories;
using CampusServicesPortal.Modules.SystemSettings.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CampusService;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder =
            WebApplication.CreateBuilder(args);


        // =====================================================
        // CONTROLLERS
        // =====================================================

        builder.Services.AddControllers();

        // =====================================================
        // CORS - Angular development client
        // =====================================================
        const string angularCorsPolicy = "AngularClient";
        var configuredOrigins =
            builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? Array.Empty<string>();

        var allowedOrigins = configuredOrigins.Length > 0
            ? configuredOrigins
            : new[] { "http://localhost:4200" };

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                angularCorsPolicy,
                policy => policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });


        // =====================================================
        // HTTP CONTEXT / CURRENT USER
        // =====================================================

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<
            ICurrentUserService,
            CurrentUserService>();


        // =====================================================
        // SWAGGER / OPENAPI
        // =====================================================

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title =
                        "Campus Services Portal API",

                    Version =
                        "v1"
                });


            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name =
                        "Authorization",

                    Type =
                        SecuritySchemeType.Http,

                    Scheme =
                        "bearer",

                    BearerFormat =
                        "JWT",

                    In =
                        ParameterLocation.Header,

                    Description =
                        "Paste ONLY the JWT access token. " +
                        "Do not type the word Bearer."
                });


            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference =
                                new OpenApiReference
                                {
                                    Type =
                                        ReferenceType.SecurityScheme,

                                    Id =
                                        "Bearer"
                                }
                        },

                        Array.Empty<string>()
                    }
                });
        });


        // =====================================================
        // DATABASE
        // =====================================================

        builder.Services.AddDbContext<
            ApplicationDbContext>(
            options =>
                options.UseSqlServer(
                    builder.Configuration
                        .GetConnectionString(
                            "DefaultConnection")));


        // =====================================================
        // JWT CONFIGURATION
        // =====================================================

        var jwtSection =
            builder.Configuration
                .GetSection(
                    JwtOptions.SectionName);


        builder.Services.Configure<
            JwtOptions>(
            jwtSection);


        var jwt =
            jwtSection.Get<JwtOptions>()
            ?? throw new InvalidOperationException(
                "JWT configuration is missing.");

        var requireEmailVerification =
            builder.Configuration.GetValue<bool>(
                "Identity:RequireEmailVerification");


        // =====================================================
        // VALIDATE JWT SETTINGS AT STARTUP
        // =====================================================

        if (
            string.IsNullOrWhiteSpace(
                jwt.Issuer))
        {
            throw new InvalidOperationException(
                "Jwt:Issuer is missing.");
        }


        if (
            string.IsNullOrWhiteSpace(
                jwt.Audience))
        {
            throw new InvalidOperationException(
                "Jwt:Audience is missing.");
        }


        if (
            string.IsNullOrWhiteSpace(
                jwt.Key))
        {
            throw new InvalidOperationException(
                "Jwt:Key is missing.");
        }


        if (
            Encoding.UTF8
                .GetByteCount(
                    jwt.Key) < 32)
        {
            throw new InvalidOperationException(
                "Jwt:Key must contain at least 32 bytes.");
        }


        // =====================================================
        // CREATE THE JWT SIGNING KEY
        // =====================================================

        var signingKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    jwt.Key));


        /*
         * Single local signing key used by this API.
         *
         * Giving it a KeyId also makes signing-key
         * identification explicit.
         */

        signingKey.KeyId =
            "CampusServicesPortal-HS256-Key";


        // =====================================================
        // JWT AUTHENTICATION
        // =====================================================

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults
                        .AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults
                        .AuthenticationScheme;

                options.DefaultScheme =
                    JwtBearerDefaults
                        .AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata =
                    false;

                options.SaveToken =
                    true;


                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        // -------------------------------------
                        // ISSUER
                        // -------------------------------------

                        ValidateIssuer =
                            true,

                        ValidIssuer =
                            jwt.Issuer,


                        // -------------------------------------
                        // AUDIENCE
                        // -------------------------------------

                        ValidateAudience =
                            true,

                        ValidAudience =
                            jwt.Audience,


                        // -------------------------------------
                        // SIGNATURE
                        // -------------------------------------

                        RequireSignedTokens =
                            true,

                        ValidateIssuerSigningKey =
                            true,

                        IssuerSigningKey =
                            signingKey,


                        /*
                         * Explicitly provide the local
                         * symmetric key during validation.
                         *
                         * This avoids:
                         *
                         * "The signature key was not found"
                         *
                         * when the JWT does not contain a
                         * matching kid.
                         */

                        IssuerSigningKeyResolver =
                            (
                                token,
                                securityToken,
                                kid,
                                validationParameters
                            ) =>
                            {
                                return new[]
                                {
                                    signingKey
                                };
                            },


                        TryAllIssuerSigningKeys =
                            true,


                        // -------------------------------------
                        // LIFETIME
                        // -------------------------------------

                        ValidateLifetime =
                            true,

                        RequireExpirationTime =
                            true,

                        ClockSkew =
                            TimeSpan
                                .FromSeconds(30)
                    };


                // =================================================
                // EXTRA SESSION VALIDATION
                // =================================================

                options.Events =
                    new JwtBearerEvents
                    {
                        OnTokenValidated =
                            async context =>
                            {
                                var userIdText =
                                    context.Principal?
                                        .FindFirst(
                                            System.Security.Claims
                                                .ClaimTypes
                                                .NameIdentifier)?
                                        .Value;


                                var securityStamp =
                                    context.Principal?
                                        .FindFirst(
                                            "securityStamp")?
                                        .Value;


                                if (
                                    !int.TryParse(
                                        userIdText,
                                        out var userId)
                                    ||
                                    string.IsNullOrWhiteSpace(
                                        securityStamp))
                                {
                                    context.Fail(
                                        "Invalid access token claims.");

                                    return;
                                }


                                var db =
                                    context.HttpContext
                                        .RequestServices
                                        .GetRequiredService<
                                            ApplicationDbContext>();


                                var user =
                                    await db.Users
                                        .AsNoTracking()
                                        .SingleOrDefaultAsync(
                                            x =>
                                                x.UserId ==
                                                userId);


                                if (user is null)
                                {
                                    context.Fail(
                                        "User account no longer exists.");

                                    return;
                                }


                                if (!user.IsActive)
                                {
                                    context.Fail(
                                        "This account is inactive.");

                                    return;
                                }


                                if (requireEmailVerification &&
                                    !user.EmailVerified)
                                {
                                    context.Fail(
                                        "Email verification is required.");

                                    return;
                                }


                                if (
                                    !string.Equals(
                                        user.SecurityStamp,
                                        securityStamp,
                                        StringComparison.Ordinal))
                                {
                                    context.Fail(
                                        "This session is no longer valid.");
                                }
                            }
                    };
            });


        builder.Services.AddAuthorization();


        // =====================================================
        // IDENTITY MODULE
        // =====================================================

        builder.Services.AddIdentityModule(
            builder.Configuration);


        // =====================================================
        // STUDENTS MODULE
        // =====================================================

        builder.Services.AddStudentsModule(
            builder.Configuration);


        // =====================================================
        // HOSTELS MODULE
        // =====================================================

        builder.Services.AddHostelsModule(
            builder.Configuration);


        // =====================================================
        // LABS MODULE
        // =====================================================

        builder.Services.AddLabsModule(
            builder.Configuration);


        // =====================================================
        // COMPLAINTS MODULE
        // =====================================================

        builder.Services.AddComplaintsModule(
            builder.Configuration);


        // =====================================================
        // CERTIFICATES MODULE
        // =====================================================

        builder.Services.AddCertificatesModule(
            builder.Configuration);


        // =====================================================
        // DASHBOARDS MODULE
        // =====================================================

        builder.Services.AddDashboardsModule(
            builder.Configuration);


        // =====================================================
        // EVENT REPOSITORIES
        // =====================================================

        builder.Services.AddScoped<
            IVenueRepository,
            VenueRepository>();


        builder.Services.AddScoped<
            IEventRepository,
            EventRepository>();


        builder.Services.AddScoped<
            IEventSeatRepository,
            EventSeatRepository>();


        builder.Services.AddScoped<
            IEventRegistrationRepository,
            EventRegistrationRepository>();


        // =====================================================
        // EVENT SERVICES
        // =====================================================

        builder.Services.AddScoped<
            IVenueService,
            VenueService>();


        builder.Services.AddScoped<
            IEventService,
            EventService>();


        builder.Services.AddScoped<
            IEventSeatService,
            EventSeatService>();


        builder.Services.AddScoped<
            IEventRegistrationService,
            EventRegistrationService>();


        // =====================================================
        // EVENT REGISTRATION EXPIRY BACKGROUND SERVICE
        // =====================================================

        builder.Services.AddHostedService<
            EventRegistrationExpiryBackgroundService>();


        // =====================================================
        // FEES
        // =====================================================

        builder.Services.AddScoped<
            IFeeRepository,
            FeeRepository>();


        builder.Services.AddScoped<
            IFeeService,
            FeeService>();


        // =====================================================
        // NOTIFICATIONS
        // =====================================================

        builder.Services.AddScoped<
            INotificationRepository,
            NotificationRepository>();


        builder.Services.AddScoped<
            INotificationService,
            NotificationService>();


        // =====================================================
        // SYSTEM SETTINGS
        // =====================================================

        builder.Services.AddScoped<
            ISystemSettingRepository,
            SystemSettingRepository>();


        builder.Services.AddScoped<
            ISystemSettingService,
            SystemSettingService>();


        // =====================================================
        // BUILD APP
        // =====================================================

        var app =
            builder.Build();


        // =====================================================
        // EXCEPTION HANDLING
        // =====================================================

        app.UseMiddleware<
            ExceptionHandlingMiddleware>();


        // =====================================================
        // SWAGGER
        // =====================================================

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();

            app.UseSwaggerUI();
        }


        // =====================================================
        // HTTP PIPELINE
        // =====================================================

        // The local HTTP launch profile intentionally has no HTTPS listener.
        // Production should still enforce HTTPS at the application boundary.
        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }


        app.UseCors(angularCorsPolicy);


        // =====================================================
        // AUTHENTICATION / AUTHORIZATION
        // Order is important
        // =====================================================

        app.UseAuthentication();

        app.UseAuthorization();


        // =====================================================
        // CONTROLLERS
        // =====================================================

        app.MapControllers();


        // =====================================================
        // DATABASE SEEDING / MIGRATIONS
        // =====================================================

        await app.Services
            .SeedDatabaseAsync();


        // =====================================================
        // RUN
        // =====================================================

        await app.RunAsync();
    }
}
