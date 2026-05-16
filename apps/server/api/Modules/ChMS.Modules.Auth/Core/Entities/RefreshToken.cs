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

        /*
         * VIRTUAL PROP

         * virtual enables Lazy Loading in Entity Framework.
         * When virtual is present, EF Core can create a proxy class (a wrapper) around your entity.
         * This proxy allows EF to automatically load related data (like User) the first time you access refreshToken.User
  
         * Lazy Loading is disabled by default in modern EF Core projects (for performance reasons, and same goes to our project).
         * Use .include() this tells Entity Framework Core to load related data (navigation properties) together with the main entity in a single query.
         */

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
        public bool IsRevoked => RevokedAt == null && !IsExpired;
    }
}
