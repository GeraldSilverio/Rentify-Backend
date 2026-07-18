using Rentify.Backend.Core.Domain.Commons;
using Rentify.Backend.Core.Domain.Enums;

namespace Rentify.Backend.Core.Domain.Entities.Vehicles;

public sealed class Vehicle : BaseEntity
{
    private List<VehicleImage> _images = [];
    private List<VehicleRate> _rates = [];
    private List<VehicleFeatureAssignment> _featureAssignments = [];
    private List<VehicleUnavailableDate> _unavailableDates = [];

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid VehicleBrandId { get; private set; }
    public Guid VehicleModelId { get; private set; }
    public Guid VehicleTypeId { get; private set; }
    public int Year { get; private set; }
    public string PlateNumber { get; private set; } = null!;
    public string Color { get; private set; } = null!;
    public int? CurrentMileage { get; private set; }
    public bool SecurityDepositRequired { get; private set; }
    public decimal SecurityDepositAmount { get; private set; }
    public VehicleStatus Status { get; private set; }
    public VehicleBrand VehicleBrand { get; private set; } = null!;
    public VehicleModel VehicleModel { get; private set; } = null!;
    public VehicleType VehicleType { get; private set; } = null!;
    public IReadOnlyCollection<VehicleImage> Images => _images.AsReadOnly();
    public IReadOnlyCollection<VehicleRate> Rates => _rates.AsReadOnly();
    public IReadOnlyCollection<VehicleFeatureAssignment> FeatureAssignments => _featureAssignments.AsReadOnly();
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
        string color,
        int? currentMileage,
        bool securityDepositRequired,
        decimal securityDepositAmount,
        string createdBy)
    {
        Id = id;
        TenantId = tenantId;
        VehicleBrandId = vehicleBrandId;
        VehicleModelId = vehicleModelId;
        VehicleTypeId = vehicleTypeId;
        Year = year;
        PlateNumber = NormalizePlateNumber(plateNumber);
        Color = color.Trim();
        CurrentMileage = currentMileage;
        ValidateSecurityDeposit(securityDepositRequired, securityDepositAmount);
        SecurityDepositRequired = securityDepositRequired;
        SecurityDepositAmount = securityDepositRequired ? securityDepositAmount : 0;
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
        bool securityDepositRequired,
        decimal securityDepositAmount,
        string createdBy)
    {
        return new Vehicle(
            Guid.NewGuid(),
            tenantId,
            vehicleBrandId,
            vehicleModelId,
            vehicleTypeId,
            year,
            plateNumber,
            color,
            currentMileage,
            securityDepositRequired,
            securityDepositAmount,
            createdBy);
    }

    public void Update(
        Guid vehicleBrandId,
        Guid vehicleModelId,
        Guid vehicleTypeId,
        int year,
        string plateNumber,
        string color,
        int? currentMileage,
        bool securityDepositRequired,
        decimal securityDepositAmount,
        string modifiedBy)
    {
        ValidateMileageChange(currentMileage);

        VehicleBrandId = vehicleBrandId;
        VehicleModelId = vehicleModelId;
        VehicleTypeId = vehicleTypeId;
        Year = year;
        PlateNumber = NormalizePlateNumber(plateNumber);
        Color = color.Trim();
        CurrentMileage = currentMileage;
        ValidateSecurityDeposit(securityDepositRequired, securityDepositAmount);
        SecurityDepositRequired = securityDepositRequired;
        SecurityDepositAmount = securityDepositRequired ? securityDepositAmount : 0;
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

    public void ReplaceRates(
        IReadOnlyCollection<(RentalType RentalType, decimal Price)> rates,
        string modifiedBy)
    {
        if (rates.Count == 0)
            throw new ArgumentException("Vehicle must have at least one rate.");

        if (string.IsNullOrWhiteSpace(modifiedBy))
            throw new ArgumentException("ModifiedBy is required.");

        RentalType[] duplicatedRentalTypes = rates
            .GroupBy(x => x.RentalType)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();

        if (duplicatedRentalTypes.Length > 0)
            throw new ArgumentException("Vehicle rates cannot contain duplicated rental types.");

        foreach ((RentalType rentalType, decimal price) in rates)
        {
            if (!Enum.IsDefined(typeof(RentalType), rentalType))
                throw new ArgumentException("Rental type is invalid.");

            if (price <= 0)
                throw new ArgumentException("Vehicle rate price must be greater than zero.");
        }

        RentalType[] requestedRentalTypes = rates
            .Select(x => x.RentalType)
            .ToArray();

        foreach (VehicleRate rate in _rates.Where(x => !x.IsDeleted && !requestedRentalTypes.Contains(x.RentalType)))
        {
            rate.Delete(modifiedBy);
        }

        foreach ((RentalType rentalType, decimal price) in rates)
        {
            VehicleRate? existingRate = _rates.FirstOrDefault(x => !x.IsDeleted && x.RentalType == rentalType);

            if (existingRate is null)
                _rates.Add(VehicleRate.Create(TenantId, Id, rentalType, price, modifiedBy));
            else
                existingRate.Update(price, modifiedBy);
        }

        ModifiedBy = modifiedBy;
        ModifiedDate = DateTime.UtcNow;
    }

    public void ReplaceFeatures(
        IReadOnlyCollection<Guid> featureIds,
        string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
            throw new ArgumentException("ModifiedBy is required.");

        Guid[] distinctFeatureIds = featureIds
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToArray();

        if (distinctFeatureIds.Length != featureIds.Count)
            throw new ArgumentException("Vehicle features cannot contain duplicates or empty identifiers.");

        foreach (VehicleFeatureAssignment assignment in _featureAssignments
                     .Where(x => !x.IsDeleted && !distinctFeatureIds.Contains(x.VehicleFeatureId)))
        {
            assignment.Delete(modifiedBy);
        }

        foreach (Guid featureId in distinctFeatureIds)
        {
            VehicleFeatureAssignment? assignment = _featureAssignments
                .FirstOrDefault(x => x.VehicleFeatureId == featureId);

            if (assignment is null)
                _featureAssignments.Add(VehicleFeatureAssignment.Create(TenantId, Id, featureId, modifiedBy));
            else if (assignment.IsDeleted)
                assignment.Activate(modifiedBy);
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

    public void MarkAsRented(string modifiedBy)
    {
        ChangeStatus(VehicleStatus.Rented, modifiedBy);
    }

    public void MarkAsMaintenance(string modifiedBy)
    {
        ChangeStatus(VehicleStatus.Maintenance, modifiedBy);
    }

    public void MarkAsOutOfService(string modifiedBy)
    {
        ChangeStatus(VehicleStatus.OutOfService, modifiedBy);
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
        Status = VehicleStatus.OutOfService;
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

    private static void ValidateSecurityDeposit(bool securityDepositRequired, decimal securityDepositAmount)
    {
        if (securityDepositAmount < 0)
            throw new ArgumentException("Security deposit amount cannot be negative.");

        if (securityDepositRequired && securityDepositAmount <= 0)
            throw new ArgumentException("Security deposit amount must be greater than zero when required.");
    }

    private void UnmarkPrimaryImages(string modifiedBy)
    {
        foreach (VehicleImage image in _images.Where(x => x.IsPrimary && !x.IsDeleted))
        {
            image.UnmarkAsPrimary(modifiedBy);
        }
    }

    public static string NormalizePlateNumber(string plateNumber)
    {
        return plateNumber.Trim().ToUpperInvariant().Replace("-", string.Empty).Replace(" ", string.Empty);
    }
}
