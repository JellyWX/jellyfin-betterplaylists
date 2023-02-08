using System;
using System.IO;
using System.Text.Json;

namespace Jellyfin.Plugin.BetterPlaylists;

[Serializable]
public class Config
{
    private const string DefaultLastFmApiBase = "https://ws.audioscrobbler.com";

    public string LastFmApiBase { get; set; }
    public string LastFmApiKey { get; set; }

    public static Config Init(IBetterPlaylistFileSystem betterPlaylistFileSystem)
    {
        var configPath = betterPlaylistFileSystem.GetConfigPath();
        if (File.Exists(configPath))
        {
            var reader = File.OpenRead(configPath);
            var config = JsonSerializer.Deserialize<Config>(reader);
            reader.Close();

            return config;
        }
        else
        {
            var config = new Config
            {
                LastFmApiBase = DefaultLastFmApiBase,
                LastFmApiKey = ""
            };

            if (configPath != null)
            {
                var configFile = File.Create(configPath);
                JsonSerializer.Serialize(configFile, config);
            }

            return config;
        }
    }
}