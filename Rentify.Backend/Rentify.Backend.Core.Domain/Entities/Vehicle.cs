using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Domain.Entities;

public sealed class Vehicle : BaseEntity
{
    private readonly List<VehicleImage> _images = [];
    private readonly List<VehicleUnavailableDate> _unavailableDates = [];

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid RentCarId { get; private set; }
    public Guid VehicleModelId { get; private set; }
    public Guid VehicleTypeId { get; private set; }
    public int Year { get; private set; }
    public string PlateNumber { get; private set; } = null!;
    public string Vin { get; private set; } = null!;
    public string Color { get; private set; } = null!;
    public decimal DailyRate { get; private set; }
    public VehicleStatus Status { get; private set; }

    public RentCar RentCar { get; private set; } = null!;
    public VehicleModel VehicleModel { get; private set; } = null!;
    public VehicleType VehicleType { get; private set; } = null!;
    public IReadOnlyCollection<VehicleImage> Images => _images.AsReadOnly();
    public IReadOnlyCollection<VehicleUnavailableDate> UnavailableDates => _unavailableDates.AsReadOnly();

    private Vehicle()
    {
    }

    private Vehicle(
        Guid id,
        Guid tenantId,
        Guid rentCarId,
        Guid vehicleModelId,
        Guid vehicleTypeId,
        int year,
        string plateNumber,
        string vin,
        string color,
        decimal dailyRate,
        string createdBy)
    {
        Id = id;
        TenantId = tenantId;
        RentCarId = rentCarId;
        VehicleModelId = vehicleModelId;
        VehicleTypeId = vehicleTypeId;
        Year = year;
        PlateNumber = NormalizePlateNumber(plateNumber);
        Vin = vin.Trim().ToUpperInvariant();
        Color = color.Trim();
        DailyRate = dailyRate;
        Status = VehicleStatus.Available;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
    }

    public static Vehicle Create(
        Guid tenantId,
        Guid rentCarId,
        Guid vehicleModelId,
        Guid vehicleTypeId,
        int year,
        string plateNumber,
        string vin,
        string color,
        decimal dailyRate,
        string createdBy)
    {
        ValidateIds(tenantId, rentCarId, vehicleModelId, vehicleTypeId);
        Validate(year, plateNumber, vin, color, dailyRate);

        return new Vehicle(
            Guid.NewGuid(),
            tenantId,
            rentCarId,
            vehicleModelId,
            vehicleTypeId,
            year,
            plateNumber,
            vin,
            color,
            dailyRate,
            createdBy);
    }

    public void Update(
        Guid vehicleModelId,
        Guid vehicleTypeId,
        int year,
        string plateNumber,
        string vin,
        string color,
        decimal dailyRate,
        string modifiedBy)
    {
        if (vehicleModelId == Guid.Empty)
            throw new ArgumentException("Vehicle model Id is required.");

        if (vehicleTypeId == Guid.Empty)
            throw new ArgumentException("Vehicle type Id is required.");

        Validate(year, plateNumber, vin, color, dailyRate);

        VehicleModelId = vehicleModelId;
        VehicleTypeId = vehicleTypeId;
        Year = year;
        PlateNumber = NormalizePlateNumber(plateNumber);
        Vin = vin.Trim().ToUpperInvariant();
        Color = color.Trim();
        DailyRate = dailyRate;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public VehicleImage AddImage(string url, string publicId, bool isPrimary, string createdBy)
    {
        bool shouldBePrimary = isPrimary || !_images.Any(x => !x.IsDeleted);

        if (shouldBePrimary)
            UnmarkPrimaryImages(createdBy);

        VehicleImage image = VehicleImage.Create(TenantId, Id, url, publicId, shouldBePrimary, createdBy);
        _images.Add(image);

        ModifiedBy = createdBy;
        ModifiedDate = DateTime.UtcNow;

        return image;
    }

    public void SetPrimaryImage(Guid imageId, string modifiedBy)
    {
        VehicleImage image = _images.FirstOrDefault(x => x.Id == imageId && !x.IsDeleted)
                             ?? throw new ArgumentException("Vehicle image not found.");

        UnmarkPrimaryImages(modifiedBy);
        image.MarkAsPrimary(modifiedBy);
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void ChangeStatus(VehicleStatus status, string modifiedBy)
    {
        Status = status;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void Delete(string modifiedBy)
    {
        IsDeleted = true;
        IsActive = false;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void AddUnavailableDate(DateOnly startDate, DateOnly endDate, string? reason, string createdBy)
    {
        if (_unavailableDates.Any(x => !x.IsDeleted && x.Overlaps(startDate, endDate)))
            throw new ArgumentException("Vehicle already has an unavailable range that overlaps these dates.");

        _unavailableDates.Add(VehicleUnavailableDate.Create(TenantId, Id, startDate, endDate, reason, createdBy));
        Status = VehicleStatus.Unavailable;
        ModifiedBy = createdBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public bool IsAvailableFor(DateOnly startDate, DateOnly endDate)
    {
        return Status == VehicleStatus.Available
               && !_unavailableDates.Any(x => !x.IsDeleted && x.Overlaps(startDate, endDate));
    }

    private void UnmarkPrimaryImages(string modifiedBy)
    {
        foreach (VehicleImage image in _images.Where(x => x.IsPrimary && !x.IsDeleted))
        {
            image.UnmarkAsPrimary(modifiedBy);
        }
    }

    private static void ValidateIds(
        Guid tenantId,
        Guid rentCarId,
        Guid vehicleModelId,
        Guid vehicleTypeId)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant Id is required.");

        if (rentCarId == Guid.Empty)
            throw new ArgumentException("Rent car Id is required.");

        if (vehicleModelId == Guid.Empty)
            throw new ArgumentException("Vehicle model Id is required.");

        if (vehicleTypeId == Guid.Empty)
            throw new ArgumentException("Vehicle type Id is required.");
    }

    private static void Validate(
        int year,
        string plateNumber,
        string vin,
        string color,
        decimal dailyRate)
    {
        if (year < 1980 || year > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("Vehicle year is invalid.");

        if (string.IsNullOrWhiteSpace(plateNumber))
            throw new ArgumentException("Plate number is required.");

        if (string.IsNullOrWhiteSpace(vin))
            throw new ArgumentException("VIN is required.");

        if (vin.Trim().Length != 17)
            throw new ArgumentException("VIN must contain 17 characters.");

        if (string.IsNullOrWhiteSpace(color))
            throw new ArgumentException("Vehicle color is required.");

        if (color.Length > 50)
            throw new ArgumentException("Vehicle color is too long.");

        if (dailyRate <= 0)
            throw new ArgumentException("Daily rate must be greater than zero.");
    }

    private static string NormalizePlateNumber(string plateNumber)
    {
        return plateNumber.Trim().ToUpperInvariant().Replace("-", string.Empty).Replace(" ", string.Empty);
    }
}
