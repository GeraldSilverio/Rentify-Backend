using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rentify.Backend.Core.Application;
using Rentify.Backend.Core.Application.Modules.Secutiry;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Vehicles;
using Rentify.Backend.Infraestructure.Identity;
using Rentify.Backend.Infraestructure.Identity.Entities;
using Rentify.Backend.Infraestructure.Identity.Seeds;
using Rentify.Backend.Infraestructure.Persistence;
using Rentify.Backend.Infraestructure.Shared;
using Rentify.Backend.Presentation.WebApi.Endpoints.Admin.Vehicles;
using Rentify.Backend.Presentation.WebApi.Endpoints.Customers;
using Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;
using Rentify.Backend.Presentation.WebApi.Endpoints.Vehicles;
using Rentify.Backend.Presentation.WebApi.Extensions;
using Rentify.Backend.Presentation.WebApi.Services;
using Rentify.Backend.Shared;
using Rentify.Backend.Shared.Configuration;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
EnvFileLoader.LoadFromNearest(builder.Environment.ContentRootPath);

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
})
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressInferBindingSourcesForParameters = true;
    options.SuppressMapClientErrors = true;
});
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentRequestContext, CurrentRequestContext>();
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddSharedServices();
builder.Services.AddApplicationLayer();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddSwaggerExtension();
builder.Services.AddApiVersioningExtension();

builder.Services.AddAntiforgery();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("PublicCatalogPolicy", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    await DefaultRoles.CreateRoles(roleManager);
    await DefaultUser.CreateUser(userManager);
}

app.UseErrorHandlingMiddleware();

app.UseHttpsRedirection();

app.UseCors(a => a.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAntiforgery();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");
app.UseRentifyHangfireJobs();

app.UseHealthChecks("/health");
app.UseSession();

#region Endpoints

#region Vehicles
app.MapVehicleCatalogEndpoints();
app.MapVehiclesEndpoints();
#endregion
app.MapVehicleFeatureAssignmentEndpoints();
app.MapCustomersEndpoints();
app.MapCustomerDocumentsEndpoints();
app.MapAdminTenantEndpoints();
app.MapAdminVehicleCatalogEndpoints();
app.MapRegisterTenant();
app.MapAuthEndpoints();
app.MapSubscriptionEndpoints();
app.MapTenantEndpoints();
#endregion

app.Run();

