using Microsoft.Extensions.Logging.Abstractions;
using Modules.ShoppingLists.Domain;
using Modules.ShoppingLists.Features;
using todo_momo_api.common.Constants;

namespace Modules.ShoppingLists.Tests.Features;

/// <summary>
/// Tests for the <see cref="GetShoppingListItems"/> vertical slice.
/// </summary>
public class GetShoppingListItemsTests
{
    [Test]
    public async Task Handle_WhenListExists_ReturnsOnlyNonDeletedItemsForList()
    {
        await using var context = TestContextFactory.Create();
        var list = new ShoppingList { Id = Guid.NewGuid(), Title = "L", Status = BusinessObjectStatus.Active };
        var otherList = new ShoppingList { Id = Guid.NewGuid(), Title = "Other", Status = BusinessObjectStatus.Active };
        context.ShoppingLists.AddRange(list, otherList);
        context.ShoppingListItems.AddRange(
            new ShoppingListItem { Id = Guid.NewGuid(), ShoppingListId = list.Id, Title = "Keep", Status = BusinessObjectStatus.Active },
            new ShoppingListItem { Id = Guid.NewGuid(), ShoppingListId = list.Id, Title = "Deleted", Status = BusinessObjectStatus.Deleted },
            new ShoppingListItem { Id = Guid.NewGuid(), ShoppingListId = otherList.Id, Title = "Other item", Status = BusinessObjectStatus.Active });
        await context.SaveChangesAsync();

        var handler = new GetShoppingListItems.Handler(context, NullLogger<GetShoppingListItems.Handler>.Instance);

        var result = await handler.Handle(new GetShoppingListItems.Query(list.Id), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsTrue();
        var single = result.Value.Single();
        await Assert.That(single.Title).IsEqualTo("Keep");
    }

    [Test]
    public async Task Handle_WhenListDoesNotExist_ReturnsNotFoundFailure()
    {
        await using var context = TestContextFactory.Create();
        var handler = new GetShoppingListItems.Handler(context, NullLogger<GetShoppingListItems.Handler>.Instance);

        var result = await handler.Handle(new GetShoppingListItems.Query(Guid.NewGuid()), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Error.Code).IsEqualTo("ShoppingList.NotFound");
    }

    [Test]
    public async Task Handle_WhenListHasNoItems_ReturnsSuccessWithEmptyCollection()
    {
        await using var context = TestContextFactory.Create();
        var list = new ShoppingList { Id = Guid.NewGuid(), Title = "L", Status = BusinessObjectStatus.Active };
        context.ShoppingLists.Add(list);
        await context.SaveChangesAsync();

        var handler = new GetShoppingListItems.Handler(context, NullLogger<GetShoppingListItems.Handler>.Instance);

        var result = await handler.Handle(new GetShoppingListItems.Query(list.Id), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value).IsEmpty();
    }
}
