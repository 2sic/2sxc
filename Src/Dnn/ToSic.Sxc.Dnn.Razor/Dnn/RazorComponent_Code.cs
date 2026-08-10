namespace ToSic.Sxc.Dnn;

abstract partial class RazorComponent
{
    #region Code Behind - a Dnn feature which won't exist in Oqtane

    [PrivateApi]
    internal RazorCodeManager CodeManager => field ??= new(this, Log?.GetContents());

    /// <inheritdoc />
    public dynamic Code => field ??= CodeManager.GetCodeOrException(RzrGetCodeHlp);

    #endregion

}