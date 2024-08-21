namespace ToSic.Sxc.Data;

/// <summary>
/// Experimental 2dm - not done yet 2024-08
/// 2dm Experimental - trying to get the lifecycle info into ITypedItem
/// 
/// Idea is to provide versioning information for items - different for original item, latest item, etc.
///
/// See IVersion / ILifecycle - not yet in use.
/// </summary>
internal interface ILifecycle
{
    public IVersion Created { get; }
    public IVersion Modified { get; }
}