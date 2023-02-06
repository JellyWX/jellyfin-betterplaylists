namespace Jellyfin.Plugin.BetterPlaylists;

public static class Strings
{
    public static class Endpoints
    {
        public static string LastfmApi = "ws.audioscrobbler.com";
    }

    public static class Methods
    {
        // Last.FM API specs located at https://last.fm/api
        public static string GetTopTracks = "user.gettoptracks";
    }

    public static class Keys
    {
        public static string LastfmApiKey = "";
    }
}