

// ReSharper disable once CheckNamespace
namespace ToSic.Eav.Run;

/// <summary>
/// This interface is used in the Dnn RazorComponent of v10, so we must still support it.
/// The only use case is in an overridable CustomizeSearch, so it is never really called,
/// but just defined by a razor page.
/// </summary>
[PrivateApi("Obsolete")]
[Obsolete("this was replaced by IModule")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IContainer;