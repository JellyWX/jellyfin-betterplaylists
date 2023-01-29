using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Extensions;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.BetterPlaylists;

[Serializable]
public class AudioQuery
{
    public string MusicbrainzId { get; set; }
    public string SongName { get; set; }
    public string AlbumName { get; set; }
    public string ArtistName { get; set; }

    public BaseItem Resolve(ILogger logger, ILibraryManager libraryManager, IProviderManager providerManager)
    {
        logger.Log(LogLevel.Information, $"Resolving: {SongName} by {ArtistName} from {AlbumName} with MBID {MusicbrainzId}");
        
        var query = new InternalItemsQuery
        {
            MediaTypes = new[] { nameof(Audio) }
        };

        var items = libraryManager.GetItemList(query);

        BaseItem matches; 
        if (MusicbrainzId != null)
        {
            matches = items.FirstOrDefault(item => providerManager
                .GetExternalIdInfos(item)
                .First(info => info.Name == "Musicbrainz")
                .Key == MusicbrainzId);
        }
        else
        {
            var filter = items.Where(item => string.Equals(
                item.Name.Replace("’", "").Replace("'", ""), 
                SongName.Replace("’", "").Replace("'", ""), 
                StringComparison.CurrentCultureIgnoreCase)).ToArray();

            if (filter.Length > 1 && AlbumName != null)
            {
                filter = filter.Where(item => string.Equals(item.Album, AlbumName, StringComparison.CurrentCultureIgnoreCase)).ToArray();
            }
            
            if (filter.Length > 1 && ArtistName != null)
            {
                var newFilter = filter.Where(item =>
                {
                    var artist = item.FindParent<MusicArtist>();
                    return artist == null || string.Equals(artist.Name, ArtistName, StringComparison.CurrentCultureIgnoreCase);
                }).ToArray();

                if (newFilter.Length > 0)
                {
                    filter = newFilter;
                }
            }

            matches = filter.FirstOrDefault();
        }
        
        logger.LogInformation($"Match found?: {matches != null}");

        return matches;
    }
}