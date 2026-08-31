using Modules.ShoppingLists.Infrastructure.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using todo_momo_api.common.Domain;

namespace Modules.ShoppingLists.Domain;

/// <summary>
/// Represents a shopping list containing multiple <see cref="ShoppingListItem"/> entries.
/// </summary>
[ExcludeFromCodeCoverage]
[Table("ShoppingLists", Schema = Schemas.Default)]
public sealed class ShoppingList : BusinessObject
{
    /// <summary>
    /// Gets or sets the title of the shopping list.
    /// </summary>
    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the items that belong to this shopping list.
    /// </summary>
    public ICollection<ShoppingListItem> Items { get; set; } = new List<ShoppingListItem>();
}
