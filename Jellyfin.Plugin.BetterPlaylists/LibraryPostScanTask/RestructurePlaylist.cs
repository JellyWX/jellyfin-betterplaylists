using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Data.Entities;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Playlists;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Serialization;
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
    private readonly IXmlSerializer _xmlSerializer;

    public RestructurePlaylist(
        ILibraryManager libraryManager, 
        IPlaylistManager playlistManager, 
        IUserManager userManager, 
        IProviderManager providerManager, 
        ILogger<Plugin> logger,
        IXmlSerializer xmlSerializer,
        IServerApplicationPaths serverApplicationPaths)
    {
        _libraryManager = libraryManager;
        _playlistManager = playlistManager;
        _userManager = userManager;
        _providerManager = providerManager;
        _logger = logger;
        _xmlSerializer = xmlSerializer;

        _betterPlaylistFileSystem = new BetterPlaylistFileSystem(serverApplicationPaths);
    }

    public async Task Run(IProgress<double> progress, CancellationToken cancellationToken)
    {
        foreach (User user in _userManager.Users)
        {
            _logger.Log(LogLevel.Information, $"Getting playlists for {user.Username}");
            IEnumerable<Playlist> playlists = _playlistManager.GetPlaylists(user.Id);

            _logger.Log(LogLevel.Information, "Found playlists");
            foreach (Playlist playlist in playlists)
            {
                _logger.Log(LogLevel.Information, $"Looking for {playlist.Name}.json");
                
                string filePath = _betterPlaylistFileSystem.GetBetterPlaylistFilePath(playlist.Name);
                _logger.Log(LogLevel.Information, $"Loading {playlist.Name} from {filePath}");
                await using var reader = File.OpenRead(filePath);
                BetterPlaylist betterPlaylist = await JsonSerializer.DeserializeAsync<BetterPlaylist>(reader).ConfigureAwait(false);
                reader.Close();
        
                // Check file matches playlist
                foreach (AudioQuery query in betterPlaylist.Queries)
                {
                    _logger.Log(LogLevel.Information, $"Song: {query.SongName}");
                }
            }
        }
    }
}