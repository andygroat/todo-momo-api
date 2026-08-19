using Microsoft.EntityFrameworkCore;
using Modules.Todos.Domain;
using System.Diagnostics.CodeAnalysis;

namespace Modules.Todos.Infrastructure.Database;

[ExcludeFromCodeCoverage]
public sealed class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the DbSet of TodoItem entities.
    /// </summary>
    public DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Default);
    }
}
