﻿using ToSic.Eav.LookUp;

namespace ToSic.Sxc.LookUp.Sys;

/// <summary>
/// Resolves / builds the LookUp Engine.
/// This is the default implementation and used in Oqtane.
/// Dnn has its own implementation.
/// </summary>
/// <param name="builtInSources"></param>
internal class LookUpEngineResolver(LazySvc<IEnumerable<ILookUp>> builtInSources)
    : LookUpEngineResolverBase(builtInSources, $"{SxcLogName}.LUpEnR");