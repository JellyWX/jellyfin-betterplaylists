namespace Jellyfin.Plugin.BetterPlaylists;

using System;
using System.Collections.Generic;

[Serializable]
public class BetterPlaylist
{
    public string Type { get; set; }
    public List<AudioQuery> Queries { get; set; }
}