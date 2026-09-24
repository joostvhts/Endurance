using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Endurance.API.Models.Database;

[Table("verified_emails")]
public class VerifiedEmailEntity
{
    [Key]
    public int Id { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("is_verified")]
    public bool IsVerified { get; set; }

    [Column("verification_token")]
    public string? VerificationToken { get; set; }

    [Column("verification_expires_at")]
    public DateTime? VerificationExpiresAt { get; set; }

    [Column("last_verification_email_sent_at")]
    public DateTime? LastVerificationEmailSentAt { get; set; }

    [Column("unsubscribe_token")]
    public string UnsubscribeToken { get; set; }

    [Column("verified_at")]
    public DateTime? VerifiedAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
