using IssueTracker.Api.Data;
using IssueTracker.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IssuesController : ControllerBase
{
    private readonly AppDbContext _context;

    public IssuesController(AppDbContext context)
    {
        _context = context;
    }

    // =========================================================
    // GET: api/issues
    // Get all issues
    // =========================================================

    [HttpGet]
public async Task<ActionResult<IEnumerable<Issue>>> GetIssues(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5)
{
    if (page < 1)
    {
        page = 1;
    }

    if (pageSize < 1)
    {
        pageSize = 5;
    }

    var issues = await _context.Issues
        .OrderBy(i => i.Id)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return Ok(issues);
}

    // =========================================================
    // GET: api/issues/{id}
    // Get a specific issue by ID
    // =========================================================

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Issue>> GetIssue(int id)
    {
        var issue = await _context.Issues.FindAsync(id);

        if (issue == null)
        {
            return NotFound();
        }

        return Ok(issue);
    }


    // =========================================================
    // POST: api/issues
    // Create a new issue
    // =========================================================

    [HttpPost]
    public async Task<ActionResult<Issue>> CreateIssue(Issue issue)
    {
        _context.Issues.Add(issue);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetIssue),
            new { id = issue.Id },
            issue);
    }


    // =========================================================
    // PUT: api/issues/{id}
    // Update an existing issue
    // =========================================================

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateIssue(
        int id,
        Issue updatedIssue)
    {
        if (id != updatedIssue.Id)
        {
            return BadRequest();
        }

        var issue = await _context.Issues.FindAsync(id);

        if (issue == null)
        {
            return NotFound();
        }

        issue.Title = updatedIssue.Title;
        issue.Description = updatedIssue.Description;
        issue.Status = updatedIssue.Status;
        issue.Priority = updatedIssue.Priority;
        issue.AssignedTo = updatedIssue.AssignedTo;

        await _context.SaveChangesAsync();

        return NoContent();
    }


    // =========================================================
    // DELETE: api/issues/{id}
    // Delete an issue
    // =========================================================

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteIssue(int id)
    {
        var issue = await _context.Issues.FindAsync(id);

        if (issue == null)
        {
            return NotFound();
        }

        _context.Issues.Remove(issue);

        await _context.SaveChangesAsync();

        return NoContent();
    }


    // =========================================================
    // GET: api/issues/search
    // Search issues using query parameters
    //
    // Examples:
    //
    // /api/issues/search?status=Open
    //
    // /api/issues/search?priority=High
    //
    // /api/issues/search?status=Open&priority=High
    //
    // /api/issues/search?assignedTo=Piyu
    // =========================================================

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Issue>>> SearchIssues(
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] string? assignedTo)
    {
        var query = _context.Issues.AsQueryable();

        // Filter by Status
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(i => i.Status == status);
        }

        // Filter by Priority
        if (!string.IsNullOrWhiteSpace(priority))
        {
            query = query.Where(i => i.Priority == priority);
        }

        // Filter by Assigned To
        if (!string.IsNullOrWhiteSpace(assignedTo))
        {
            query = query.Where(i => i.AssignedTo == assignedTo);
        }

        var issues = await query.ToListAsync();

        return Ok(issues);
    }


    // =========================================================
    // GET: api/issues/filter
    // Filter issues using query parameters
    //
    // This currently performs the same filtering as /search.
    // We can later give /filter different functionality if needed.
    // =========================================================

    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<Issue>>> FilterIssues(
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] string? assignedTo)
    {
        var query = _context.Issues.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(i => i.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(priority))
        {
            query = query.Where(i => i.Priority == priority);
        }

        if (!string.IsNullOrWhiteSpace(assignedTo))
        {
            query = query.Where(i => i.AssignedTo == assignedTo);
        }

        var issues = await query.ToListAsync();

        return Ok(issues);
    }
}