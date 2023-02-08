using System.IO;
using System.Linq;
using MediaBrowser.Common.Configuration;

namespace Jellyfin.Plugin.BetterPlaylists;

public interface IBetterPlaylistFileSystem
{
    string GetBetterPlaylistFilePath(string playlistName);
    string GetConfigPath();
}

public class BetterPlaylistFileSystem : IBetterPlaylistFileSystem
{
    public BetterPlaylistFileSystem(IApplicationPaths serverApplicationPaths)
    {
        BasePath = Path.Combine(serverApplicationPaths.DataPath, "betterplaylists");
        if (!Directory.Exists(BasePath)) Directory.CreateDirectory(BasePath);

        PlaylistPath = Path.Combine(BasePath, "playlists");
        if (!Directory.Exists(PlaylistPath)) Directory.CreateDirectory(BasePath);

        ConfigPath = Path.Combine(BasePath, "config");
        if (!Directory.Exists(ConfigPath)) Directory.CreateDirectory(BasePath);
    }

    public string BasePath { get; }
    public string PlaylistPath { get; }
    public string ConfigPath { get; }

    public string GetBetterPlaylistFilePath(string playlistName)
    {
        return Directory.GetFiles(PlaylistPath, $"{playlistName}.json", SearchOption.TopDirectoryOnly).FirstOrDefault();
    }

    public string GetConfigPath()
    {
        return Directory.GetFiles(ConfigPath, "config.json", SearchOption.TopDirectoryOnly).FirstOrDefault();
    }
}