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

    public async Task<List<Issue>> GetIssuesAsync(
    int page = 1,
    int pageSize = 5)
{
    return await _httpClient.GetFromJsonAsync<List<Issue>>(
        $"api/issues?page={page}&pageSize={pageSize}")
        ?? new List<Issue>();
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

    public async Task<Issue?> GetIssueAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Issue>(
            $"api/issues/{id}");
    }
    public async Task<bool> UpdateIssueAsync(Issue issue)
{
    var response = await _httpClient.PutAsJsonAsync(
        $"api/issues/{issue.Id}",
        issue);

    return response.IsSuccessStatusCode;
}
  public async Task<bool> DeleteIssueAsync(int id)
    {
        var response = await _httpClient.DeleteAsync(
            $"api/issues/{id}");

        return response.IsSuccessStatusCode;
    }

    public async Task<List<Issue>> SearchIssuesAsync(
    string? status,
    string? priority,
    string? assignedTo)
{
    var url = "api/issues/search";

    var parameters = new List<string>();

    if (!string.IsNullOrWhiteSpace(status))
    {
        parameters.Add($"status={Uri.EscapeDataString(status)}");
    }

    if (!string.IsNullOrWhiteSpace(priority))
    {
        parameters.Add($"priority={Uri.EscapeDataString(priority)}");
    }

    if (!string.IsNullOrWhiteSpace(assignedTo))
    {
        parameters.Add($"assignedTo={Uri.EscapeDataString(assignedTo)}");
    }

    if (parameters.Count > 0)
    {
        url += "?" + string.Join("&", parameters);
    }

    return await _httpClient.GetFromJsonAsync<List<Issue>>(url)
           ?? new List<Issue>();
}

public async Task<DashboardStats?> GetDashboardStatsAsync()
{
    return await _httpClient.GetFromJsonAsync<DashboardStats>(
        "api/dashboard/stats");
}

public async Task<List<User>> GetUsersAsync()
{
    return await _httpClient.GetFromJsonAsync<List<User>>(
        "api/users") ?? new List<User>();
}
}