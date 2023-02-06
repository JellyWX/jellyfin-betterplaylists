using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Playlists;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.BetterPlaylists.ScheduleTasks;

public class DynamicPlaylist : IScheduledTask, IConfigurableScheduledTask
{
    private readonly IFileSystem _fileSystem;
    private readonly ILibraryManager _libraryManager;
    private readonly ILogger _logger;
    private readonly IPlaylistManager _playlistManager;
    private readonly IProviderManager _providerManager;
    private readonly IUserManager _userManager;

    public DynamicPlaylist(
        IFileSystem fileSystem,
        ILibraryManager libraryManager,
        ILogger<Plugin> logger,
        IPlaylistManager playlistManager,
        IProviderManager providerManager,
        IServerApplicationPaths serverApplicationPaths,
        IUserManager userManager, string key)
    {
        _fileSystem = fileSystem;
        _libraryManager = libraryManager;
        _logger = logger;
        _playlistManager = playlistManager;
        _providerManager = providerManager;
        _userManager = userManager;
        Key = key;
    }

    public bool IsHidden => false;
    public bool IsEnabled => true;
    public bool IsLogged => true;
    public string Key { get; }
    public string Name => "Update Last.FM playlists";
    public string Description => "Update all Last.FM playlists with current listening stats";
    public string Category => "Library";

    public async Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
    }

    public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
    {
        return new[]
        {
            new TaskTriggerInfo
            {
                IntervalTicks = TimeSpan.FromMinutes(60).Ticks,
                Type = TaskTriggerInfo.TriggerInterval
            }
        };
    }
}