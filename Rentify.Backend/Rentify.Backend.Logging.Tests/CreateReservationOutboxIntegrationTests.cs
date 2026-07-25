using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Rentify.Backend.Core.Application.Modules.Customers.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Customers.Dtos;
using Rentify.Backend.Core.Application.Modules.Customers.Queries.SearchCustomers;
using Rentify.Backend.Core.Application.Modules.Reservations.Commands.CreateReservation;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Reservations.Emails;
using Rentify.Backend.Core.Application.Modules.Reservations.Events;
using Rentify.Backend.Core.Application.Modules.Shared.Constants;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Application.Modules.Shared.UnitOfWork;
using Rentify.Backend.Core.Domain.Entities.Customers;
using Rentify.Backend.Core.Domain.Entities.Events;
using Rentify.Backend.Core.Domain.Entities.Reservations;
using Rentify.Backend.Core.Domain.Entities.Vehicles;
using Rentify.Backend.Core.Domain.Enums;
using Rentify.Backend.Infraestructure.Persistence.Context;
using Rentify.Backend.Infraestructure.Persistence.Repositories;
using Rentify.Backend.Infraestructure.Shared.Services;
using Rentify.Backend.Infrastructure.Persistence.Repositories;

namespace Rentify.Backend.Logging.Tests;

public sealed class CreateReservationOutboxIntegrationTests
{
    [Theory]
    [InlineData(ReservationChannel.PublicWeb, "RESERVATION_WEB")]
    [InlineData(ReservationChannel.Internal, "RESERVATION_TENANT")]
    public async Task PersistsReservationAndOneCreatedEventInSameSave(
        ReservationChannel channel,
        string expectedTemplateCode)
    {
        await using RentifyContext context = CreateContext();
        Guid tenantId = Guid.NewGuid();
        Guid customerId = Guid.NewGuid();
        Vehicle vehicle = CreateVehicle(tenantId);
        ReservationRepository reservationRepository = new(context);
        UnitOfWork unitOfWork = new(
            context,
            NullLogger<UnitOfWork>.Instance);
        CreateReservationCommandHandler handler = new(
            reservationRepository,
            new StubVehicleResolver(vehicle),
            new StubLocationResolver(),
            new StubCodeGenerator(),
            unitOfWork,
            new StubCustomerRepository(customerId),
            new OutboxService(context),
            NullLogger<CreateReservationCommandHandler>.Instance);

        CreateReservationCommand command = CreateCommand(
            tenantId,
            customerId,
            vehicle.Id,
            channel);

        ResultReponse<ReservationResponse> result =
            await handler.Handle(command, CancellationToken.None);

        Reservation reservation =
            Assert.Single(await context.Reservations.AsNoTracking().ToListAsync());
        OutboxMessage outboxMessage =
            Assert.Single(await context.OutboxMessages.AsNoTracking().ToListAsync());
        ReservationCreatedEvent createdEvent = JsonSerializer.Deserialize<ReservationCreatedEvent>(
            outboxMessage.Payload,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        Assert.Equal(reservation.Id, result.Value!.Id);
        Assert.Equal(OutboxMessageTypes.ReservationCreated, outboxMessage.Type);
        Assert.Equal(tenantId, outboxMessage.TenantId);
        Assert.Equal(reservation.Id, createdEvent.ReservationId);
        Assert.Equal(channel, createdEvent.Channel);
        Assert.Equal(
            expectedTemplateCode,
            ReservationCreatedEmailTemplateResolver.Resolve(
                createdEvent.Channel));
    }

    [Fact]
    public async Task FailedSavePersistsNeitherReservationNorCreatedEvent()
    {
        await using RentifyContext context = CreateContext();
        Guid tenantId = Guid.NewGuid();
        Guid customerId = Guid.NewGuid();
        Vehicle vehicle = CreateVehicle(tenantId);
        CreateReservationCommandHandler handler = new(
            new ReservationRepository(context),
            new StubVehicleResolver(vehicle),
            new StubLocationResolver(),
            new StubCodeGenerator(),
            new ThrowingUnitOfWork(),
            new StubCustomerRepository(customerId),
            new OutboxService(context),
            NullLogger<CreateReservationCommandHandler>.Instance);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(
                CreateCommand(
                    tenantId,
                    customerId,
                    vehicle.Id,
                    ReservationChannel.PublicWeb),
                CancellationToken.None));

        Assert.Equal(0, await context.Reservations.AsNoTracking().CountAsync());
        Assert.Equal(0, await context.OutboxMessages.AsNoTracking().CountAsync());
    }

    private static RentifyContext CreateContext()
    {
        DbContextOptions<RentifyContext> options =
            new DbContextOptionsBuilder<RentifyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        return new RentifyContext(options);
    }

    private static Vehicle CreateVehicle(Guid tenantId)
    {
        return Vehicle.Create(
            tenantId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            2024,
            "A123456",
            null,
            "Blanco",
            10000,
            false,
            0,
            "tests");
    }

    private static CreateReservationCommand CreateCommand(
        Guid tenantId,
        Guid customerId,
        Guid vehicleId,
        ReservationChannel channel)
    {
        DateTime delivery = new(2026, 8, 1, 14, 0, 0, DateTimeKind.Utc);

        return new CreateReservationCommand(
            tenantId,
            customerId,
            vehicleId,
            delivery,
            delivery.AddDays(2),
            RentalType.Daily,
            null,
            "Aeropuerto",
            "Terminal A",
            500,
            null,
            "Sucursal Centro",
            null,
            0,
            100,
            channel,
            null,
            "tests");
    }

    private sealed class StubVehicleResolver : IReservationVehicleResolver
    {
        private readonly Vehicle _vehicle;
        private readonly VehicleRate _rate;

        public StubVehicleResolver(Vehicle vehicle)
        {
            _vehicle = vehicle;
            _rate = VehicleRate.Create(
                vehicle.TenantId,
                vehicle.Id,
                RentalType.Daily,
                2000,
                "tests");
        }

        public Task<Vehicle> GetReservableVehicleAsync(
            Guid tenantId,
            Guid vehicleId,
            CancellationToken cancellationToken)
        {
            Assert.Equal(_vehicle.TenantId, tenantId);
            Assert.Equal(_vehicle.Id, vehicleId);
            return Task.FromResult(_vehicle);
        }

        public VehicleRate GetRateOrThrow(Vehicle vehicle, RentalType rentalType)
        {
            Assert.Equal(RentalType.Daily, rentalType);
            return _rate;
        }
    }

    private sealed class StubLocationResolver : IReservationLocationResolver
    {
        public Task<ResolvedReservationLocation> ResolveDeliveryAsync(
            Guid tenantId,
            Guid? tenantLocationId,
            string? customName,
            decimal customFee,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new ResolvedReservationLocation(
                tenantLocationId,
                customName!,
                customFee));
        }

        public Task<ResolvedReservationLocation> ResolveReturnAsync(
            Guid tenantId,
            Guid? tenantLocationId,
            string? customName,
            decimal customFee,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new ResolvedReservationLocation(
                tenantLocationId,
                customName!,
                customFee));
        }
    }

    private sealed class StubCodeGenerator : IReservationCodeGenerator
    {
        public Task<string> GenerateAsync(
            Guid tenantId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult("RV-2026-0001");
        }
    }

    private sealed class ThrowingUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Synthetic save failure");
        }
    }

    private sealed class StubCustomerRepository : ICustomerRepository
    {
        private readonly Guid _customerId;

        public StubCustomerRepository(Guid customerId)
        {
            _customerId = customerId;
        }

        public Task<bool> ExistCustomerByIdAsync(Guid tenantId, Guid customerId, CancellationToken cancellationToken = default) => Task.FromResult(customerId == _customerId);
        public Task AddAsync(Customer customer, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task AddDocumentAsync(CustomerDocument document, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Customer?> GetByIdAsync(Guid tenantId, Guid customerId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<CustomerDocument?> GetDocumentByIdAsync(Guid tenantId, Guid customerId, Guid documentId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<bool> IdentificationExistsAsync(Guid tenantId, IdentificationType identificationType, string identificationNumberNormalized, Guid? excludedCustomerId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<CustomerDetailsResponse?> GetDetailsAsync(Guid tenantId, Guid customerId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PaginatedResponse<CustomerResponse>> SearchAsync(SearchCustomersQuery query, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<bool> EmailExistsAsync(Guid tenantId, string email, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<bool> EmailExistsAsync(Guid tenantId, string email, Guid? excludedCustomerId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
