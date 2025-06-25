using ToSic.Eav.Cms.Internal;
using ToSic.Lib.Services;
using ToSic.Lib.Wrappers;

namespace ToSic.Sxc.Context.Sys.Module;

/// <summary>
/// A base implementation of the block information wrapping the CMS specific object along with it.
/// </summary>
/// <typeparam name="T"></typeparam>
[PrivateApi("this is just fyi")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class Module<T>(string logName) : ServiceBase(logName), IModule, IWrapper<T>
    where T : class
{
    #region Constructors and DI

    public T GetContents() => UnwrappedModule;
    [PrivateApi] protected T UnwrappedModule = null!;

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