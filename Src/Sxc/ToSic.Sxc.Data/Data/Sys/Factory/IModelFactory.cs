namespace ToSic.Sxc.Data.Sys.Factory;

/// <summary>
/// An object which can create custom models.
/// </summary>
public interface IModelFactory
{
    TCustom AsCustomFrom<TCustom, TData>(TData item)
        where TCustom : class, ICanWrapData;
}