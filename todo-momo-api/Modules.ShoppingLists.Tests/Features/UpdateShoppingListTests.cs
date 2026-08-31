using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Modules.ShoppingLists.Domain;
using Modules.ShoppingLists.Features;
using todo_momo_api.common.Constants;

namespace Modules.ShoppingLists.Tests.Features;

/// <summary>
/// Tests for the <see cref="UpdateShoppingList"/> vertical slice.
/// </summary>
public class UpdateShoppingListTests
{
    [Test]
    public async Task Handle_WhenListExists_UpdatesTitleAndReturnsSuccess()
    {
        await using var context = TestContextFactory.Create();
        var list = new ShoppingList { Id = Guid.NewGuid(), Title = "Old", Status = BusinessObjectStatus.Active };
        context.ShoppingLists.Add(list);
        await context.SaveChangesAsync();

        var handler = new UpdateShoppingList.Handler(context, NullLogger<UpdateShoppingList.Handler>.Instance);

        var result = await handler.Handle(new UpdateShoppingList.Command(list.Id, "New"), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsTrue();
        var stored = await context.ShoppingLists.SingleAsync();
        await Assert.That(stored.Title).IsEqualTo("New");
    }

    [Test]
    public async Task Handle_WhenListDoesNotExist_ReturnsNotFoundFailure()
    {
        await using var context = TestContextFactory.Create();
        var handler = new UpdateShoppingList.Handler(context, NullLogger<UpdateShoppingList.Handler>.Instance);

        var result = await handler.Handle(new UpdateShoppingList.Command(Guid.NewGuid(), "New"), CancellationToken.None);

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

        var handler = new UpdateShoppingList.Handler(context, NullLogger<UpdateShoppingList.Handler>.Instance);

        var result = await handler.Handle(new UpdateShoppingList.Command(list.Id, "New"), CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.Error.Code).IsEqualTo("ShoppingList.NotFound");
    }
}

/// <summary>
/// Tests for the <see cref="UpdateShoppingList.Validator"/> FluentValidation rules.
/// </summary>
public class UpdateShoppingListValidatorTests
{
    [Test]
    public async Task Validator_WhenTitleIsEmpty_FailsValidation()
    {
        var validator = new UpdateShoppingList.Validator();

        var result = await validator.ValidateAsync(new UpdateShoppingList.UpdateShoppingListCommand(string.Empty));

        await Assert.That(result.IsValid).IsFalse();
    }

    [Test]
    public async Task Validator_WhenTitleExceedsMaxLength_FailsValidation()
    {
        var validator = new UpdateShoppingList.Validator();

        var result = await validator.ValidateAsync(new UpdateShoppingList.UpdateShoppingListCommand(new string('x', 101)));

        await Assert.That(result.IsValid).IsFalse();
    }

    [Test]
    public async Task Validator_WhenCommandIsValid_PassesValidation()
    {
        var validator = new UpdateShoppingList.Validator();

        var result = await validator.ValidateAsync(new UpdateShoppingList.UpdateShoppingListCommand("Groceries"));

        await Assert.That(result.IsValid).IsTrue();
    }
}
