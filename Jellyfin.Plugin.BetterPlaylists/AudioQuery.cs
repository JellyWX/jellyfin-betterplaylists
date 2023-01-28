using System;
using System.Collections.Generic;
using System.Linq;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;

namespace Jellyfin.Plugin.BetterPlaylists;

[Serializable]
public class AudioQuery
{
    public string MusicbrainzId { get; set; }
    public string SongName { get; set; }
    public string AlbumName { get; set; }

    public BaseItem Resolve(ILibraryManager libraryManager, IProviderManager providerManager)
    {
        var query = new InternalItemsQuery
        {
            MediaTypes = new[] { nameof(Audio) }
        };

        var items = libraryManager.GetItemList(query);

        BaseItem matches; 
        if (MusicbrainzId != null)
        {
            matches = items.First(item => providerManager
                .GetExternalIdInfos(item)
                .First(info => info.Name == "Musicbrainz")
                .Key == MusicbrainzId);
        }
        else
        {
            matches = items.First(item => string.Equals(item.Album, AlbumName, StringComparison.CurrentCultureIgnoreCase) 
                                          && string.Equals(item.Name, SongName, StringComparison.CurrentCultureIgnoreCase));
        }

        return matches;
    }
}