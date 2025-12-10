namespace ToSic.Sxc.Code;

/// <summary>
/// WIP!!!
///
/// This should provide special APIs to assist developers.
/// It will probably change from version to version, so the use should be limited to quick debugs and similar,
/// but never remain in the code.
/// </summary>
[WorkInProgressApi("Not yet in use")]
public interface IDevTools
{
    /// <summary>
    /// Enable debugging on a specific object, if it supports it.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="debug"></param>
    void Debug(object target, NoParamOrder npo = default, bool debug = true);
}