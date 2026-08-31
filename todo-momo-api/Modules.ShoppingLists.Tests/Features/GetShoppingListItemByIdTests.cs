using Microsoft.Extensions.Logging.Abstractions;
using Modules.ShoppingLists.Domain;
using Modules.ShoppingLists.Features;
using todo_momo_api.common.Constants;

namespace Modules.ShoppingLists.Tests.Features;

/// <summary>
/// Tests for the <see cref="GetShoppingListItemById"/> vertical slice.
/// </summary>
public class GetShoppingListItemByIdTests
{
    [Test]
    public async Task Handle_WhenItemExists_ReturnsSuccessWithMappedResponse()
    {
        await using var context = TestContextFactory.Create();
        var list = new ShoppingList { Id = Guid.NewGuid(), Title = "L", Status = BusinessObjectStatus.Active };
        var item = new ShoppingListItem { Id = Guid.NewGuid(), ShoppingListId = list.Id, Title = "Milk", IsComplete = true, Status = BusinessObjectStatus.Active };
        context.ShoppingLists.Add(list);
        context.ShoppingListItems.Add(item);
        await context.SaveChangesAsync();

        var handler = new GetShoppingListItemById.Handler(context, NullLogger<GetShoppingListItemById.Handler>.Instance);

        var result = await handler.Handle(new GetShoppingListItemById.Query(list.Id, item.Id), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Id).IsEqualTo(item.Id);
        await Assert.That(result.Value.Title).IsEqualTo("Milk");
        await Assert.That(result.Value.IsComplete).IsTrue();
    }

    [Test]
    public async Task Handle_WhenListDoesNotExist_ReturnsListNotFoundFailure()
    {
        await using var context = TestContextFactory.Create();
        var handler = new GetShoppingListItemById.Handler(context, NullLogger<GetShoppingListItemById.Handler>.Instance);

        var result = await handler.Handle(new GetShoppingListItemById.Query(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Error.Code).IsEqualTo("ShoppingList.NotFound");
    }

    [Test]
    public async Task Handle_WhenItemDoesNotExist_ReturnsItemNotFoundFailure()
    {
        await using var context = TestContextFactory.Create();
        var list = new ShoppingList { Id = Guid.NewGuid(), Title = "L", Status = BusinessObjectStatus.Active };
        context.ShoppingLists.Add(list);
        await context.SaveChangesAsync();

        var handler = new GetShoppingListItemById.Handler(context, NullLogger<GetShoppingListItemById.Handler>.Instance);

        var result = await handler.Handle(new GetShoppingListItemById.Query(list.Id, Guid.NewGuid()), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Error.Code).IsEqualTo("ShoppingListItem.NotFound");
    }

    [Test]
    public async Task Handle_WhenItemBelongsToDifferentList_ReturnsItemNotFoundFailure()
    {
        await using var context = TestContextFactory.Create();
        var list = new ShoppingList { Id = Guid.NewGuid(), Title = "L", Status = BusinessObjectStatus.Active };
        var otherList = new ShoppingList { Id = Guid.NewGuid(), Title = "Other", Status = BusinessObjectStatus.Active };
        var item = new ShoppingListItem { Id = Guid.NewGuid(), ShoppingListId = otherList.Id, Title = "Milk", Status = BusinessObjectStatus.Active };
        context.ShoppingLists.AddRange(list, otherList);
        context.ShoppingListItems.Add(item);
        await context.SaveChangesAsync();

        var handler = new GetShoppingListItemById.Handler(context, NullLogger<GetShoppingListItemById.Handler>.Instance);

        var result = await handler.Handle(new GetShoppingListItemById.Query(list.Id, item.Id), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Error.Code).IsEqualTo("ShoppingListItem.NotFound");
    }
}
