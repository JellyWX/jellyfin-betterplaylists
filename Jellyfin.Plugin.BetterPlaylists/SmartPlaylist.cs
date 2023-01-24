using Jellyfin.Data.Entities;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jellyfin.Plugin.BetterPlaylists
{
    public class SmartPlaylist
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string User { get; set; }

        public SmartPlaylist(SmartPlaylistDto dto)
        {
            this.Id = dto.Id;
            this.Name = dto.Name;
            this.FileName = dto.FileName;
            this.User = dto.User;
        }

        // Returns the ID's of the items, if order is provided the IDs are sorted.
        public IEnumerable<Guid> FilterPlaylistItems(IEnumerable<BaseItem> items, ILibraryManager libraryManager,
            User user)
        {
            var results = new List<BaseItem> { };

            return results.Select(x => x.Id);
        }
    }
}