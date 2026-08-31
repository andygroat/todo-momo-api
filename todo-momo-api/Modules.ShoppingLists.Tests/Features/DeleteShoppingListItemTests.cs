using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Modules.ShoppingLists.Domain;
using Modules.ShoppingLists.Features;
using todo_momo_api.common.Constants;

namespace Modules.ShoppingLists.Tests.Features;

/// <summary>
/// Tests for the <see cref="DeleteShoppingListItem"/> vertical slice.
/// </summary>
public class DeleteShoppingListItemTests
{
    [Test]
    public async Task Handle_WhenItemExists_SoftDeletesItemAndReturnsSuccess()
    {
        await using var context = TestContextFactory.Create();
        var list = new ShoppingList { Id = Guid.NewGuid(), Title = "L", Status = BusinessObjectStatus.Active };
        var item = new ShoppingListItem { Id = Guid.NewGuid(), ShoppingListId = list.Id, Title = "Milk", Status = BusinessObjectStatus.Active };
        context.ShoppingLists.Add(list);
        context.ShoppingListItems.Add(item);
        await context.SaveChangesAsync();

        var handler = new DeleteShoppingListItem.Handler(context, NullLogger<DeleteShoppingListItem.Handler>.Instance);

        var result = await handler.Handle(new DeleteShoppingListItem.DeleteShoppingListItemCommand(list.Id, item.Id), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsTrue();
        var stored = await context.ShoppingListItems.SingleAsync();
        await Assert.That(stored.Status).IsEqualTo(BusinessObjectStatus.Deleted);
    }

    [Test]
    public async Task Handle_WhenListDoesNotExist_ReturnsListNotFoundFailure()
    {
        await using var context = TestContextFactory.Create();
        var handler = new DeleteShoppingListItem.Handler(context, NullLogger<DeleteShoppingListItem.Handler>.Instance);

        var result = await handler.Handle(new DeleteShoppingListItem.DeleteShoppingListItemCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

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

        var handler = new DeleteShoppingListItem.Handler(context, NullLogger<DeleteShoppingListItem.Handler>.Instance);

        var result = await handler.Handle(new DeleteShoppingListItem.DeleteShoppingListItemCommand(list.Id, Guid.NewGuid()), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Error.Code).IsEqualTo("ShoppingListItem.NotFound");
    }

    [Test]
    public async Task Handle_WhenItemAlreadyDeleted_ReturnsItemNotFoundFailure()
    {
        await using var context = TestContextFactory.Create();
        var list = new ShoppingList { Id = Guid.NewGuid(), Title = "L", Status = BusinessObjectStatus.Active };
        var item = new ShoppingListItem { Id = Guid.NewGuid(), ShoppingListId = list.Id, Title = "Milk", Status = BusinessObjectStatus.Deleted };
        context.ShoppingLists.Add(list);
        context.ShoppingListItems.Add(item);
        await context.SaveChangesAsync();

        var handler = new DeleteShoppingListItem.Handler(context, NullLogger<DeleteShoppingListItem.Handler>.Instance);

        var result = await handler.Handle(new DeleteShoppingListItem.DeleteShoppingListItemCommand(list.Id, item.Id), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Error.Code).IsEqualTo("ShoppingListItem.NotFound");
    }
}
