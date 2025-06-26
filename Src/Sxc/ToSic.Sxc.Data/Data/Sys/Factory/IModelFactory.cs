namespace ToSic.Sxc.Data.Sys.Factory;

/// <summary>
/// An object which can create custom models.
/// </summary>
public interface IModelFactory
{
    [return: NotNullIfNotNull(nameof(item))]
    TCustom? AsCustomFrom<TCustom, TData>(TData? item, ConvertItemSettings? settings = default)
        where TCustom : class, ICanWrapData;
}