using ToSic.Lib.Documentation;

// ReSharper disable once CheckNamespace
namespace Custom.Oqtane;

/// <summary>
/// The base class for Razor files in Oqtane.
/// 
/// As of 2sxc 12.0 it's identical to [](xref:Custom.Hybrid.Razor12), but in future it may have some more Oqtane specific features. 
/// </summary>
[PublicApi]
public abstract class Razor12: Razor12<object>;