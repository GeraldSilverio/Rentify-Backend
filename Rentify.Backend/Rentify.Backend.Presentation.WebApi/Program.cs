using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rentify.Backend.Core.Application;
using Rentify.Backend.Core.Application.Modules.Customers;
using Rentify.Backend.Core.Application.Modules.Dashboard;
using Rentify.Backend.Core.Application.Modules.Emails;
using Rentify.Backend.Core.Application.Modules.Payments;
using Rentify.Backend.Core.Application.Modules.Reservations;
using Rentify.Backend.Core.Application.Modules.Secutiry;
using Rentify.Backend.Core.Application.Modules.Shared.Context;
using Rentify.Backend.Core.Application.Modules.Vehicles;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.BlockVehicleAvailability;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleStatus;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ChangeVehicleFeatureStatus;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicleFeature;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.CreateVehicle;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicle;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.DeleteVehicleImage;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ManageVehicleCatalog;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.ReplaceVehicleFeatures;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.SetPrimaryVehicleImage;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicle;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UpdateVehicleFeature;
using Rentify.Backend.Core.Application.Modules.Vehicles.Commands.UploadVehicleImage;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetAssignedVehicleFeatures;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleImages;
using Rentify.Backend.Core.Application.Modules.Vehicles.Queries.GetVehicleFeatures;
using Rentify.Backend.Infraestructure.Identity;
using Rentify.Backend.Infraestructure.Identity.Entities;
using Rentify.Backend.Infraestructure.Identity.Seeds;
using Rentify.Backend.Infraestructure.Persistence;
using Rentify.Backend.Infraestructure.Shared;
using Rentify.Backend.Presentation.Endpoints;
using Rentify.Backend.Presentation.WebApi.Endpoints.Tenants;
using Rentify.Backend.Presentation.WebApi.Extensions;
using Rentify.Backend.Presentation.WebApi.Services;
using Rentify.Backend.Shared;
using Rentify.Backend.Shared.Configuration;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
EnvFileLoader.LoadFromNearest(builder.Environment.ContentRootPath);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
builder.Services.AddScoped<ICurrentTenantService, CurrentTenantService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
//Dependecias de las capas.
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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    await DefaultRoles.CreateRoles(roleManager);
    await DefaultUser.CreateUser(userManager);
}

app.MapRegisterTenant();
app.MapAuthEndpoints();
app.MapSubscriptionEndpoints();
app.MapTenantEndpoints();
app.MapAdminTenantEndpoints();

var securedEndpoints = app.MapGroup(string.Empty)
    .RequireAuthorization();

securedEndpoints.MapCreateVehicleEndpoints();
securedEndpoints.MapUpdateVehicleEndpoints();
securedEndpoints.MapDeleteVehicleEndpoints();
securedEndpoints.MapUploadVehicleImageEndpoints();
securedEndpoints.MapGetVehicleImagesEndpoints();
securedEndpoints.MapSetPrimaryVehicleImageEndpoints();
securedEndpoints.MapDeleteVehicleImageEndpoints();
securedEndpoints.MapChangeVehicleStatusEndpoints();
securedEndpoints.MapBlockVehicleAvailabilityEndpoints();
securedEndpoints.MapVehicleCatalogEndpoints();
securedEndpoints.MapManageVehicleCatalogEndpoints();
securedEndpoints.MapGetVehicleFeaturesEndpoints();
securedEndpoints.MapCreateVehicleFeatureEndpoint();
securedEndpoints.MapUpdateVehicleFeatureEndpoint();
securedEndpoints.MapChangeVehicleFeatureStatusEndpoints();
securedEndpoints.MapGetAssignedVehicleFeaturesEndpoint();
securedEndpoints.MapReplaceVehicleFeaturesEndpoint();
securedEndpoints.MapCustomerEndpoints();
securedEndpoints.MapReservationEndpoints();
securedEndpoints.MapPaymentEndpoints();
securedEndpoints.MapDashboardEndpoints();
securedEndpoints.MapUserEndpoints();
securedEndpoints.MapEmailEndpoints();

app.UseCors(a => a.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHangfireDashboard("/hangfire");
app.UseRentifyHangfireJobs();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwaggerExtension();
app.UseErrorHandlingMiddleware();

app.UseHealthChecks("/health");
app.UseSession();
app.Run();

