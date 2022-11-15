using System.Net;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Umbraco.Common.Models.ApplicationSettings;
using Umbraco.Common.Models.Dtos;

namespace Umbraco.Common.Services;

public class ShowcaseService : IShowcaseService
{
    private readonly HttpClient _httpClient;
    private readonly string BaseUrl;

    public ShowcaseService(IOptions<ServiceSettings> serviceSettings, HttpClient httpClient)
    {
        _httpClient = httpClient;
        BaseUrl = serviceSettings.Value.ShowcaseBaseUrl;
    }

    public async Task<Showcase?> GetShowcaseById(string id)
    {
        var url = BaseUrl + $"api/showcases/{id}";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<Showcase>(url);
            if (response != null) 
                return response;
        }
        catch (HttpRequestException httpRequestException)
        {
            if (httpRequestException.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        return null;
    }

    public async Task<PaginatedList<Showcase>> GetAllShowcases(int pageSize = 10, int currentPage = 1)
    {
        var url = BaseUrl + "api/showcase";

        var queryParams = new Dictionary<string, string>
        {
            { "pageSize", pageSize.ToString() },
            { "currentPage", currentPage.ToString() },
        };

        url = QueryHelpers.AddQueryString(url, queryParams);

        var response = await _httpClient.GetStringAsync(url);

        // return empty so that our calling methods never "fail"
        return JsonConvert.DeserializeObject<PaginatedList<Showcase>>(response) ?? new PaginatedList<Showcase>();
    }
}