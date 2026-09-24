using Endurance.API.Models;
using Endurance.API.Models.Database;

namespace Endurance.API.Interfaces;

public interface IWatchedClassService
{
    Task AddEmailWatcher(string venueId, string classId, DateTime startDateTime, VerifiedEmailEntity verifiedEmail);
    Task DisableWatcher(int id);
    Task ActivatePendingWatchers(int verifiedEmailId);
    Task DisableAllWatchers(int verifiedEmailId);
    Task<List<WatchedClassModel>> GetAllWatchedClassesAsync();
}
