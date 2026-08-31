using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Modules.ShoppingLists.Domain;
using Modules.ShoppingLists.Features;
using todo_momo_api.common.Constants;

namespace Modules.ShoppingLists.Tests.Features;

/// <summary>
/// Tests for the <see cref="UpdateShoppingListItem"/> vertical slice.
/// </summary>
public class UpdateShoppingListItemTests
{
    [Test]
    public async Task Handle_WhenItemExists_UpdatesTitleAndCompletionAndReturnsSuccess()
    {
        await using var context = TestContextFactory.Create();
        var list = new ShoppingList { Id = Guid.NewGuid(), Title = "L", Status = BusinessObjectStatus.Active };
        var item = new ShoppingListItem { Id = Guid.NewGuid(), ShoppingListId = list.Id, Title = "Old", IsComplete = false, Status = BusinessObjectStatus.Active };
        context.ShoppingLists.Add(list);
        context.ShoppingListItems.Add(item);
        await context.SaveChangesAsync();

        var handler = new UpdateShoppingListItem.Handler(context, NullLogger<UpdateShoppingListItem.Handler>.Instance);

        var result = await handler.Handle(new UpdateShoppingListItem.Command(list.Id, item.Id, "New", true), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsTrue();
        var stored = await context.ShoppingListItems.SingleAsync();
        await Assert.That(stored.Title).IsEqualTo("New");
        await Assert.That(stored.IsComplete).IsTrue();
    }

    [Test]
    public async Task Handle_WhenListDoesNotExist_ReturnsListNotFoundFailure()
    {
        await using var context = TestContextFactory.Create();
        var handler = new UpdateShoppingListItem.Handler(context, NullLogger<UpdateShoppingListItem.Handler>.Instance);

        var result = await handler.Handle(new UpdateShoppingListItem.Command(Guid.NewGuid(), Guid.NewGuid(), "New", true), CancellationToken.None);

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

        var handler = new UpdateShoppingListItem.Handler(context, NullLogger<UpdateShoppingListItem.Handler>.Instance);

        var result = await handler.Handle(new UpdateShoppingListItem.Command(list.Id, Guid.NewGuid(), "New", true), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Error.Code).IsEqualTo("ShoppingListItem.NotFound");
    }
}

/// <summary>
/// Tests for the <see cref="UpdateShoppingListItem.Validator"/> FluentValidation rules.
/// </summary>
public class UpdateShoppingListItemValidatorTests
{
    [Test]
    public async Task Validator_WhenTitleIsEmpty_FailsValidation()
    {
        var validator = new UpdateShoppingListItem.Validator();

        var result = await validator.ValidateAsync(new UpdateShoppingListItem.UpdateShoppingListItemCommand(string.Empty, false));

        await Assert.That(result.IsValid).IsFalse();
    }

    [Test]
    public async Task Validator_WhenTitleExceedsMaxLength_FailsValidation()
    {
        var validator = new UpdateShoppingListItem.Validator();

        var result = await validator.ValidateAsync(new UpdateShoppingListItem.UpdateShoppingListItemCommand(new string('x', 201), false));

        await Assert.That(result.IsValid).IsFalse();
    }

    [Test]
    public async Task Validator_WhenCommandIsValid_PassesValidation()
    {
        var validator = new UpdateShoppingListItem.Validator();

        var result = await validator.ValidateAsync(new UpdateShoppingListItem.UpdateShoppingListItemCommand("Milk", true));

        await Assert.That(result.IsValid).IsTrue();
    }
}
