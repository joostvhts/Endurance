using Endurance.API.Interfaces;
using Endurance.API.Models.Database;
using Endurance.API.Models.Settings;
using Microsoft.EntityFrameworkCore;

namespace Endurance.API.Services;

public class VerificationService : IVerificationService
{
    private readonly EnduranceDbContext _dbContext;
    private readonly IEmailService _emailService;
    private readonly Settings _settings;

    public VerificationService(EnduranceDbContext dbContext, IEmailService emailService, Settings settings)
    {
        _dbContext = dbContext;
        _emailService = emailService;
        _settings = settings;
    }

    public async Task<VerifiedEmailEntity> GetOrCreateVerifiedEmail(string emailAddress)
    {
        var normalizedEmail = emailAddress.Trim().ToLowerInvariant();
        var verifiedEmail = await _dbContext.VerifiedEmails
            .SingleOrDefaultAsync(x => x.Email == normalizedEmail);

        if (verifiedEmail is not null)
        {
            return verifiedEmail;
        }

        verifiedEmail = new VerifiedEmailEntity
        {
            Email = normalizedEmail,
            IsVerified = false,
            UnsubscribeToken = Guid.NewGuid().ToString("N")
        };
        
        _dbContext.VerifiedEmails.Add(verifiedEmail);
        await _dbContext.SaveChangesAsync();
        
        return verifiedEmail;
    }

    public async Task SendVerificationEmailIfNeeded(VerifiedEmailEntity verifiedEmail)
    {
        if (verifiedEmail.IsVerified)
        {
            return;
        }

        var now = DateTime.UtcNow;
        if (verifiedEmail.VerificationToken is null || verifiedEmail.VerificationExpiresAt <= now)
        {
            verifiedEmail.VerificationToken = Guid.NewGuid().ToString("N");
            verifiedEmail.VerificationExpiresAt = now.AddHours(24);
            await _dbContext.SaveChangesAsync();
        }

        var canSendEmail = verifiedEmail.LastVerificationEmailSentAt is null ||
                           verifiedEmail.LastVerificationEmailSentAt <= now.AddMinutes(-15);
        if (!canSendEmail)
        {
            return;
        }

        verifiedEmail.LastVerificationEmailSentAt = now;
        await _dbContext.SaveChangesAsync();

        var verificationUrl =
            $"{_settings.FrontendBaseUrl.TrimEnd('/')}/#action=verify-email&token={Uri.EscapeDataString(verifiedEmail.VerificationToken)}";
        try
        {
            await _emailService.SendVerificationEmail(verifiedEmail.Email, verificationUrl);
        }
        catch
        {
            verifiedEmail.LastVerificationEmailSentAt = null;
            await _dbContext.SaveChangesAsync();
            throw;
        }
    }

    public async Task<int?> VerifyEmail(string? token)
    {
        if (token is null || !Guid.TryParseExact(token, "N", out _))
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var verifiedEmail = await _dbContext.VerifiedEmails.SingleOrDefaultAsync(x =>
            x.VerificationToken == token &&
            x.VerificationExpiresAt != null &&
            x.VerificationExpiresAt > now);

        if (verifiedEmail is null)
        {
            return null;
        }

        verifiedEmail.IsVerified = true;
        verifiedEmail.VerifiedAt = now;
        verifiedEmail.VerificationToken = null;
        verifiedEmail.VerificationExpiresAt = null;
        await _dbContext.SaveChangesAsync();
        return verifiedEmail.Id;
    }

    public async Task<int?> Unsubscribe(string? token)
    {
        if (token is null || (token.Length != 32 && token.Length != 64) || token.Any(x => !Uri.IsHexDigit(x)))
        {
            return null;
        }

        var verifiedEmail = await _dbContext.VerifiedEmails
            .SingleOrDefaultAsync(x => x.UnsubscribeToken == token);

        if (verifiedEmail is null)
        {
            return null;
        }

        verifiedEmail.IsVerified = false;
        verifiedEmail.VerificationToken = null;
        verifiedEmail.VerificationExpiresAt = null;
        await _dbContext.SaveChangesAsync();
        
        return verifiedEmail.Id;
    }

    public async Task<bool> IsEmailVerified(int? verifiedEmailId)
    {
        return verifiedEmailId is not null && await _dbContext.VerifiedEmails
            .AnyAsync(x => x.Id == verifiedEmailId && x.IsVerified);
    }
}
