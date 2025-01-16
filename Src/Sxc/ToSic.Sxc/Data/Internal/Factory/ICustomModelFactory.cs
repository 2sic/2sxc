using ToSic.Sxc.Models;

namespace ToSic.Sxc.Data.Internal;

/// <summary>
/// An object which can create custom models.
/// </summary>
public interface ICustomModelFactory
{
    TCustom AsCustomFrom2<TCustom, TData>(TData item)
        where TCustom : class, IDataModel;
}