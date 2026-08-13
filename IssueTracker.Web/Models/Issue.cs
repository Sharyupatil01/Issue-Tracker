namespace IssueTracker.Web.Models;

public class Issue
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public int? AssignedToUserId { get; set; }

    public User? AssignedToUser { get; set; }
}