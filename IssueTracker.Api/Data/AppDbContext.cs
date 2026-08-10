using IssueTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Issue> Issues => Set<Issue>();
}