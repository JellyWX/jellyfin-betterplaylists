# BetterPlaylists

**A small plugin to improve & enhance playlist experience in Jellyfin**

BetterPlaylists allows you to specify music playlists using MBIDs or search queries
rather than by file path.

This means one can construct playlists that are resilient to directory restructure, 
or construct playlists from data extracted from external providers such as Last.FM.

BetterPlaylists also adds support for automatically generating *some* dynamic playlists 
from Last.FM, such as an "On Repeat" playlist.

## Example use case

Here is an example use case for converting a Last.FM playlist into a Jellyfin playlist.

1. Create a new playlist on Jellyfin, and get the file name of the playlist. This is probably the same as the name, unless the name has been used before.
2. Go to the playlist in Last.FM and scroll to the bottom to force it to load. Open dev tools.
3. Execute the following in the JavaScript console:

   `JSON.stringify({"Type": "querylist", "Queries": [...document.querySelectorAll(".chartlist-name")].map(el => ({ ArtistName: el.parentNode.querySelector(".chartlist-artist").textContent.trim(), SongName: el.textContent.trim() }))})`

For example, my (prettified) output is:

    {
        "Type": "querylist",
        "Queries": [
            {
                "ArtistName": "Hot Milk",
                "SongName": "Awful Ever After"
            },
            {
                "ArtistName": "Stand Atlantic",
                "SongName": "Hate Me (Sometimes)"
            },
            {
                "ArtistName": "No Love For The Middle Child",
                "SongName": "Pretty Little Lies"
            },
            {
                "ArtistName": "Avril Lavigne",
                "SongName": "Girlfriend"
            },
            {
                "ArtistName": "The Cumshots",
                "SongName": "And The Sun Pissed Red"
            },
            ...
        ]
    }

4. Copy the output to a file `/data/betterplaylists/Top 100.json`, where the name of the file is the same as the file name of the playlist.
5. Run "Scan All Media" and BetterPlaylists will populate the playlist for you.

## Available queries

There are 4 available queries:

* Musicbrainz ID (untested)
* SongName
* ArtistName
* AlbumName

ArtistName and AlbumName are only useful along with SongName. They only apply if multiple 
songs match the song name.

The plugin will perform song name matches case insensitively, and also removes all 
apostrophes during comparison (as often outputs contain inconsistent use of the 
different unicode apostrophes).