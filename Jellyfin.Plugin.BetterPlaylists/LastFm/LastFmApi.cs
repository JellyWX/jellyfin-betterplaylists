using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities.Audio;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.BetterPlaylists.LastFm;

public class LastFmApi : BaseLastFmApiClient
{
    private readonly ILogger _logger;

    public LastFmApi(IHttpClientFactory httpClientFactory, ILogger logger) : base(httpClientFactory, logger)
    {
        _logger = logger;
    }

    public async Task<GetTopTracksResponse> GetTopTracks(string user,
        CancellationToken cancellationToken)
    {
        var request = new GetTopTracksRequest
        {
            User = user,
            Period = "7day",
            ApiKey = Strings.Keys.LastfmApiKey,
            Method = Strings.Methods.GetTopTracks,
            Limit = 30,
            Secure = true
        };

        return await Get<GetTopTracksRequest, GetTopTracksResponse>(request, cancellationToken);
    }
}

public class GetTopTracksRequest : BaseRequest, IPagedRequest
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

[DataContract]
public class GetTopTracksResponse : BaseResponse
{
    [DataMember(Name = "tracks")] public GetTracksTracks Tracks { get; set; }
}

[DataContract]
public class GetTracksTracks
{
    [DataMember(Name = "track")] public List<LastfmTrack> Tracks { get; set; }

    [DataMember(Name = "@attr")] public GetTracksMeta Metadata { get; set; }
}

[DataContract]
public class GetTracksMeta
{
    [DataMember(Name = "totalPages")] public int TotalPages { get; set; }

    [DataMember(Name = "total")] public int TotalTracks { get; set; }

    [DataMember(Name = "page")] public int Page { get; set; }
}

[DataContract]
public class LastfmArtist
{
    [DataMember(Name = "name")] public string Name { get; set; }

    [DataMember(Name = "mbid")] public string MusicBrainzId { get; set; }
}

[DataContract]
public class LastfmTrack
{
    [DataMember(Name = "artist")] public LastfmArtist Artist { get; set; }

    [DataMember(Name = "name")] public string Name { get; set; }

    [DataMember(Name = "mbid")] public string MusicBrainzId { get; set; }

    [DataMember(Name = "playcount")] public int PlayCount { get; set; }
}