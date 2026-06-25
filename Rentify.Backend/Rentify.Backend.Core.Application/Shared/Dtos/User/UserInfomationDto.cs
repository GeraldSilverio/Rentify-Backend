using Rentify.Backend.Core.Application.Shared.Dtos.Information;

namespace Rentify.Backend.Core.Application.Shared.Dtos.User
{
    public sealed record UserInfomationDto(
        string FullName,
        string UserName,
        string Email,
        string Password,
        ContactInformationDto ContactInformation);
}
