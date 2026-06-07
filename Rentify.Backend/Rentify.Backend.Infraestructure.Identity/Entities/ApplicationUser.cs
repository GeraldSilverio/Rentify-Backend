using Microsoft.AspNetCore.Identity;

namespace Rentify.Backend.Infraestructure.Identity.Entities
{
    /// <summary>
    /// Class representing users in the system, inherits from IdentityUser.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// User's first name.
        /// </summary>
        public string? FullName { get; set; }

        public Guid TenantId { get; set; }

        /// <summary>
        /// Person who created the record.
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Person who modified the record.
        /// </summary>
        public string? ModifiedBy { get; set; }

        /// <summary>
        /// Date when the record was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Date when the record was last modified.
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Indicates whether the record is active or not, for implementing SoftDelete.
        /// </summary>
        public bool IsActive { get; set; }
    }
}