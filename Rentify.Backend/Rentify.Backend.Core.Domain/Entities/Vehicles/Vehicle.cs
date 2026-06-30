using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Domain.Entities.Vehicles;

public sealed class Vehicle : BaseEntity
{
    private readonly List<VehicleImage> _images = [];
    private readonly List<VehicleUnavailableDate> _unavailableDates = [];

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid VehicleBrandId { get; private set; }
    public Guid VehicleModelId { get; private set; }
    public Guid VehicleTypeId { get; private set; }
    public int Year { get; private set; }
    public string PlateNumber { get; private set; } = null!;
    public string? Vin { get; private set; }
    public string Color { get; private set; } = null!;
    public int? CurrentMileage { get; private set; }
    public VehicleStatus Status { get; private set; }

    // Temporary compatibility for modules that still depend on the old pricing field.
    public decimal DailyRate => 0m;

    public VehicleBrand VehicleBrand { get; private set; } = null!;
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
        Guid vehicleBrandId,
        Guid vehicleModelId,
        Guid vehicleTypeId,
        int year,
        string plateNumber,
        string? vin,
        string color,
        int? currentMileage,
        string createdBy)
    {
        Id = id;
        TenantId = tenantId;
        VehicleBrandId = vehicleBrandId;
        VehicleModelId = vehicleModelId;
        VehicleTypeId = vehicleTypeId;
        Year = year;
        PlateNumber = NormalizePlateNumber(plateNumber);
        Vin = NormalizeVinOrNull(vin);
        Color = color.Trim();
        CurrentMileage = currentMileage;
        Status = VehicleStatus.Available;
        CreatedBy = createdBy;
        ModifiedBy = createdBy;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = CreatedDate;
        IsActive = true;
        IsDeleted = false;
    }

    public static Vehicle Create(
        Guid tenantId,
        Guid vehicleBrandId,
        Guid vehicleModelId,
        Guid vehicleTypeId,
        int year,
        string plateNumber,
        string? vin,
        string color,
        int? currentMileage,
        string createdBy)
    {
        ValidateIds(tenantId, vehicleBrandId, vehicleModelId, vehicleTypeId);
        Validate(year, plateNumber, vin, color, currentMileage, createdBy);

        return new Vehicle(
            Guid.NewGuid(),
            tenantId,
            vehicleBrandId,
            vehicleModelId,
            vehicleTypeId,
            year,
            plateNumber,
            vin,
            color,
            currentMileage,
            createdBy);
    }

    public void Update(
        Guid vehicleBrandId,
        Guid vehicleModelId,
        Guid vehicleTypeId,
        int year,
        string plateNumber,
        string? vin,
        string color,
        int? currentMileage,
        string modifiedBy)
    {
        ValidateIds(TenantId, vehicleBrandId, vehicleModelId, vehicleTypeId);
        Validate(year, plateNumber, vin, color, currentMileage, modifiedBy);
        ValidateMileageChange(currentMileage);

        VehicleBrandId = vehicleBrandId;
        VehicleModelId = vehicleModelId;
        VehicleTypeId = vehicleTypeId;
        Year = year;
        PlateNumber = NormalizePlateNumber(plateNumber);
        Vin = NormalizeVinOrNull(vin);
        Color = color.Trim();
        CurrentMileage = currentMileage;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void UpdateMileage(int? currentMileage, string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
            throw new ArgumentException("ModifiedBy is required.");

        ValidateMileageChange(currentMileage);
        CurrentMileage = currentMileage;
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

    public void RemoveImage(Guid imageId, string modifiedBy)
    {
        VehicleImage image = _images.FirstOrDefault(x => x.Id == imageId && !x.IsDeleted)
                             ?? throw new ArgumentException("Vehicle image not found.");

        bool wasPrimary = image.IsPrimary;
        image.Delete(modifiedBy);

        if (wasPrimary)
        {
            VehicleImage? nextPrimary = _images
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.CreatedDate)
                .FirstOrDefault();

            nextPrimary?.MarkAsPrimary(modifiedBy);
        }

        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void ChangeStatus(VehicleStatus status, string modifiedBy)
    {
        if (!Enum.IsDefined(typeof(VehicleStatus), status))
            throw new ArgumentException("Vehicle status is invalid.");

        Status = status;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void MarkAsAvailable(string modifiedBy)
    {
        ChangeStatus(VehicleStatus.Available, modifiedBy);
    }

    public void MarkAsReserved(string modifiedBy)
    {
        ChangeStatus(VehicleStatus.Reserved, modifiedBy);
    }

    public void MarkAsMaintenance(string modifiedBy)
    {
        ChangeStatus(VehicleStatus.Maintenance, modifiedBy);
    }

    public void Activate(string modifiedBy)
    {
        IsActive = true;
        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void Deactivate(string modifiedBy)
    {
        IsActive = false;
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

    private void ValidateMileageChange(int? currentMileage)
    {
        if (CurrentMileage.HasValue && !currentMileage.HasValue)
            throw new ArgumentException("No se puede eliminar el kilometraje actual del vehículo.");

        if (CurrentMileage.HasValue && currentMileage.HasValue && currentMileage.Value < CurrentMileage.Value)
            throw new ArgumentException("El kilometraje no puede ser menor que el kilometraje actual del vehículo.");
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
        Guid vehicleBrandId,
        Guid vehicleModelId,
        Guid vehicleTypeId)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant Id is required.");

        if (vehicleBrandId == Guid.Empty)
            throw new ArgumentException("Vehicle brand Id is required.");

        if (vehicleModelId == Guid.Empty)
            throw new ArgumentException("El modelo del vehículo es requerido.");

        if (vehicleTypeId == Guid.Empty)
            throw new ArgumentException("Vehicle type Id is required.");
    }

    private static void Validate(
        int year,
        string plateNumber,
        string? vin,
        string color,
        int? currentMileage,
        string user)
    {
        if (year < 1980 || year > DateTime.UtcNow.Year + 1)
            throw new ArgumentException("Vehicle year is invalid.");

        if (string.IsNullOrWhiteSpace(plateNumber))
            throw new ArgumentException("La placa es requerida.");

        if (plateNumber.Trim().Length > 20)
            throw new ArgumentException("Vehicle plate number is too long.");

        if (!string.IsNullOrWhiteSpace(vin) && vin.Trim().Length > 50)
            throw new ArgumentException("VIN is too long.");

        if (string.IsNullOrWhiteSpace(color))
            throw new ArgumentException("Vehicle color is required.");

        if (color.Trim().Length > 50)
            throw new ArgumentException("Vehicle color is too long.");

        if (currentMileage < 0)
            throw new ArgumentException("Vehicle mileage cannot be negative.");

        if (string.IsNullOrWhiteSpace(user))
            throw new ArgumentException("User is required.");
    }

    private static string NormalizePlateNumber(string plateNumber)
    {
        return plateNumber.Trim().ToUpperInvariant().Replace("-", string.Empty).Replace(" ", string.Empty);
    }

    private static string? NormalizeVinOrNull(string? vin)
    {
        return string.IsNullOrWhiteSpace(vin)
            ? null
            : vin.Trim().ToUpperInvariant();
    }
}
