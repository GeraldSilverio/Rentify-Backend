using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.RentCars.Commands.UpdateRentCar;

public sealed class UpdateRentCarValidator : AbstractValidator<UpdateRentCarCommand>
{
    public UpdateRentCarValidator()
    {
        RuleFor(c => c.Id).NotEmpty().WithMessage("Rent car Id is required.");
        RuleFor(c => c.Name).NotEmpty().WithMessage("Name is required.").MaximumLength(100);
        RuleFor(c => c.Description).NotEmpty().WithMessage("Description is required.").MaximumLength(500);
        RuleFor(c => c.ModifiedBy).NotEmpty().WithMessage("Modified by is required.");
        RuleFor(c => c.ContactInformation).NotNull().WithMessage("Contact Information is required.");
        RuleFor(c => c.AddressInformation).NotNull().WithMessage("Address Information is required.");

        When(x => x.ContactInformation is not null, () =>
        {
            RuleFor(x => x.ContactInformation.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Please enter a valid email address.");

            RuleFor(x => x.ContactInformation.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone Number is required.");

            RuleFor(x => x.ContactInformation.WhatsApp)
                .NotEmpty()
                .WithMessage("WhatsApp is required.");
        });

        When(x => x.AddressInformation is not null, () =>
        {
            RuleFor(x => x.AddressInformation.Street)
                .NotEmpty()
                .WithMessage("Street is required.");

            RuleFor(x => x.AddressInformation.City)
                .NotEmpty()
                .WithMessage("City is required.");

            RuleFor(x => x.AddressInformation.Country)
                .NotEmpty()
                .WithMessage("Country is required.");
        });

        RuleFor(x => x.LogoUrl)
            .MaximumLength(500)
            .Must(BeAValidAbsoluteUrl)
            .When(x => !string.IsNullOrWhiteSpace(x.LogoUrl))
            .WithMessage("Logo URL must be a valid absolute URL.");
    }

    private static bool BeAValidAbsoluteUrl(string? logoUrl)
    {
        return Uri.TryCreate(logoUrl, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
