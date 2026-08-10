using System.Net.Http.Json;
using IssueTracker.Web.Models;

namespace IssueTracker.Web.Services;

public class IssueService
{
    private readonly HttpClient _httpClient;

    public IssueService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("IssueTrackerApi");
    }

    public async Task<List<Issue>> GetIssuesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Issue>>(
            "api/issues") ?? new List<Issue>();
    }

    public async Task<Issue?> CreateIssueAsync(Issue issue)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/issues",
            issue);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<Issue>();
    }
}