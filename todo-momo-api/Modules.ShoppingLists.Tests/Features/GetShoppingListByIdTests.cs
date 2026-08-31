using Microsoft.Extensions.Logging.Abstractions;
using Modules.ShoppingLists.Domain;
using Modules.ShoppingLists.Features;
using todo_momo_api.common.Constants;

namespace Modules.ShoppingLists.Tests.Features;

/// <summary>
/// Tests for the <see cref="GetShoppingListById"/> vertical slice.
/// </summary>
public class GetShoppingListByIdTests
{
    [Test]
    public async Task Handle_WhenListExists_ReturnsSuccessWithMappedResponse()
    {
        await using var context = TestContextFactory.Create();
        var list = new ShoppingList { Id = Guid.NewGuid(), Title = "Find me", Status = BusinessObjectStatus.Active };
        context.ShoppingLists.Add(list);
        await context.SaveChangesAsync();

        var handler = new GetShoppingListById.Handler(context, NullLogger<GetShoppingListById.Handler>.Instance);

        var result = await handler.Handle(new GetShoppingListById.Query(list.Id), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Id).IsEqualTo(list.Id);
        await Assert.That(result.Value.Title).IsEqualTo("Find me");
    }

    [Test]
    public async Task Handle_WhenListDoesNotExist_ReturnsNotFoundFailure()
    {
        await using var context = TestContextFactory.Create();
        var handler = new GetShoppingListById.Handler(context, NullLogger<GetShoppingListById.Handler>.Instance);

        var result = await handler.Handle(new GetShoppingListById.Query(Guid.NewGuid()), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Error.Code).IsEqualTo("ShoppingList.NotFound");
    }

    [Test]
    public async Task Handle_WhenListIsDeleted_ReturnsNotFoundFailure()
    {
        await using var context = TestContextFactory.Create();
        var list = new ShoppingList { Id = Guid.NewGuid(), Title = "Gone", Status = BusinessObjectStatus.Deleted };
        context.ShoppingLists.Add(list);
        await context.SaveChangesAsync();

        var handler = new GetShoppingListById.Handler(context, NullLogger<GetShoppingListById.Handler>.Instance);

        var result = await handler.Handle(new GetShoppingListById.Query(list.Id), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Error.Code).IsEqualTo("ShoppingList.NotFound");
    }
}
