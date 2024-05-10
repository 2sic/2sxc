using Oqtane.Models;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Oqt.Server.Context;
using static ToSic.Sxc.LookUp.LookUpConstants;

namespace ToSic.Sxc.Oqt.Server.LookUps;

internal class OqtModuleLookUp(ISxcContextResolver ctxResolver) : LookUpBase(SourceModule, "LookUp in Oqtane Module")
{
    private Module Module { get; set; }

    public Module GetSource()
    {
        if (_alreadyTried) return null;
        _alreadyTried = true;
        var ctx = ctxResolver.BlockContextOrNull();
        var module = (OqtModule)ctx?.Module;
        return module?.GetContents();
    }

    private bool _alreadyTried;

    public override string Get(string key, string format)
    {
        try
        {
            Module ??= GetSource();

            if (Module == null) return null;

            return key.ToLowerInvariant() switch
            {
                KeyId => $"{Module.ModuleId}",
                OldDnnModuleId => $"Warning: '{OldDnnModuleId}' was requested, but the {nameof(OqtModuleLookUp)} source can only answer to '{KeyId}'",
                _ => string.Empty
            };
        }
        catch
        {
            return string.Empty;
        }
    }
}