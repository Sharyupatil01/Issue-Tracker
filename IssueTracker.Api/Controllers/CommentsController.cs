using IssueTracker.Api.Data;
using IssueTracker.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Api.Controllers;

[ApiController]
[Route("api")]
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CommentsController(AppDbContext context)
    {
        _context = context;
    }


    // =========================================================
    // GET: api/issues/{issueId}/comments
    // Get all comments for a specific issue
    // =========================================================

    [HttpGet("issues/{issueId:int}/comments")]
    public async Task<ActionResult<IEnumerable<Comment>>> GetComments(
        int issueId)
    {
        // Check whether issue exists

        var issueExists = await _context.Issues
            .AnyAsync(i => i.Id == issueId);

        if (!issueExists)
        {
            return NotFound("Issue not found.");
        }

        var comments = await _context.Comments
            .Where(c => c.IssueId == issueId)
            .Include(c => c.User)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        return Ok(comments);
    }


    // =========================================================
    // POST: api/issues/{issueId}/comments
    // Create a comment for an issue
    // =========================================================

    [HttpPost("issues/{issueId:int}/comments")]
    public async Task<ActionResult<Comment>> CreateComment(
        int issueId,
        Comment comment)
    {
        // Check whether issue exists

        var issueExists = await _context.Issues
            .AnyAsync(i => i.Id == issueId);

        if (!issueExists)
        {
            return NotFound("Issue not found.");
        }


        // Check whether user exists

        var userExists = await _context.Users
            .AnyAsync(u => u.Id == comment.UserId);

        if (!userExists)
        {
            return BadRequest("User not found.");
        }


        // Make sure the comment belongs to
        // the issue from the URL

        comment.IssueId = issueId;

        // Let the server generate the creation time

        comment.CreatedAt = DateTime.UtcNow;

        _context.Comments.Add(comment);

        await _context.SaveChangesAsync();


        // Load the User so that the response
        // contains user information

        await _context.Entry(comment)
            .Reference(c => c.User)
            .LoadAsync();

        return CreatedAtAction(
            nameof(GetComments),
            new { issueId = issueId },
            comment);
    }


    // =========================================================
    // DELETE: api/comments/{id}
    // Delete a comment
    // =========================================================

    [HttpDelete("comments/{id:int}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var comment = await _context.Comments
            .FindAsync(id);

        if (comment == null)
        {
            return NotFound();
        }

        _context.Comments.Remove(comment);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}

