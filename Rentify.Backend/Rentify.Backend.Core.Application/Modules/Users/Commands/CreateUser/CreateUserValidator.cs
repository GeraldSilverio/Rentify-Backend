using FluentValidation;

namespace Rentify.Backend.Core.Application.Modules.Users.Commands.CreateUser
{
    public class CreateUserValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.FullName)
                .NotNull().WithMessage("FullName is required")
                .NotEmpty().WithMessage("FullName is required");

            RuleFor(x => x.Email)
                .NotNull().NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Please enter a valid email address.");

            RuleFor(x => x.UserName).NotNull().NotEmpty().WithMessage("UserName is required");

            RuleFor(x => x.PhoneNumber).NotNull().NotEmpty().WithMessage("PhoneNumber is required");

            RuleFor(x => x.TenantId).NotNull().NotEmpty().WithMessage("TenantId is required");

            RuleFor(x => x.CreatedBy).NotNull().NotEmpty().WithMessage("CreatedBy is required");

            RuleFor(x => x.Role).NotNull().NotEmpty().WithMessage("Role is required");

            RuleFor(x => x.Password)
                .NotNull().NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must have 8 characters")
                .Matches("[A-Z]").WithMessage("Password must have 1 capital letter")
                .Matches("[a-z]").WithMessage("Password must have 1 minuscule letter")
                .Matches("[0-9]").WithMessage("Password must have 1 number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must have 1 special character");
        }
    }
}
