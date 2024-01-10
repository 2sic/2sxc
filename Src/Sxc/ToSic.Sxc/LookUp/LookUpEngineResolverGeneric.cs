using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.LookUp;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.LookUp;

/// <summary>
/// Generic Lookup Resolver - will provide all lookups which are registered in DI
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class LookUpEngineResolverGeneric(LazySvc<IEnumerable<ILookUp>> lookUps)
    : ServiceBase($"{SxcLogging.SxcLogName}.LookUp"), ILookUpEngineResolver
{
    #region Constructor and DI

    #endregion

    public ILookUpEngine GetLookUpEngine(int instanceId)
    {
        var luEngine = new LookUpEngine(Log);
        luEngine.Add(lookUps.Value.ToList());
        return luEngine;
    }
}