namespace Rentify.Backend.Core.Domain.Entities;

public class RentCar : BaseEntity
{
    public Guid RentCarId { get; set; }

    // Información básica
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Contacto
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? WhatsApp { get; set; }

    // Ubicación
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    // Dueño del negocio
    public Guid OwnerId { get; set; }

}