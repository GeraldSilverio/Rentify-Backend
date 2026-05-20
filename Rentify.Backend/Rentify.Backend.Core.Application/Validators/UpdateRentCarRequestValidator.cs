using FluentValidation;
using Rentify.Backend.Core.Application.Dtos.RentCars;

namespace Rentify.Backend.Core.Application.Validators;

public class UpdateRentCarRequestValidator : AbstractValidator<UpdateRentCarRequest>
{
    public UpdateRentCarRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required.");

        RuleFor(x => x.WhatsApp)
            .MaximumLength(20).WithMessage("WhatsApp must not exceed 20 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.WhatsApp));

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.");
    }
}
