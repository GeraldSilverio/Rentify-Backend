using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Npgsql;
using Rentify.Backend.Core.Application.Modules.Secutiry.Contracts.Services;
using Rentify.Backend.Core.Application.Shared.Helpers;
using Rentify.Backend.Core.Application.Shared.Response;
using Rentify.Backend.Core.Domain.Settings;
using Rentify.Backend.Infraestructure.Identity.Context;
using Rentify.Backend.Infraestructure.Identity.Contracts.Services;
using Rentify.Backend.Infraestructure.Identity.Entities;
using Rentify.Backend.Infraestructure.Identity.Services;

namespace Rentify.Backend.Infraestructure.Identity
{
    public static class ServiceRegistration
    {
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
            services.AddAuthorization();

            #endregion

            #region JWToken

            services.Configure<JwtSettings>(configuration.GetSection("JWTSettings"));
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
                    ValidIssuer = configuration["JWTSettings:Issuer"],
                    ValidAudience = configuration["JWTSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]!))
                };

                options.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = c =>
                    {
                        c.NoResult();
                        c.Response.StatusCode = 500;
                        c.Response.ContentType = "text/plain";
                        return c.Response.WriteAsync(c.Exception.ToString());
                    },
                    OnChallenge = c =>
                    {
                        c.HandleResponse();
                        c.Response.StatusCode = 401;
                        c.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(ResultReponse<string>.Failure(Error.SetError("You are not authorized", StatusCodes.Status401Unauthorized)));
                        return c.Response.WriteAsync(result);
                    },
                    OnForbidden = c =>
                    {
                        c.Response.StatusCode = 403;
                        c.Response.ContentType = "application/json";
                        var result =
                            JsonConvert.SerializeObject(
                                 ResultReponse<string>.Failure(Error.SetError("You are not authorized to access this resource",StatusCodes.Status403Forbidden)));
                        return c.Response.WriteAsync(result);
                    },
                };
            });

            #endregion

            ServiceConfiguration(services);
        }

        #region "Private Methods"

        private static void ContextConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            #region IdentityContext

            string hostAddress = ReadFromConfiguration.GetValueFromConfig(configuration, "DB_HOST");
            string dataBase = ReadFromConfiguration.GetValueFromConfig(configuration, "DB_DATABASE_NAME");
            string userDb = ReadFromConfiguration.GetValueFromConfig(configuration, "DB_USER");
            string passwordDb = ReadFromConfiguration.GetValueFromConfig(configuration, "DB_PASSWORD");
            string portNumber = ReadFromConfiguration.GetValueFromConfig(configuration, "DB_PORT");

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

        #endregion
    }
}
