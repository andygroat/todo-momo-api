using System.Diagnostics.CodeAnalysis;

namespace todo_momo_api.common.ResultHelper;

/// <summary>
/// Represents an application error with a code and description.
/// </summary>
/// <param name="Code">The error code.</param>
/// <param name="Description">The error description.</param>
[ExcludeFromCodeCoverage]
public sealed record Error(string Code, string Description)
{
    /// <summary>
    /// Represents no error, with an empty code and description.
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty);
}
