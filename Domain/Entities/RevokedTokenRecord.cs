using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TokenRevocationService.Domain.Entities;

[Table("revoked_tokens")]
public class RevokedTokenRecord
{
    [Key]
    [Column("token")]
    [MaxLength(512)]
    public string Token { get; set; } = string.Empty;

    [Column("revoked_at_utc")]
    public DateTime RevokedAtUtc { get; set; } = DateTime.UtcNow;

    [Column("expires_at_utc")]
    public DateTime ExpiresAtUtc { get; set; }

    [Column("reason")]
    [MaxLength(256)]
    public string? Reason { get; set; }
}
