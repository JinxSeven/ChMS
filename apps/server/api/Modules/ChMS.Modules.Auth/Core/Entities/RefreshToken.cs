using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ChMS.Modules.Auth.Core.Entities
{
    [Index(nameof(HashedToken), IsUnique = true)]
    [Index(nameof(UserId), nameof(RevokedAt), nameof(ExpiresAt), Name = "IX_Active_User_RefreshTokens")] // Indexing for faster user token lookup
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        [Required]
        public string HashedToken { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiresAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public Guid? ReplacedByTokenId { get; set; }

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }

        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        [NotMapped]
        public bool IsActive => RevokedAt == null && !IsExpired;
    }
}
