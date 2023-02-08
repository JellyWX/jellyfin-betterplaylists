using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.BetterPlaylists.LastFm;

public class BaseLastFmApiClient
{
    private const string ApiVersion = "2.0";

    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;

    public BaseLastFmApiClient(IHttpClientFactory httpClientFactory, ILogger logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }

    public async Task<TResponse> Get<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
        where TRequest : BaseRequest where TResponse : BaseResponse
    {
        var response =
            await _httpClient.GetFromJsonAsync<TResponse>(BuildGetUrl(request.Base, request.ToDictionary()),
                cancellationToken);

        if (response.IsError())
            _logger.LogError($"Could not GET LastFM: {response.Message}");

        return response;
    }

    #region Private methods

    private static string BuildGetUrl(string baseUrl, Dictionary<string, string> requestData)
    {
        return string.Format("{0}/{1}/?format=json&{2}",
            baseUrl,
            ApiVersion,
            Helpers.DictionaryToQueryString(requestData)
        );
    }

    #endregion
}