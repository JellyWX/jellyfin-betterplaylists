using System;
using System.Collections.Generic;

namespace Jellyfin.Plugin.BetterPlaylists;

[Serializable]
public class BetterPlaylist
{
    public string Name { get; set; }
    public List<AudioQuery> Queries { get; set; }
}