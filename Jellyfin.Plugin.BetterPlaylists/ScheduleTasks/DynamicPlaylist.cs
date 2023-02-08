using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.BetterPlaylists.LastFm;
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
    private readonly ILibraryManager _libraryManager;
    private readonly ILogger _logger;
    private readonly IPlaylistManager _playlistManager;
    private readonly IProviderManager _providerManager;
    private readonly IUserManager _userManager;
    private readonly IBetterPlaylistFileSystem _betterPlaylistFileSystem;
    private readonly LastFmApi _lastFm;

    public DynamicPlaylist(
        IFileSystem fileSystem,
        ILibraryManager libraryManager,
        ILogger<Plugin> logger,
        IPlaylistManager playlistManager,
        IProviderManager providerManager,
        IServerApplicationPaths serverApplicationPaths,
        IHttpClientFactory httpClientFactory,
        IUserManager userManager)
    {
        _libraryManager = libraryManager;
        _logger = logger;
        _playlistManager = playlistManager;
        _providerManager = providerManager;
        _userManager = userManager;

        _betterPlaylistFileSystem = new BetterPlaylistFileSystem(serverApplicationPaths);
        _lastFm = new LastFmApi(httpClientFactory, logger);
    }

    public bool IsHidden => false;
    public bool IsEnabled => true;
    public bool IsLogged => true;
    public string Key => nameof(DynamicPlaylist);
    public string Name => "Update Last.FM playlists";
    public string Description => "Update all Last.FM playlists with current listening stats";
    public string Category => "Library";

    public async Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
        foreach (var user in _userManager.Users)
        {
            var playlists = _playlistManager.GetPlaylists(user.Id);

            foreach (var playlist in playlists)
            {
                var filePath = _betterPlaylistFileSystem.GetBetterPlaylistFilePath(playlist.FileNameWithoutExtension);

                if (filePath == null) continue;

                await using var reader = File.OpenRead(filePath);
                var betterPlaylist = await JsonSerializer.DeserializeAsync<BetterPlaylist>(
                    reader,
                    new JsonSerializerOptions(),
                    cancellationToken).ConfigureAwait(false);
                reader.Close();

                // Skip any playlist files tagged as different playlist types.
                if (betterPlaylist.Type != "dynamic") continue;

                var playlistItems = playlist.GetManageableItems().ToArray().Select(item => item.Item2.Id).ToList();

                _logger.Log(LogLevel.Information, $"Current playlist contents: {string.Join(", ", playlistItems)}");

                // Fetch item details from LastFM
                var audioQueries = await _lastFm.GetTopTracks("jude-s", cancellationToken);

                var resolvedItems = audioQueries.AudioQueries(_logger)
                    .Select(audioQuery => audioQuery.Resolve(_logger, _libraryManager, _providerManager))
                    .Where(q => q != null)
                    .Select(q => q.Id)
                    .ToList();

                _logger.Log(LogLevel.Information, $"Adding {resolvedItems.Count} items to {playlist.Name}");

                await _playlistManager.RemoveFromPlaylistAsync(playlist.Id.ToString(),
                    playlistItems.Select(i => i.ToString()));
                await _playlistManager.AddToPlaylistAsync(playlist.Id, resolvedItems, user.Id);
            }
        }
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