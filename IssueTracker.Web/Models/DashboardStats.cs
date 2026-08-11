namespace IssueTracker.Web.Models;

public class DashboardStats
{
    public int TotalIssues { get; set; }

    public int OpenIssues { get; set; }

    public int InProgressIssues { get; set; }

    public int ClosedIssues { get; set; }

    public int CriticalIssues { get; set; }

    public int HighPriorityIssues { get; set; }
}