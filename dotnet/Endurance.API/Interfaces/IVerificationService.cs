using Endurance.API.Models.Database;

namespace Endurance.API.Interfaces;

public interface IVerificationService
{
    Task<VerifiedEmailEntity> GetOrCreateVerifiedEmail(string emailAddress);
    Task SendVerificationEmailIfNeeded(VerifiedEmailEntity verifiedEmail);
    Task<int?> VerifyEmail(string? token);
    Task<int?> Unsubscribe(string? token);
    Task<bool> IsEmailVerified(int? verifiedEmailId);
}
