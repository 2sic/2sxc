using ToSic.Lib.DI;
using ToSic.Lib.LookUp;

namespace ToSic.Sxc.LookUp.Internal;

/// <summary>
/// Resolves / builds the LookUp Engine.
/// This is the default implementation and used in Oqtane.
/// Dnn has its own implementation.
/// </summary>
/// <param name="builtInSources"></param>
internal class LookUpEngineResolver(LazySvc<IEnumerable<ILookUp>> builtInSources)
    : LookUpEngineResolverBase(builtInSources, $"{SxcLogName}.LUpEnR");