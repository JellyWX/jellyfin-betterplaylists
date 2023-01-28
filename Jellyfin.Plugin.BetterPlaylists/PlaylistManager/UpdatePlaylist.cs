using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Playlists;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Playlists;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.BetterPlaylists.PlaylistManager;

public class UpdatePlaylist : ILibraryMonitor
{
    private readonly ILibraryManager _libraryManager;
    private readonly IPlaylistManager _playlistManager;
    private readonly IUserManager _userManager;
    private readonly IProviderManager _providerManager;
    private readonly ILogger<Plugin> _logger;
    private readonly IBetterPlaylistFileSystem _betterPlaylistFileSystem;

    public UpdatePlaylist(
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

        _logger.Log(LogLevel.Critical, "Here 2");
        _betterPlaylistFileSystem = new BetterPlaylistFileSystem(serverApplicationPaths);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void Start()
    {
        _logger.Log(LogLevel.Critical, "Here");
    }

    public void Stop()
    {
        throw new NotImplementedException();
    }

    public void ReportFileSystemChangeBeginning(string path)
    {
        throw new NotImplementedException();
    }

    public void ReportFileSystemChangeComplete(string path, bool refreshPath)
    {
        throw new NotImplementedException();
    }

    public void ReportFileSystemChanged(string path)
    {
        throw new NotImplementedException();
    }

    public bool IsPathLocked(string path)
    {
        throw new NotImplementedException();
    }
}