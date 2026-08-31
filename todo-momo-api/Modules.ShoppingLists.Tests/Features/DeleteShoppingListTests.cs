using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Modules.ShoppingLists.Domain;
using Modules.ShoppingLists.Features;
using todo_momo_api.common.Constants;

namespace Modules.ShoppingLists.Tests.Features;

/// <summary>
/// Tests for the <see cref="DeleteShoppingList"/> vertical slice.
/// </summary>
public class DeleteShoppingListTests
{
    [Test]
    public async Task Handle_WhenListExists_SoftDeletesListAndItems()
    {
        await using var context = TestContextFactory.Create();
        var list = new ShoppingList { Id = Guid.NewGuid(), Title = "L", Status = BusinessObjectStatus.Active };
        var item = new ShoppingListItem { Id = Guid.NewGuid(), ShoppingListId = list.Id, Title = "I", Status = BusinessObjectStatus.Active };
        list.Items.Add(item);
        context.ShoppingLists.Add(list);
        await context.SaveChangesAsync();

        var handler = new DeleteShoppingList.Handler(context, NullLogger<DeleteShoppingList.Handler>.Instance);

        var result = await handler.Handle(new DeleteShoppingList.DeleteShoppingListCommand(list.Id), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsTrue();
        var storedList = await context.ShoppingLists.SingleAsync();
        await Assert.That(storedList.Status).IsEqualTo(BusinessObjectStatus.Deleted);
        var storedItem = await context.ShoppingListItems.SingleAsync();
        await Assert.That(storedItem.Status).IsEqualTo(BusinessObjectStatus.Deleted);
    }

    [Test]
    public async Task Handle_WhenListDoesNotExist_ReturnsNotFoundFailure()
    {
        await using var context = TestContextFactory.Create();
        var handler = new DeleteShoppingList.Handler(context, NullLogger<DeleteShoppingList.Handler>.Instance);

        var result = await handler.Handle(new DeleteShoppingList.DeleteShoppingListCommand(Guid.NewGuid()), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Error.Code).IsEqualTo("ShoppingList.NotFound");
    }

    [Test]
    public async Task Handle_WhenListAlreadyDeleted_ReturnsNotFoundFailure()
    {
        await using var context = TestContextFactory.Create();
        var list = new ShoppingList { Id = Guid.NewGuid(), Title = "L", Status = BusinessObjectStatus.Deleted };
        context.ShoppingLists.Add(list);
        await context.SaveChangesAsync();

        var handler = new DeleteShoppingList.Handler(context, NullLogger<DeleteShoppingList.Handler>.Instance);

        var result = await handler.Handle(new DeleteShoppingList.DeleteShoppingListCommand(list.Id), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Error.Code).IsEqualTo("ShoppingList.NotFound");
    }
}
