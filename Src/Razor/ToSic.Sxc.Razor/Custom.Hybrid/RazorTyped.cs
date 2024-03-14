using ToSic.Lib.Documentation;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;


[PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
// ReSharper disable once UnusedMember.Global

public abstract class RazorTyped : RazorTyped<dynamic> { }