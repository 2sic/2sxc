using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Dnn;

abstract partial class RazorComponent
{
    #region Code Behind - a Dnn feature which probably won't exist in Oqtane

    [PrivateApi]
    internal RazorCodeManager CodeManager => _codeManager ??= new RazorCodeManager(this, Log?.GetContents());
    private RazorCodeManager _codeManager;

    /// <inheritdoc />
    public dynamic Code => CodeManager.CodeOrException;

    #endregion

}