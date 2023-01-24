﻿using System;
using System.IO;
using System.Linq;
using MediaBrowser.Controller;

namespace Jellyfin.Plugin.BetterPlaylists
{
    public interface ISmartPlaylistFileSystem
    {
        string BasePath { get; }
        string GetSmartPlaylistFilePath(string smartPlaylistId);
        string[] GetSmartPlaylistFilePaths(string userId);
        string[] GetAllSmartPlaylistFilePaths();
        string GetSmartPlaylistPath(string userId, string playlistId);
    }
    public class SmartPlaylistFileSystem : ISmartPlaylistFileSystem
    {
        // Class Constructor for SmartPlaylistFileSystem
        // Creates directory if it doesn't exist
        public SmartPlaylistFileSystem(IServerApplicationPaths serverApplicationPaths)
        {
            BasePath = Path.Combine(serverApplicationPaths.DataPath, "smartplaylists");
            if (!Directory.Exists(BasePath)) Directory.CreateDirectory(BasePath);
        }

        public string BasePath { get; }

        public string GetSmartPlaylistFilePath(string smartPlaylistId)
        {
            return Directory.GetFiles(BasePath, $"{smartPlaylistId}.json", SearchOption.AllDirectories).First();
        }

        public string[] GetSmartPlaylistFilePaths(string userId)
        {
            return Directory.GetFiles(BasePath);
        }

        public string[] GetAllSmartPlaylistFilePaths()
        {
            return Directory.GetFiles(BasePath, "*.json", SearchOption.AllDirectories);
        }

        public string GetSmartPlaylistPath(string userId, string playlistId)
        {
            return Path.Combine(BasePath, $"{playlistId}.json");
        }
    }
}