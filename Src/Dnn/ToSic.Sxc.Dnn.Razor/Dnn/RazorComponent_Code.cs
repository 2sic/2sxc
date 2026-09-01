using ToSic.Sxc.Dnn.Razor.Sys;

namespace ToSic.Sxc.Dnn;

abstract partial class RazorComponent
{
    #region Code Behind - a Dnn feature which won't exist in Oqtane

    [PrivateApi]
    internal DnnRazorCodeBehindHelper CodeBehindHelper => field ??= new(this, Log?.GetContents());

    /// <inheritdoc />
    public dynamic Code => field ??= CodeBehindHelper.GetCodeOrException(RzrGetCodeHlp);

    #endregion

}