namespace ToSic.Sxc.Dnn;

abstract partial class RazorComponent
{
    #region Code Behind - a Dnn feature which probably won't exist in Oqtane

    [PrivateApi]
    internal RazorCodeManager CodeManager => field ??= new(this, Log?.GetContents());

    /// <inheritdoc />
    public dynamic Code => CodeManager.CodeOrException;

    #endregion

}