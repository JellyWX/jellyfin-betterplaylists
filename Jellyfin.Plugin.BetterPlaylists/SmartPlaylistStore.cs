﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.BetterPlaylists
{
    public interface ISmartPlaylistStore
    {
        Task<SmartPlaylistDto> GetSmartPlaylistAsync(Guid smartPlaylistId);
        Task<SmartPlaylistDto[]> LoadPlaylistsAsync(Guid userId);
        Task<SmartPlaylistDto[]> GetAllSmartPlaylistsAsync();
        void Save(SmartPlaylistDto smartPlaylist);
        void Delete(Guid userId, string smartPlaylistId);
    }

    public class SmartPlaylistStore : ISmartPlaylistStore
    {
        private readonly ISmartPlaylistFileSystem _fileSystem;
        private readonly IXmlSerializer _serializer;

        public SmartPlaylistStore(IXmlSerializer serializer, ISmartPlaylistFileSystem fileSystem)
        {
            _serializer = serializer;
            _fileSystem = fileSystem;
        }


        public async Task<SmartPlaylistDto> GetSmartPlaylistAsync(Guid smartPlaylistId)
        {
            var fileName = _fileSystem.GetSmartPlaylistFilePath(smartPlaylistId.ToString());

            return await LoadPlaylistAsync(fileName).ConfigureAwait(false);
        }

        public async Task<SmartPlaylistDto[]> LoadPlaylistsAsync(Guid userId)
        {
            var deserializeTasks = _fileSystem.GetSmartPlaylistFilePaths(userId.ToString()).Select(LoadPlaylistAsync).ToArray();

            await Task.WhenAll(deserializeTasks).ConfigureAwait(false);

            return deserializeTasks.Select(x => x.Result).ToArray();
        }

        public async Task<SmartPlaylistDto[]> GetAllSmartPlaylistsAsync()
        {
            var deserializeTasks = _fileSystem.GetAllSmartPlaylistFilePaths().Select(LoadPlaylistAsync).ToArray();

            await Task.WhenAll(deserializeTasks).ConfigureAwait(false);

            return deserializeTasks.Select(x => x.Result).ToArray();
        }

        public void Save(SmartPlaylistDto smartPlaylist)
        {
            var filePath = _fileSystem.GetSmartPlaylistPath(smartPlaylist.Id, smartPlaylist.FileName);
            _serializer.SerializeToFile(smartPlaylist, filePath);
        }

        public void Delete(Guid userId, string smartPlaylistId)
        {
            var filePath = _fileSystem.GetSmartPlaylistPath(userId.ToString(), smartPlaylistId);
            if (File.Exists(filePath)) File.Delete(filePath);
        }

        private async Task<SmartPlaylistDto> LoadPlaylistAsync(string filePath)
        {
            using (var reader = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None, 4096,
                FileOptions.Asynchronous))
            {
                var res = (SmartPlaylistDto)_serializer.DeserializeFromStream(typeof(SmartPlaylistDto), reader);
                return res;
            }
        }
    }
}