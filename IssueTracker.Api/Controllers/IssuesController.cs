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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Issue>>> GetIssues()
    {
        var issues = await _context.Issues.ToListAsync();

        return Ok(issues);
    }

    [HttpPost]
    public async Task<ActionResult<Issue>> CreateIssue(Issue issue)
    {
        _context.Issues.Add(issue);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetIssues),
            new { id = issue.Id },
            issue);
    }

    //Here the getissue method is added to the issue controller to get a specific issue 
    //selecting the particular id that is needed for fetching the issue from the database.


    [HttpGet("{id}")]
    public async Task<ActionResult<Issue>> GetIssue(int id)
    {
        var issue = await _context.Issues.FindAsync(id);

        if (issue == null)
        {
            return NotFound();
        }

        return Ok(issue);
    }

    //the next controllers restapi , we will execute include 
    // delete by id  and update the id 

    [HttpDelete("{id}")]
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIssue(int id, Issue updatedIssue)
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
    // getting the issues by using the search method to get the issue 
    // the search method is used to  get the issues from it id 

    

    //here the filter get method is added to issuethe controller 
    /// <summary>
    ///  fliter based on status, priority, and assignedTo parameters.
    /// </summary>
    /// <param name="status"></param>
    /// <param name="priority"></param>
    /// <param name="assignedTo"></param>
    /// <returns></returns>

    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<Issue>>> FilterIssues(
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] string? assignedTo)
    {
        var query = _context.Issues.AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(i => i.Status == status);
        }

        if (!string.IsNullOrEmpty(priority))
        {
            query = query.Where(i => i.Priority == priority);
        }

        if (!string.IsNullOrEmpty(assignedTo))
        {
            query = query.Where(i => i.AssignedTo == assignedTo);
        }

        var issues = await query.ToListAsync();

        return Ok(issues);
    }


}