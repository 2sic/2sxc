namespace ToSic.Sxc.Code;

/// <summary>
/// Special interface to mark all objects which have the current code context.
/// These objects know about the current App, are able to provide ServiceKits and more.
///
/// The interface is used to allow for code in the `AppCode` folder to pass the context to the base class.
/// Thereby enabling things such as the `Kit` or `App` object to magically work.
/// </summary>
[WorkInProgressApi("experimental v17")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IHasCodeContext
{
}