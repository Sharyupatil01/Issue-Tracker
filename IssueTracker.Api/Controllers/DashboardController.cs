using IssueTracker.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = new
        {
            TotalIssues = await _context.Issues.CountAsync(),

            OpenIssues = await _context.Issues
                .CountAsync(i => i.Status == "Open"),

            InProgressIssues = await _context.Issues
                .CountAsync(i => i.Status == "In Progress"),

            ClosedIssues = await _context.Issues
                .CountAsync(i => i.Status == "Closed"),

            CriticalIssues = await _context.Issues
                .CountAsync(i => i.Priority == "Critical"),

            HighPriorityIssues = await _context.Issues
                .CountAsync(i => i.Priority == "High")
        };

        return Ok(stats);
    }
}