using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.LookUp;
using ToSic.Lib.DI;
using ToSic.Lib.Services;

namespace ToSic.Sxc.LookUp;

/// <summary>
/// Generic Lookup Resolver - will provide all lookups which are registered in DI
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class LookUpEngineResolverGeneric : ServiceBase, ILookUpEngineResolver
{
    #region Constructor and DI

    public LookUpEngineResolverGeneric(LazySvc<IEnumerable<ILookUp>> lookUps) : base($"{Constants.SxcLogName}.LookUp")
    {
        _lookUps = lookUps;
    }

    private readonly LazySvc<IEnumerable<ILookUp>> _lookUps;

    #endregion

    public ILookUpEngine GetLookUpEngine(int instanceId)
    {
        var luEngine = new LookUpEngine(Log);
        luEngine.Add(_lookUps.Value.ToList());
        return luEngine;
    }
}