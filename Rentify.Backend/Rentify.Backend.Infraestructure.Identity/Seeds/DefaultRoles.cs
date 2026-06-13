using Microsoft.AspNetCore.Identity;
using Rentify.Backend.Core.Application.Shared.Security;

namespace Rentify.Backend.Infraestructure.Identity.Seeds
{
    /// <summary>
    /// Class responsible for creating default roles in the application.
    /// </summary>
    public class DefaultRoles
    {
        /// <summary>
        /// Creates default roles asynchronously.
        /// </summary>
        /// <param name="roleManager">The role manager instance.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task CreateRoles(RoleManager<IdentityRole> roleManager)
        {
            await CreateRoleIfNotExistsAsync(roleManager, ApplicationRoles.User);
            await CreateRoleIfNotExistsAsync(roleManager, ApplicationRoles.Owner);
        }

        private static async Task CreateRoleIfNotExistsAsync(
            RoleManager<IdentityRole> roleManager,
            string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
