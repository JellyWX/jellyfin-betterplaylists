using System;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Playlists;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;

namespace Jellyfin.Plugin.BetterPlaylists.LibraryPostScanTask;

public class RestructurePlaylist: ILibraryPostScanTask
{
    private readonly IFileSystem _fileSystem;
    private readonly ILibraryManager _libraryManager;
    private readonly IPlaylistManager _playlistManager;
    private readonly IProviderManager _providerManager;
    private readonly ISmartPlaylistFileSystem _plFileSystem;
    private readonly ISmartPlaylistStore _plStore;
    private readonly IUserManager _userManager;
    
    public Task Run(IProgress<double> progress, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}