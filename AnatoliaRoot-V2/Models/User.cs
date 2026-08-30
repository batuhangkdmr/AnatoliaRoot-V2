using System.ComponentModel.DataAnnotations;
using System;

namespace AnatoliaRoot_V2.Models
{
    public class User
    {
        public const string SecurityStampClaimType = "AnatoliaRoot.SecurityStamp";

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(50)]
        public string NormalizedUsername { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public int FailedLoginAttempts { get; set; }

        public DateTime? LockoutEndUtc { get; set; }

        [Required]
        [StringLength(36)]
        public string SecurityStamp { get; set; }
    }
}
