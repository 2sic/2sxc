using ToSic.Sxc.Dnn.Code;

// ReSharper disable UnusedMember.Global

namespace ToSic.Sxc.Dnn.WebApi;

/// <summary>
/// This interface extends the IAppAndDataHelpers with the DNN Context.
/// It's important, because if 2sxc also runs on other CMS platforms, then the Dnn Context won't be available, so it's in a separate interface.
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IDnnDynamicWebApi : IDynamicWebApi, IDnnDynamicCode
{

}