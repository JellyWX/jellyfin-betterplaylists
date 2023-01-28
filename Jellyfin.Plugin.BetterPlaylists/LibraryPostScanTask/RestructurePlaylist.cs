using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Playlists;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.BetterPlaylists.LibraryPostScanTask;

public class RestructurePlaylist : ILibraryPostScanTask
{
    private readonly ILibraryManager _libraryManager;
    private readonly IPlaylistManager _playlistManager;
    private readonly IUserManager _userManager;
    private readonly IProviderManager _providerManager;
    private readonly ILogger<Plugin> _logger;
    private readonly IBetterPlaylistFileSystem _betterPlaylistFileSystem;

    public RestructurePlaylist(
        ILibraryManager libraryManager, 
        IPlaylistManager playlistManager, 
        IUserManager userManager, 
        IProviderManager providerManager, 
        ILogger<Plugin> logger,
        IServerApplicationPaths serverApplicationPaths)
    {
        _libraryManager = libraryManager;
        _playlistManager = playlistManager;
        _userManager = userManager;
        _providerManager = providerManager;
        _logger = logger;

        _betterPlaylistFileSystem = new BetterPlaylistFileSystem(serverApplicationPaths);
    }

    public async Task Run(IProgress<double> progress, CancellationToken cancellationToken)
    {
        foreach (var user in _userManager.Users)
        {
            _logger.Log(LogLevel.Information, $"Getting playlists for {user.Username}");
            var playlists = _playlistManager.GetPlaylists(user.Id);

            foreach (var playlist in playlists)
            {
                var filePath = _betterPlaylistFileSystem.GetBetterPlaylistFilePath(playlist.FileNameWithoutExtension);

                if (filePath != null)
                {
                    await using var reader = File.OpenRead(filePath);
                    var betterPlaylist = await JsonSerializer.DeserializeAsync<BetterPlaylist>(
                        reader, 
                        new JsonSerializerOptions(), 
                        cancellationToken).ConfigureAwait(false);
                    reader.Close();

                    var playlistItems = playlist.GetManageableItems().ToArray().Select(item => item.Item2.Id).ToList();
                    
                    _logger.Log(LogLevel.Debug, $"Current playlist contents: {string.Join(", ", playlistItems)}");
                    var resolvedItems = betterPlaylist.Queries
                        .Select(audioQuery => audioQuery.Resolve(_libraryManager, _providerManager).Id)
                        .Where(item => !playlistItems.Contains(item))
                        .ToList();

                    _logger.Log(LogLevel.Debug, $"To add: {string.Join(", ", resolvedItems)}");
                    _logger.Log(LogLevel.Debug, $"Adding {resolvedItems.Count} items to {playlist.Name}");
                    
                    await _playlistManager.AddToPlaylistAsync(playlist.Id, resolvedItems, user.Id);
                }
            }
        }
    }
}