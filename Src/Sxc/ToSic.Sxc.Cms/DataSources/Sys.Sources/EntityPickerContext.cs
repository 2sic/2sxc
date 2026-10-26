using ToSic.Eav.Context;
using ToSic.Eav.LookUp.Sources;

namespace ToSic.Sxc.DataSources.Sys.Sources;

/// <summary>
/// Carries the caller's prepared permission context through the query's lookup chain.
/// </summary>
[PrivateApi]
public sealed class EntityPickerContext(IContextOfApp context) : LookUpInDictionary(SourceName)
{
    /// <summary>Lookup source used only to carry the prepared context, not token values.</summary>
    public const string SourceName = "SxcEntityPickerContext";

    /// <summary>Context after the caller has applied the requested app's permission policy.</summary>
    public IContextOfApp Context { get; } = context;
}
