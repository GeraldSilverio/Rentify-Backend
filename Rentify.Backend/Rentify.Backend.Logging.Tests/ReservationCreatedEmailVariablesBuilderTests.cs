using Rentify.Backend.Core.Application.Modules.Reservations.Dtos;
using Rentify.Backend.Core.Application.Modules.Reservations.Emails;
using Rentify.Backend.Core.Application.Modules.Secutiry;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Logging.Tests;

public sealed class ReservationCreatedEmailVariablesBuilderTests
{
    [Theory]
    [InlineData(ReservationChannel.PublicWeb, EmailTemplateCodes.ReservationWeb)]
    [InlineData(ReservationChannel.Internal, EmailTemplateCodes.ReservationTenant)]
    public void ResolvesExactlyOneTemplateForSupportedChannel(
        ReservationChannel channel,
        string expectedTemplateCode)
    {
        string templateCode = ReservationCreatedEmailTemplateResolver.Resolve(channel);

        Assert.Equal(expectedTemplateCode, templateCode);
    }

    [Fact]
    public void InvalidChannelDoesNotUseFallback()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ReservationCreatedEmailTemplateResolver.Resolve((ReservationChannel)999));
    }

    [Fact]
    public void BuildsAllRequiredVariables()
    {
        IReadOnlyDictionary<string, string> variables =
            ReservationCreatedEmailVariablesBuilder.Build(CreateData());

        string[] expectedKeys =
        [
            "ReservationCode",
            "CustomerFirstName",
            "TenantName",
            "CreatedAt",
            "VehicleDescription",
            "RentalType",
            "RentalPeriod",
            "DeliveryDateTime",
            "DeliveryLocation",
            "ExpectedReturnDateTime",
            "ReturnLocation",
            "UnitRate",
            "RentalAmount",
            "DeliveryFee",
            "ReturnFee",
            "SecurityDepositDisplay",
            "DiscountAmount",
            "TotalAmount",
            "TenantPhone",
            "TenantWhatsApp",
            "TenantEmail"
        ];

        Assert.Equal(expectedKeys.Order(), variables.Keys.Order());
        Assert.Equal("RV-2026-0001", variables["ReservationCode"]);
        Assert.Equal("Ana", variables["CustomerFirstName"]);
        Assert.Equal("Rent Car Caribe", variables["TenantName"]);
        Assert.Contains("25 de julio de 2026", variables["CreatedAt"]);
        Assert.Contains("09:30", variables["CreatedAt"]);
        Assert.Equal(
            "Toyota Corolla 2024 · Placa A123456",
            variables["VehicleDescription"]);
        Assert.DoesNotContain(variables.Values, value => value.Contains("{{"));
    }

    [Theory]
    [InlineData(RentalType.Daily, "Diaria")]
    [InlineData(RentalType.Weekly, "Semanal")]
    [InlineData(RentalType.Monthly, "Mensual")]
    public void TranslatesRentalType(
        RentalType rentalType,
        string expected)
    {
        Assert.Equal(
            expected,
            ReservationCreatedEmailVariablesBuilder.TranslateRentalType(rentalType));
    }

    [Theory]
    [InlineData(1, RentalType.Daily, "1 día")]
    [InlineData(5, RentalType.Daily, "5 días")]
    [InlineData(1, RentalType.Weekly, "1 semana")]
    [InlineData(2, RentalType.Weekly, "2 semanas")]
    [InlineData(1, RentalType.Monthly, "1 mes")]
    [InlineData(2, RentalType.Monthly, "2 meses")]
    public void FormatsRentalPeriod(
        int quantity,
        RentalType rentalType,
        string expected)
    {
        Assert.Equal(
            expected,
            ReservationCreatedEmailVariablesBuilder.FormatRentalPeriod(
                quantity,
                rentalType));
    }

    [Fact]
    public void FormatsAmountsAndSecurityDepositInDominicanPesos()
    {
        IReadOnlyDictionary<string, string> variables =
            ReservationCreatedEmailVariablesBuilder.Build(CreateData());

        Assert.Equal("RD$2,000.00", variables["UnitRate"]);
        Assert.Equal("RD$6,000.00", variables["RentalAmount"]);
        Assert.Equal("RD$5,000.00", variables["SecurityDepositDisplay"]);

        IReadOnlyDictionary<string, string> withoutDeposit =
            ReservationCreatedEmailVariablesBuilder.Build(
                CreateData(securityDepositAmount: 0));

        Assert.Equal("No aplica", withoutDeposit["SecurityDepositDisplay"]);
    }

    [Fact]
    public void CombinesLocationsWithoutNullMarkers()
    {
        Assert.Equal(
            "Aeropuerto Internacional de Las Américas — Terminal A",
            ReservationCreatedEmailVariablesBuilder.FormatLocation(
                " Aeropuerto Internacional de Las Américas ",
                " Terminal A "));
        Assert.Equal(
            "Sucursal Centro",
            ReservationCreatedEmailVariablesBuilder.FormatLocation(
                "Sucursal Centro",
                null));
        Assert.Equal(
            "No especificada",
            ReservationCreatedEmailVariablesBuilder.FormatLocation(null, null));
    }

    [Fact]
    public void UsesNeutralValuesForMissingTenantContacts()
    {
        ReservationCreatedEmailData data = CreateData(
            tenantPhone: " ",
            tenantWhatsApp: "",
            tenantEmail: " ");

        IReadOnlyDictionary<string, string> variables =
            ReservationCreatedEmailVariablesBuilder.Build(data);

        Assert.Equal("No disponible", variables["TenantPhone"]);
        Assert.Equal("No disponible", variables["TenantWhatsApp"]);
        Assert.Equal("No disponible", variables["TenantEmail"]);
    }

    private static ReservationCreatedEmailData CreateData(
        decimal securityDepositAmount = 5000,
        string tenantPhone = "+18095551212",
        string tenantWhatsApp = "+18095551213",
        string tenantEmail = "contacto@rentcar.test")
    {
        return new ReservationCreatedEmailData(
            Guid.NewGuid(),
            "RV-2026-0001",
            Guid.NewGuid(),
            ReservationChannel.PublicWeb,
            ReservationStatus.Pending,
            new DateTime(2026, 7, 25, 13, 30, 0, DateTimeKind.Utc),
            Guid.NewGuid(),
            "Ana",
            "ana@example.test",
            Guid.NewGuid(),
            "Toyota",
            "Corolla",
            2024,
            "A123456",
            "Rent Car Caribe",
            tenantPhone,
            tenantWhatsApp,
            tenantEmail,
            RentalType.Daily,
            3,
            new DateTime(2026, 8, 1, 14, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 8, 4, 14, 0, 0, DateTimeKind.Utc),
            "Aeropuerto Internacional de Las Américas",
            "Terminal A",
            "Sucursal Centro",
            null,
            2000,
            6000,
            500,
            250,
            securityDepositAmount,
            100,
            11650);
    }
}
