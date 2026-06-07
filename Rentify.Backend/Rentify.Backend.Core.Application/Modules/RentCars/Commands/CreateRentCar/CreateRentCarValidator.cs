using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Commands.CreateRentCar;

public class CreateRentCarValidator : AbstractValidator<CreateRentCarCommand>
{
    public CreateRentCarValidator()
    {
        RuleFor(c => c.Name).NotNull().WithMessage("Name is required").NotEmpty().WithMessage("Name is required.");
        RuleFor(c => c.Description).NotEmpty().WithMessage("Description is required.");
        RuleFor(c => c.ContactInformation).NotNull().WithMessage("Contact Information is required.");
        RuleFor(c => c.AddressInformation).NotNull().WithMessage("Address Information is required.");
        RuleFor(c => c.TenantId).NotNull().NotEmpty().WithMessage("Tenant Id is required.");
        RuleFor(x => x.ContactInformation.Email).NotEmpty().WithMessage("Email is required.").EmailAddress().WithMessage("Please enter a valid email address.");
        RuleFor(x => x.ContactInformation.PhoneNumber).NotEmpty().WithMessage("Phone Number is required.");
        RuleFor(x => x.ContactInformation.WhatsApp).NotEmpty().WithMessage("WhatsApp is required.");
        RuleFor(x => x.AddressInformation.Street).NotEmpty().WithMessage("Street is required.");
        RuleFor(x => x.AddressInformation.City).NotEmpty().WithMessage("City is required.");
        RuleFor(x => x.AddressInformation.Country).NotEmpty().WithMessage("Country is required.");
    }
}