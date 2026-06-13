using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rentify.Backend.Core.Application;
using Rentify.Backend.Core.Application.Modules.Emails;
using Rentify.Backend.Core.Application.Modules.RentCars.Commands.CreateRentCar;
using Rentify.Backend.Core.Application.Modules.Secutiry;
using Rentify.Backend.Core.Application.Modules.Subscriptions;
using Rentify.Backend.Core.Application.Modules.Tenants.Commands.RegisterTenant;
using Rentify.Backend.Infraestructure.Identity;
using Rentify.Backend.Infraestructure.Identity.Entities;
using Rentify.Backend.Infraestructure.Identity.Seeds;
using Rentify.Backend.Infraestructure.Persistence;
using Rentify.Backend.Presentation.Endpoints;
using Rentify.Backend.Presentation.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));
}).ConfigureApiBehaviorOptions(options =>
{
    options.SuppressInferBindingSourcesForParameters = true;
    options.SuppressMapClientErrors = true;
});
//Dependecias de las capas.
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
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
app.MapCreateRentCarEndpoints();
app.MapUserEndpoints();
app.MapAuthEndpoints();
app.MapEmailEndpoints();
app.MapSubscriptionEndpoints();

app.UseCors(a => a.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwaggerExtension();
app.UseErrorHandlingMiddleware();

app.UseHealthChecks("/health");
app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

