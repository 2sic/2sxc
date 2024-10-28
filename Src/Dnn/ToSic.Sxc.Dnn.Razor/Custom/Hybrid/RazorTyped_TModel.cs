// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

/// <summary>
/// New base class with custom model.
/// The rest of this is identical to <see cref="RazorTyped"/>.
/// </summary>
/// <typeparam name="TModel">Model type - like `string` or a class from your `AppCode`</typeparam>
/// <remarks>
/// Introduced in v17.03
/// </remarks>
[PublicApi]
public abstract class RazorTyped<TModel> : RazorTyped
{
    /// <summary>
    /// The model for this Razor file.
    /// Typed according to the `@inherits` statement.
    /// </summary>
    public TModel Model => CodeHelper.GetModel<TModel>();
}