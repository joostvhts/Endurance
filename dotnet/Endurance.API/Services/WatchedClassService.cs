using System.Text.Json;
using Endurance.API.Enums;
using Endurance.API.Interfaces;
using Endurance.API.Interfaces.Repositories;
using Endurance.API.Models;
using Endurance.API.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Endurance.API.Services;

public class WatchedClassService : IWatchedClassService
{
    private readonly IWatchedClassRepository _watchedClassRepository;
    private readonly EnduranceDbContext _dbContext;
    private const int MaxWatchersPerEmail = 5;

    public WatchedClassService(IWatchedClassRepository watchedClassRepository, EnduranceDbContext dbContext)
    {
        _watchedClassRepository = watchedClassRepository;
        _dbContext = dbContext;
    }

    public async Task AddEmailWatcher(string venueId, string classId, DateTime startDateTime,
        VerifiedEmailEntity verifiedEmail)
    {
        var now = DateTime.UtcNow;
        var watcherCount = await _dbContext.WatchedClasses.CountAsync(x =>
            x.VerifiedEmailId == verifiedEmail.Id &&
            x.StartDateTime > now &&
            (x.IsActive || x.IsPendingVerification));

        if (watcherCount >= MaxWatchersPerEmail)
        {
            throw new InvalidOperationException("You can watch at most 5 classes at the same time.");
        }

        var watchedClass = new WatchedClassEntity
        {
            VenueId = venueId,
            ClassId = classId,
            IsActive = verifiedEmail.IsVerified,
            IsPendingVerification = !verifiedEmail.IsVerified,
            StartDateTime = startDateTime,
            VerifiedEmail = verifiedEmail,
            NotifierSettings = new NotifierSettingsEntity
            {
                Type = NotifierType.Email,
                Settings = JsonSerializer.Serialize(new EmailNotifierSettings
                {
                    Email = verifiedEmail.Email
                })
            }
        };

        await _watchedClassRepository.AddWatchedClass(watchedClass);
    }

    public async Task DisableWatcher(int id)
    {
        await _watchedClassRepository.DisableWatchedClass(id);
    }

    public async Task<List<WatchedClassModel>> GetAllWatchedClassesAsync()
    {
        var response = await _watchedClassRepository.GetAllAsync(true);

        return response.Select(x => new WatchedClassModel
        {
            VenueId = x.VenueId,
            ClassId = x.ClassId,
            IsActive = x.IsActive,
            StartDateTime = x.StartDateTime,
            VerifiedEmailId = x.VerifiedEmailId,
            UnsubscribeToken = x.VerifiedEmail?.UnsubscribeToken,
            NotifierSettings = new NotifierSettingsEntity
            {
                Id = x.NotifierSettingsId,
                Settings = x.NotifierSettings.Settings,
                Type = x.NotifierSettings.Type,
                CreatedAt = x.NotifierSettings.CreatedAt
            },
            CreatedAt = x.CreatedAt,
            Id = x.Id
        }).ToList();
    }

    public async Task ActivatePendingWatchers(int verifiedEmailId)
    {
        var now = DateTime.UtcNow;
        var pendingWatchers = await _dbContext.WatchedClasses
            .Where(x => x.VerifiedEmailId == verifiedEmailId && x.IsPendingVerification)
            .ToListAsync();

        foreach (var watcher in pendingWatchers)
        {
            watcher.IsPendingVerification = false;
            watcher.IsActive = watcher.StartDateTime > now;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task DisableAllWatchers(int verifiedEmailId)
    {
        var watchers = await _dbContext.WatchedClasses
            .Where(x => x.VerifiedEmailId == verifiedEmailId && (x.IsActive || x.IsPendingVerification))
            .ToListAsync();

        foreach (var watcher in watchers)
        {
            watcher.IsActive = false;
            watcher.IsPendingVerification = false;
        }

        await _dbContext.SaveChangesAsync();
    }

}
