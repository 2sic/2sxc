namespace ToSic.Sxc.Context;

/// <summary>
/// Factory to create / lookup module information.
/// </summary>
[PrivateApi, ShowApiWhenReleased(ShowApiMode.Never)]
public interface IModuleFactory
{
    public IModule Get(int moduleId);
}
