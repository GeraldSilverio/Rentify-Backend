using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Npgsql;
using Rentify.Backend.Core.Application.Modules.Secutiry.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Helpers;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Settings;
using Rentify.Backend.Infrastructure.Identity.Context;
using Rentify.Backend.Infrastructure.Identity.Contracts.Services;
using Rentify.Backend.Infrastructure.Identity.Entities;
using Rentify.Backend.Infrastructure.Identity.Services;

namespace Rentify.Backend.Infrastructure.Identity
{
    public static class ServiceRegistration
    {
        private const string AuthenticationFailureKey = "AuthenticationFailure";
        private const string TokenExpired = "TOKEN_EXPIRED";
        private const string InvalidToken = "INVALID_TOKEN";
        private const string MissingToken = "MISSING_TOKEN";
        private const string Forbidden = "FORBIDDEN";

        public static void AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            ContextConfiguration(services, configuration);

            #region Identity

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Login";
                options.AccessDeniedPath = "/Login/AccessDenied";
            });

            services.AddAuthentication();
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    AuthorizationPolicies.RequiredRoles,
                    policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        policy.RequireRole(
                            ApplicationRoles.Owner,
                            ApplicationRoles.Secretary);
                    });
            });

            #endregion

            #region JWToken

            JwtSettings jwtSettings = GetJwtSettings(configuration);

            services.Configure<JwtSettings>(options =>
            {
                options.Key = jwtSettings.Key;
                options.Issuer = jwtSettings.Issuer;
                options.Audience = jwtSettings.Audience;
                options.DurationInMinutes = jwtSettings.DurationInMinutes;
                options.RefreshTokenDurationInDays = jwtSettings.RefreshTokenDurationInDays;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        string failureKey = context.Exception is SecurityTokenExpiredException
                            ? TokenExpired
                            : InvalidToken;

                        context.HttpContext.Items[AuthenticationFailureKey] = failureKey;

                        ILogger logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILoggerFactory>()
                            .CreateLogger("JwtAuthentication");

                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            logger.LogInformation(
                                "El access token ha vencido; path={Path}; traceId={TraceId}",
                                context.HttpContext.Request.Path,
                                context.HttpContext.TraceIdentifier);
                        }
                        else
                        {
                            logger.LogWarning(
                                context.Exception,
                                "El access token no es válido; path={Path}; traceId={TraceId}",
                                context.HttpContext.Request.Path,
                                context.HttpContext.TraceIdentifier);
                        }

                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        if (context.Response.HasStarted)
                        {
                            return Task.CompletedTask;
                        }

                        string errorKey = ResolveAuthenticationErrorKey(context);

                        return WriteAuthenticationFailureAsync(
                            context.Response,
                            StatusCodes.Status401Unauthorized,
                            errorKey,
                            GetAuthenticationMessage(errorKey));
                    },
                    OnForbidden = context =>
                    {
                        if (context.Response.HasStarted)
                        {
                            return Task.CompletedTask;
                        }

                        return WriteAuthenticationFailureAsync(
                            context.Response,
                            StatusCodes.Status403Forbidden,
                            Forbidden,
                            "No tienes permisos para realizar esta operación.");
                    }
                };
            });

            #endregion

            ServiceConfiguration(services);
        }

        #region "Private Methods"

        private static void ContextConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            #region IdentityContext

            string hostAddress = ReadFromConfiguration.GetValueFromConfig("DB_HOST");
            string dataBase = ReadFromConfiguration.GetValueFromConfig("DB_DATABASE_NAME");
            string userDb = ReadFromConfiguration.GetValueFromConfig("DB_USER");
            string passwordDb = ReadFromConfiguration.GetValueFromConfig("DB_PASSWORD");
            string portNumber = ReadFromConfiguration.GetValueFromConfig("DB_PORT");

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = hostAddress,
                Port = int.Parse(portNumber),
                Database = dataBase,
                Username = userDb,
                Password = passwordDb
            };

            services.AddDbContext<IdentityContext>(options =>
                    options.UseNpgsql(builder.ConnectionString,
                        m => m.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName)),
                ServiceLifetime.Scoped);

            #endregion
        }

        private static void ServiceConfiguration(IServiceCollection services)
        {
            #region Services

            //services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IJwtServices, JwtServices>();
            services.AddScoped<IAccountService, AccountService>();

            services.AddScoped<IAuthenticationService, AuthenticationService>();

            #endregion
        }

        private static JwtSettings GetJwtSettings(IConfiguration configuration)
        {
            JwtSettings jwtSettings = new()
            {
                Key = ReadFromConfiguration.GetValueFromConfig("JWT_KEY"),
                Issuer = ReadFromConfiguration.GetValueFromConfig("JWT_ISSUER"),
                Audience = ReadFromConfiguration.GetValueFromConfig("JWT_AUDIENCE"),
                DurationInMinutes = int.Parse(ReadFromConfiguration.GetValueFromConfig("JWT_DURATION_IN_MINUTES")),
                RefreshTokenDurationInDays = int.Parse(ReadFromConfiguration.GetValueFromConfig("JWT_REFRESH_TOKEN_DURATION_IN_DAYS"))
            };

            ValidateJwtSettings(jwtSettings);

            return jwtSettings;
        }

        private static string ResolveAuthenticationErrorKey(JwtBearerChallengeContext context)
        {
            if (context.HttpContext.Items.TryGetValue(AuthenticationFailureKey, out object? value)
                && value is string failureKey)
            {
                return failureKey;
            }

            string? authorizationHeader = context.Request.Headers.Authorization.ToString();

            return string.IsNullOrWhiteSpace(authorizationHeader)
                || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                    ? MissingToken
                    : InvalidToken;
        }

        private static string GetAuthenticationMessage(string errorKey)
        {
            return errorKey switch
            {
                TokenExpired => "Tu sesión ha vencido.",
                InvalidToken => "El token de acceso no es válido.",
                MissingToken => "Debes iniciar sesión para acceder a este recurso.",
                _ => "Debes iniciar sesión para acceder a este recurso."
            };
        }

        private static Task WriteAuthenticationFailureAsync(
            HttpResponse response,
            int statusCode,
            string key,
            string message)
        {
            response.StatusCode = statusCode;
            response.ContentType = "application/json";

            string result = JsonConvert.SerializeObject(
                ResultReponse<string>.Failure(Error.SetError(message, statusCode, key)),
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Include
                });

            return response.WriteAsync(result);
        }

        private static void ValidateJwtSettings(JwtSettings jwtSettings)
        {
            if (string.IsNullOrWhiteSpace(jwtSettings.Key))
            {
                throw new InvalidOperationException("JWT_KEY is required.");
            }

            if (string.IsNullOrWhiteSpace(jwtSettings.Issuer))
            {
                throw new InvalidOperationException("JWT_ISSUER is required.");
            }

            if (string.IsNullOrWhiteSpace(jwtSettings.Audience))
            {
                throw new InvalidOperationException("JWT_AUDIENCE is required.");
            }

            if (jwtSettings.DurationInMinutes <= 0)
            {
                throw new InvalidOperationException("JWT_DURATION_IN_MINUTES must be greater than zero.");
            }

            if (jwtSettings.RefreshTokenDurationInDays <= 0)
            {
                throw new InvalidOperationException("JWT_REFRESH_TOKEN_DURATION_IN_DAYS must be greater than zero.");
            }

            if (TimeSpan.FromDays(jwtSettings.RefreshTokenDurationInDays)
                <= TimeSpan.FromMinutes(jwtSettings.DurationInMinutes))
            {
                throw new InvalidOperationException("JWT_REFRESH_TOKEN_DURATION_IN_DAYS must be greater than JWT_DURATION_IN_MINUTES.");
            }
        }

        #endregion
    }
}
