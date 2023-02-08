using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.BetterPlaylists.LastFm;

public class BaseLastFmApiClient
{
    private const string ApiVersion = "2.0";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;

    public BaseLastFmApiClient(IHttpClientFactory httpClientFactory, ILogger logger)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient();
        _logger = logger;
    }

    public async Task<TResponse> Get<TRequest, TResponse>(TRequest request)
        where TRequest : BaseRequest where TResponse : BaseResponse
    {
        return await Get<TRequest, TResponse>(request, CancellationToken.None);
    }

    public async Task<TResponse> Get<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
        where TRequest : BaseRequest where TResponse : BaseResponse
    {
        var response =
            await _httpClient.GetFromJsonAsync<TResponse>(BuildGetUrl(request.ToDictionary(), request.Secure),
                cancellationToken);

        if (response.IsError())
            _logger.LogError($"Could not GET LastFM: {response.Message}");

        return response;
    }

    #region Private methods

    private static string BuildGetUrl(Dictionary<string, string> requestData, bool secure)
    {
        return string.Format("{0}://{1}/{2}/?format=json&{3}",
            secure ? "https" : "http",
            Strings.Endpoints.LastfmApi,
            ApiVersion,
            Helpers.DictionaryToQueryString(requestData)
        );
    }

    #endregion
}