using Microsoft.EntityFrameworkCore;
using Modules.ShoppingLists.Domain;
using System.Diagnostics.CodeAnalysis;

namespace Modules.ShoppingLists.Infrastructure.Database;

/// <summary>
/// The Schemas class contains constants for the database schema names used by the shopping lists module.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Schemas
{
    /// <summary>
    /// The schema name for the default related tables.
    /// </summary>
    public const string Default = "dbo";
}

/// <summary>
/// Entity Framework Core database context for the shopping lists module.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class ShoppingListDbContext(DbContextOptions<ShoppingListDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the DbSet of <see cref="ShoppingList"/> entities.
    /// </summary>
    public DbSet<ShoppingList> ShoppingLists { get; set; }

    /// <summary>
    /// Gets or sets the DbSet of <see cref="ShoppingListItem"/> entities.
    /// </summary>
    public DbSet<ShoppingListItem> ShoppingListItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Default);

        modelBuilder.Entity<ShoppingList>()
            .HasMany(list => list.Items)
            .WithOne()
            .HasForeignKey(item => item.ShoppingListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
