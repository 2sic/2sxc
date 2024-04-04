using System.Web.Http.Controllers;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;

namespace ToSic.Sxc.Dnn.WebApi.Internal;

/// <summary>
/// This is the foundation for both the old SxcApiController and the new Dnn.ApiController.
/// incl. the current App, DNN, Data, etc.
/// For others, please use the SxiApiControllerBase, which doesn't have all that, and is usually then
/// safer because it can't accidentally mix the App with a different appId in the params
/// </summary>
[PrivateApi("This is an internal base class used for the App ApiControllers. Make sure the implementations don't break")]
// Note: 2022-02 2dm I'm not sure if this was ever published as the official api controller, but it may have been?
[DnnLogExceptions]
// Can't hide in Intellisense, because that would hide it for all derived classes too
// [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
[method: PrivateApi]
public abstract class DnnSxcCustomControllerBase(string logSuffix, string insightsGroup = default)
    : DnnSxcControllerBase(logSuffix, insightsGroup), IHasCodeApiService
{
    #region Constructor & DI / Setup

    /// <summary>
    /// Empty constructor is important for inheriting classes
    /// </summary>
    [PrivateApi]
    protected DnnSxcCustomControllerBase() : this("DynApi") { }

    [PrivateApi]
    protected override void Initialize(HttpControllerContext controllerContext)
    {
        base.Initialize(controllerContext);
        var init = DynHlp.Initialize(controllerContext);
        if (this is IGetCodePath thisWithPath)
            thisWithPath.CreateInstancePath = init.Folder;
        _CodeApiSvc = init.Root;
    }

    #endregion

    #region Internal / Plumbing / Obsolete

    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public ICodeApiService _CodeApiSvc { get; private set; }


    /// <summary>
    /// The name of the logger in insights. The inheriting class should provide the real name to be used.
    /// Note: Probably almost never used, except by 2sic. Must determine if we just remove it
    /// </summary>
    [Obsolete("Deprecated in v13.03 - doesn't serve a purpose any more. Will just remain to avoid breaking public uses of this property.")]
    // ReSharper disable once UnassignedGetOnlyAutoProperty
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    protected virtual string HistoryLogName { get; }

    #endregion

}