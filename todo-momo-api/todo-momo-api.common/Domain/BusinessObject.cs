using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using todo_momo_api.common.Constants;

namespace todo_momo_api.common.Domain;

/// <summary>
/// Base business object class to provide the base properties that all
/// business objects should have.
/// </summary>
public abstract class BusinessObject
{
    /// <summary>
    /// The business object's unique id.
    /// </summary>
    [Required, Key]
    public virtual Guid Id { get; set; }

    /// <summary>
    /// The business object's status.
    /// </summary>
    [Required]
    public virtual BusinessObjectStatus Status { get; set; }
}
