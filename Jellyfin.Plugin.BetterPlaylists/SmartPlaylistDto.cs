namespace Jellyfin.Plugin.BetterPlaylists;

using System;
using System.Collections.Generic;

[Serializable]
public class SmartPlaylistDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string FileName { get; set; }
    public string User { get; set; }
    public List<AudioQuery> Queries { get; set; }
}