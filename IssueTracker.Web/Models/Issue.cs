// here the issue model is created to get the issue details from the database and to display the issue details in the view 

namespace IssueTracker.Web.Models;

public class Issue
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = "Open";

    public string Priority { get; set; } = "Medium";

    public string AssignedTo { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}