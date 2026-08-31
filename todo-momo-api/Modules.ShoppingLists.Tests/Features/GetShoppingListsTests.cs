using Microsoft.Extensions.Logging.Abstractions;
using Modules.ShoppingLists.Domain;
using Modules.ShoppingLists.Features;
using todo_momo_api.common.Constants;

namespace Modules.ShoppingLists.Tests.Features;

/// <summary>
/// Tests for the <see cref="GetShoppingLists"/> vertical slice.
/// </summary>
public class GetShoppingListsTests
{
    [Test]
    public async Task Handle_WhenNoLists_ReturnsSuccessWithEmptyCollection()
    {
        await using var context = TestContextFactory.Create();
        var handler = new GetShoppingLists.Handler(context, NullLogger<GetShoppingLists.Handler>.Instance);

        var result = await handler.Handle(new GetShoppingLists.Query(), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value).IsEmpty();
    }

    [Test]
    public async Task Handle_ReturnsOnlyNonDeletedLists()
    {
        await using var context = TestContextFactory.Create();
        var active = new ShoppingList { Id = Guid.NewGuid(), Title = "Active", Status = BusinessObjectStatus.Active };
        var deleted = new ShoppingList { Id = Guid.NewGuid(), Title = "Deleted", Status = BusinessObjectStatus.Deleted };
        context.ShoppingLists.AddRange(active, deleted);
        await context.SaveChangesAsync();

        var handler = new GetShoppingLists.Handler(context, NullLogger<GetShoppingLists.Handler>.Instance);

        var result = await handler.Handle(new GetShoppingLists.Query(), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsTrue();
        var single = result.Value.Single();
        await Assert.That(single.Id).IsEqualTo(active.Id);
        await Assert.That(single.Title).IsEqualTo("Active");
    }
}
