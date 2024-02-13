using ToSic.Eav.LookUp;
using ToSic.Lib.DI;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.LookUp.Internal;

/// <summary>
/// Resolves / builds the LookUp Engine.
/// This is the default implementation and used in Oqtane.
/// Dnn has its own implementation.
/// </summary>
/// <param name="lookUps"></param>
internal class LookUpEngineResolver(LazySvc<IEnumerable<ILookUp>> lookUps)
    : LookUpEngineResolverBase(lookUps, $"{SxcLogging.SxcLogName}.LUpEnR");