using System;
using System.IO;
using System.Linq;
using MediaBrowser.Controller;

namespace Jellyfin.Plugin.BetterPlaylists;

public interface IBetterPlaylistFileSystem
{
    string BasePath { get; }
    string GetBetterPlaylistFilePath(string playlistName);
}

public class BetterPlaylistFileSystem : IBetterPlaylistFileSystem
{
    public BetterPlaylistFileSystem(IServerApplicationPaths serverApplicationPaths)
    {
        BasePath = Path.Combine(serverApplicationPaths.DataPath, "betterplaylists");
        if (!Directory.Exists(BasePath)) Directory.CreateDirectory(BasePath);
    }

    public string BasePath { get; }

    public string GetBetterPlaylistFilePath(string playlistName)
    {
        return Directory.GetFiles(BasePath, $"{playlistName}.json", SearchOption.AllDirectories).First();
    }
}