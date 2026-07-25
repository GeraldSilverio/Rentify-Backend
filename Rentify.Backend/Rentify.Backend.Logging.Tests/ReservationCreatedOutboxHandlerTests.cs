using System.Text.Json;
using Microsoft.Extensions.Logging;
using Rentify.Backend.Core.Application.Modules.Emails.Commands.SendTemplateEmail;
using Rentify.Backend.Core.Application.Modules.Emails.Contracts.Services;
using Rentify.Backend.Core.Application.Modules.Reservations.Contracts.Repositories;
using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Reservations.Events;
using Rentify.Backend.Core.Application.Modules.Reservations.Queries.GetReservationById;
using Rentify.Backend.Core.Application.Modules.Reservations.Queries.GetReservations;
using Rentify.Backend.Core.Application.Modules.Secutiry;
using Rentify.Backend.Core.Application.Modules.Shared.Response;
using Rentify.Backend.Core.Domain.Entities.Reservations;
using Rentify.Backend.Core.Domain.Enums;
using Rentify.Backend.Infraestructure.Shared.Services.OutBox;
using Serilog;
using Serilog.Core;

namespace Rentify.Backend.Logging.Tests;

public sealed class ReservationCreatedOutboxHandlerTests
{
    [Theory]
    [InlineData(ReservationChannel.PublicWeb, EmailTemplateCodes.ReservationWeb)]
    [InlineData(ReservationChannel.Internal, EmailTemplateCodes.ReservationTenant)]
    public async Task SendsExactlyOneEmailUsingChannelTemplate(
        ReservationChannel channel,
        string expectedTemplateCode)
    {
        ReservationCreatedEmailData data = CreateData(channel);
        StubReservationRepository repository = new(data);
        CapturingEmailService emailService = new();
        ReservationCreatedOutboxHandler handler = CreateHandler(
            repository,
            emailService);

        await handler.HandleAsync(CreatePayload(data));

        SendTemplateEmailCommand command = Assert.Single(emailService.Commands);
        Assert.Equal(expectedTemplateCode, command.TemplateCode);
        Assert.Equal(data.CustomerEmail, command.To);
        Assert.Equal(data.TenantId, repository.RequestedTenantId);
        Assert.Equal(data.ReservationId, repository.RequestedReservationId);
    }

    [Fact]
    public async Task InvalidCustomerEmailIsSkippedWithoutSensitiveLogging()
    {
        ReservationCreatedEmailData data = CreateData(
            ReservationChannel.PublicWeb,
            customerEmail: "invalid-address");
        StubReservationRepository repository = new(data);
        CapturingEmailService emailService = new();
        TestLogSink sink = new();
        using Logger serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(sink)
            .CreateLogger();
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            builder.AddSerilog(serilogLogger, dispose: false));
        ReservationCreatedOutboxHandler handler = new(
            repository,
            emailService,
            loggerFactory.CreateLogger<ReservationCreatedOutboxHandler>());

        await handler.HandleAsync(CreatePayload(data));

        Assert.Empty(emailService.Commands);
        Assert.DoesNotContain(
            sink.Events,
            logEvent => logEvent.RenderMessage().Contains(
                data.CustomerEmail,
                StringComparison.Ordinal));
    }

    [Fact]
    public async Task MissingOrCrossTenantReservationIsSkipped()
    {
        ReservationCreatedEmailData data = CreateData(ReservationChannel.Internal);
        StubReservationRepository repository = new(null);
        CapturingEmailService emailService = new();
        ReservationCreatedOutboxHandler handler = CreateHandler(
            repository,
            emailService);

        await handler.HandleAsync(CreatePayload(data));

        Assert.Empty(emailService.Commands);
        Assert.Equal(data.TenantId, repository.RequestedTenantId);
        Assert.Equal(data.ReservationId, repository.RequestedReservationId);
    }

    [Fact]
    public async Task EmailFailureIsRethrownForOutboxRetry()
    {
        ReservationCreatedEmailData data = CreateData(ReservationChannel.Internal);
        StubReservationRepository repository = new(data);
        CapturingEmailService emailService = new()
        {
            ExceptionToThrow = new InvalidOperationException("Provider unavailable")
        };
        ReservationCreatedOutboxHandler handler = CreateHandler(
            repository,
            emailService);

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(CreatePayload(data)));

        Assert.Equal("Provider unavailable", exception.Message);
        Assert.Single(emailService.Commands);
    }

    [Fact]
    public async Task InvalidChannelFailsWithoutSending()
    {
        ReservationCreatedEmailData data = CreateData((ReservationChannel)999);
        StubReservationRepository repository = new(data);
        CapturingEmailService emailService = new();
        ReservationCreatedOutboxHandler handler = CreateHandler(
            repository,
            emailService);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            handler.HandleAsync(CreatePayload(data)));

        Assert.Empty(emailService.Commands);
    }

    private static ReservationCreatedOutboxHandler CreateHandler(
        IReservationRepository repository,
        IEmailService emailService)
    {
        return new ReservationCreatedOutboxHandler(
            repository,
            emailService,
            Microsoft.Extensions.Logging.Abstractions.NullLogger<ReservationCreatedOutboxHandler>.Instance);
    }

    private static string CreatePayload(ReservationCreatedEmailData data)
    {
        return JsonSerializer.Serialize(new ReservationCreatedEvent(
            data.TenantId,
            data.ReservationId,
            data.Channel));
    }

    private static ReservationCreatedEmailData CreateData(
        ReservationChannel channel,
        string customerEmail = "ana@example.test")
    {
        return new ReservationCreatedEmailData(
            Guid.NewGuid(),
            "RV-2026-0002",
            Guid.NewGuid(),
            channel,
            ReservationStatus.Pending,
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Ana",
            customerEmail,
            Guid.NewGuid(),
            "Toyota",
            "Corolla",
            2024,
            "A123456",
            "Rent Car Caribe",
            "+18095551212",
            "+18095551213",
            "contacto@rentcar.test",
            RentalType.Daily,
            2,
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(3),
            "Aeropuerto",
            "Terminal A",
            "Sucursal Centro",
            null,
            2000,
            4000,
            500,
            0,
            5000,
            0,
            9500);
    }

    private sealed class CapturingEmailService : IEmailService
    {
        public List<SendTemplateEmailCommand> Commands { get; } = [];
        public Exception? ExceptionToThrow { get; init; }

        public Task<SendTemplateEmailResponse> SendEmailAsync(
            SendTemplateEmailCommand command,
            CancellationToken cancellationToken = default)
        {
            Commands.Add(command);

            if (ExceptionToThrow is not null)
            {
                throw ExceptionToThrow;
            }

            return Task.FromResult(new SendTemplateEmailResponse(
                "Fake",
                "message-id"));
        }
    }

    private sealed class StubReservationRepository : IReservationRepository
    {
        private readonly ReservationCreatedEmailData? _data;

        public StubReservationRepository(ReservationCreatedEmailData? data)
        {
            _data = data;
        }

        public Guid RequestedTenantId { get; private set; }
        public Guid RequestedReservationId { get; private set; }

        public Task<ReservationCreatedEmailData?> GetCreatedEmailDataAsync(
            Guid tenantId,
            Guid reservationId,
            CancellationToken cancellationToken = default)
        {
            RequestedTenantId = tenantId;
            RequestedReservationId = reservationId;
            return Task.FromResult(_data);
        }

        public Task<Reservation?> GetByIdAsync(Guid tenantId, Guid reservationId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<ReservationDetailsResponse?> GetDetailsAsync(Guid tenantId, Guid reservationId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<ReservationApprovedEmailData?> GetApprovedEmailDataAsync(Guid tenantId, Guid reservationId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<bool> CodeExistsAsync(Guid tenantId, string code, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<string?> GetLastCodeAsync(Guid tenantId, int year, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<bool> HasApprovedOverlapAsync(Guid tenantId, Guid vehicleId, DateTime deliveryDateTime, DateTime expectedReturnDateTime, Guid? excludedReservationId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PaginatedResponse<ReservationListItemResponse>> GetPagedAsync(GetReservationsQuery query, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
