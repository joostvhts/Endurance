using Endurance.API.Models;

namespace Endurance.API.Interfaces;

public interface IEmailService
{
    Task SendEmail(WatchedClassModel watchedClassModel, ClassInfoModel classInfoModel);
    Task SendVerificationEmail(string emailAddress, string verificationUrl);
}
