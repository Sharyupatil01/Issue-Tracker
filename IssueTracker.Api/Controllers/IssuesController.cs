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
    // Get all issues with pagination
    //
    // Example:
    // /api/issues?page=1&pageSize=5
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
            .Include(i => i.AssignedToUser)
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
        var issue = await _context.Issues
            .Include(i => i.AssignedToUser)
            .FirstOrDefaultAsync(i => i.Id == id);

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
        // If an AssignedToUserId was supplied,
        // make sure that user actually exists.

        if (issue.AssignedToUserId.HasValue)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == issue.AssignedToUserId.Value);

            if (!userExists)
            {
                return BadRequest(
                    "The assigned user does not exist.");
            }
        }

        _context.Issues.Add(issue);

        await _context.SaveChangesAsync();

        // Load the assigned user before returning the response
        await _context.Entry(issue)
            .Reference(i => i.AssignedToUser)
            .LoadAsync();

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
            return BadRequest(
                "The ID in the URL does not match the issue ID.");
        }

        var issue = await _context.Issues.FindAsync(id);

        if (issue == null)
        {
            return NotFound();
        }

        // Validate assigned user
        if (updatedIssue.AssignedToUserId.HasValue)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == updatedIssue.AssignedToUserId.Value);

            if (!userExists)
            {
                return BadRequest(
                    "The assigned user does not exist.");
            }
        }

        issue.Title = updatedIssue.Title;
        issue.Description = updatedIssue.Description;
        issue.Status = updatedIssue.Status;
        issue.Priority = updatedIssue.Priority;
        issue.AssignedToUserId = updatedIssue.AssignedToUserId;

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
    //
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
    // /api/issues/search?assignedToUserId=1
    //
    // /api/issues/search?status=Open&assignedToUserId=1
    // =========================================================

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Issue>>> SearchIssues(
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] int? assignedToUserId)
    {
        var query = _context.Issues
            .Include(i => i.AssignedToUser)
            .AsQueryable();

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

        // Filter by Assigned User
        if (assignedToUserId.HasValue)
        {
            query = query.Where(i =>
                i.AssignedToUserId == assignedToUserId.Value);
        }

        var issues = await query
            .OrderBy(i => i.Id)
            .ToListAsync();

        return Ok(issues);
    }


    // =========================================================
    // GET: api/issues/filter
    //
    // Filter issues using query parameters
    //
    // Examples:
    //
    // /api/issues/filter?status=Open
    //
    // /api/issues/filter?priority=Critical
    //
    // /api/issues/filter?assignedToUserId=2
    //
    // /api/issues/filter?status=Open&priority=High
    // =========================================================

    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<Issue>>> FilterIssues(
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] int? assignedToUserId)
    {
        var query = _context.Issues
            .Include(i => i.AssignedToUser)
            .AsQueryable();

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

        // Filter by Assigned User
        if (assignedToUserId.HasValue)
        {
            query = query.Where(i =>
                i.AssignedToUserId == assignedToUserId.Value);
        }

        var issues = await query
            .OrderBy(i => i.Id)
            .ToListAsync();

        return Ok(issues);
    }
}