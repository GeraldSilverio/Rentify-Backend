namespace Rentify.Backend.Core.Application.Modules.Secutiry.Dtos.Response
{
    public sealed record UserResponse(Guid UserId,
        Guid TenantId,
        string RentCarCompanyName,
        string UserName,
        string Email,
        string Fullname,
        List<string> Roles);
}
