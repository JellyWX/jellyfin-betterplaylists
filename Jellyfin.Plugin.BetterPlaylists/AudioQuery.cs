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
    public string MusicbrainzId;
    // public string SongName;
    // public string AlbumName;
    // public string ArtistName;

    public IEnumerable<Guid> Resolve(ILibraryManager libraryManager, IProviderManager providerManager)
    {
        var query = new InternalItemsQuery
        {
            MediaTypes = new [] { nameof(Audio) }
        };
        
        var guids = libraryManager.GetItemIds(query);

        var matches = guids
            .Where(guid => providerManager
                .GetExternalIdInfos(libraryManager.GetItemById(guid))
                .First(info => info.Name == "Musicbrainz")
                .Key == MusicbrainzId
                );

        return matches;
    } 
}