using Endurance.API.Enums;
using Endurance.API.Interfaces;
using Endurance.API.Models;

namespace Endurance.API.Services;

public class NotifyService : INotifyService
{
    private readonly IEmailService _emailService;
    private readonly IWatchedClassService _watchedClassService;
    private readonly IVerificationService _verificationService;
    
    public NotifyService(IEmailService emailService, IWatchedClassService watchedClassService,
        IVerificationService verificationService)
    {
        _emailService = emailService;
        _watchedClassService = watchedClassService;
        _verificationService = verificationService;
    }
    
    public async Task Notify(WatchedClassModel watchedClassModel, ClassInfoModel classInfoModel)
    {
        if (watchedClassModel.NotifierSettings.Type == NotifierType.Email)
        {
            if (!await _verificationService.IsEmailVerified(watchedClassModel.VerifiedEmailId))
            {
                await _watchedClassService.DisableWatcher(watchedClassModel.Id);
                return;
            }

            await _emailService.SendEmail(watchedClassModel, classInfoModel);
        }

        await _watchedClassService.DisableWatcher(watchedClassModel.Id);
    }
}
