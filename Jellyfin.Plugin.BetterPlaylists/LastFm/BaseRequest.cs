namespace Jellyfin.Plugin.BetterPlaylists.LastFm;

using System.Collections.Generic;

public class BaseRequest
{
    public string ApiKey { get; set; }
    public string Method { get; set; }
    public string Base { get; set; }

    public virtual Dictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>
        {
            { "api_key", ApiKey },
            { "method", Method }
        };
    }
}