namespace ToSic.Sxc.Data.Internal;

/// <summary>
/// An object which can create custom models.
/// </summary>
public interface IModelFactory
{
    TCustom AsCustomFrom<TCustom, TData>(TData item)
        where TCustom : class, ICanWrapData;
}