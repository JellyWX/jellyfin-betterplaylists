using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.BetterPlaylists.LastFm;

public class LastFmApi : BaseLastFmApiClient
{
    private readonly ILogger _logger;
    private readonly Config _config;

    public LastFmApi(IHttpClientFactory httpClientFactory, ILogger logger, Config config) : base(httpClientFactory,
        logger)
    {
        _logger = logger;
        _config = config;
    }

    public async Task<GetTopTracksResponse> GetTopTracks(string user,
        CancellationToken cancellationToken)
    {
        var request = new GetTopTracksRequest
        {
            User = user,
            Period = "7day",
            ApiKey = _config.LastFmApiKey,
            Method = Strings.Methods.GetTopTracks,
            Limit = 30,
            Page = 1,
            Base = _config.LastFmApiBase
        };

        return await Get<GetTopTracksRequest, GetTopTracksResponse>(request, cancellationToken);
    }
}

public class GetTopTracksRequest : BaseRequest
{
    public string User { get; set; }
    public string Period { get; set; }
    public int Limit { get; set; }
    public int Page { get; set; }

    public override Dictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>(base.ToDictionary())
        {
            { "user", User },
            { "period", Period },
            { "limit", Limit.ToString() },
            { "page", Page.ToString() }
        };
    }
}

public class GetTopTracksResponse : BaseResponse
{
    [JsonPropertyName("toptracks")] public GetTopTracksTracks TopTracks { get; set; }

    public IEnumerable<AudioQuery> AudioQueries()
    {
        return TopTracks.Tracks.Select(track => new AudioQuery
            { MusicbrainzId = track.MusicBrainzId, SongName = track.Name, ArtistName = track.Artist.Name }).ToList();
    }
}

public class GetTopTracksTracks
{
    [JsonPropertyName("track")] public List<LastfmTrack> Tracks { get; set; }
}

public class LastfmArtist
{
    [JsonPropertyName("name")] public string Name { get; set; }
}

public class LastfmTrack
{
    [JsonPropertyName("artist")] public LastfmArtist Artist { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("mbid")] public string MusicBrainzId { get; set; }
}