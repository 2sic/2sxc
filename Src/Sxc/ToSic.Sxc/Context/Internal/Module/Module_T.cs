using ToSic.Eav.Cms.Internal;
using ToSic.Lib.Data;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Context.Internal;

/// <summary>
/// A base implementation of the block information wrapping the CMS specific object along with it.
/// </summary>
/// <typeparam name="T"></typeparam>
[PrivateApi("this is just fyi")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class Module<T>(string logName) : ServiceBase(logName), IModule, IWrapper<T>
    where T : class
{
    #region Constructors and DI

    public T GetContents() => UnwrappedModule;
    [PrivateApi] protected T UnwrappedModule;

    public IModule Init(T item)
    {
        UnwrappedModule = item;
        return this;
    }

    public abstract IModule Init(int id);
    #endregion

    /// <inheritdoc />
    public abstract int Id { get; }

    /// <inheritdoc />
    public abstract bool IsContent { get; }

    /// <inheritdoc />
    public abstract IBlockIdentifier BlockIdentifier { get; }
}