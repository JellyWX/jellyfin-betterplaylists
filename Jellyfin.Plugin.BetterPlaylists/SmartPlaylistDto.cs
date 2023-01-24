using System;
using System.Collections.Generic;
using Jellyfin.Plugin.BetterPlaylists.QueryEngine;

namespace Jellyfin.Plugin.BetterPlaylists
{
    [Serializable]
    public class SmartPlaylistDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string User { get; set; }
        public List<ExpressionSet> ExpressionSets { get; set; }
        public int MaxItems { get; set; }
    }
    
    public class ExpressionSet
    {
        public List<Expression> Expressions { get; set; }
    }
}