using Microsoft.EntityFrameworkCore;
using Modules.ShoppingLists.Infrastructure.Database;

namespace Modules.ShoppingLists.Tests;

/// <summary>
/// Helper for creating isolated in-memory <see cref="ShoppingListDbContext"/> instances per test.
/// </summary>
internal static class TestContextFactory
{
    public static ShoppingListDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ShoppingListDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ShoppingListDbContext(options);
    }
}
