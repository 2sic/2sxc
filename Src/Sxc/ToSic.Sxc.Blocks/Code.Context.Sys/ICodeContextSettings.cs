using ToSic.Sxc.Data;

namespace ToSic.Sxc.Code.Context.Sys;

/// <summary>
/// Mini-Injectable for systems which just need the current settings injected
/// </summary>
public interface ICodeContextSettings
{
    ITypedStack AllSettings { get; }
}
