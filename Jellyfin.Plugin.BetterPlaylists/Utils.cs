namespace Jellyfin.Plugin.BetterPlaylists;

using System;
using System.Collections.Generic;
using System.Linq;

public static class Helpers
{
    public static string DictionaryToQueryString(Dictionary<string, string> data)
    {
        return string.Join("&",
            data.Where(k => !string.IsNullOrWhiteSpace(k.Value)).Select(kvp =>
                string.Format("{0}={1}", Uri.EscapeDataString(kvp.Key), Uri.EscapeDataString(kvp.Value))));
    }
}