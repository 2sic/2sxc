using Custom.Hybrid;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.IO;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.Data.Internal.Wrapper;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Razor;

internal class OqtRazorHelper<TModel>(OqtRazorBase<TModel> owner) : RazorHelperBase("Oqt.RzrHlp"), ISetDynamicModel
{
    #region DynamicCode Attachment / Handling through ViewData

    public override void ConnectToRoot(ICodeApiService codeRoot)
    {
        base.ConnectToRoot(codeRoot);
        _dynCode = codeRoot;
        owner.LinkLog(codeRoot.Log);
        Log.A("OqtRazorHelper connect Log");
    }

    private const string DynCode = "_dynCode";

    public ICodeApiService DynCodeRootMain
    {
        get
        {
            // Child razor page will have _dynCode == null, so it is provided via ViewData from parent razor page.
            if (_dynCode != null || owner.ViewData?[DynCode] is not ICodeApiService cdRt) return _dynCode;
            ConnectToRoot(cdRt);
            Log.A( "DynCode attached from ViewData");

            return _dynCode;
        }
    }
    private ICodeApiService _dynCode;

    public ViewDataDictionary<TModel> HandleViewDataInject(ViewDataDictionary<TModel> value)
    {
        // Store _dynCode in ViewData, for child razor page.
        if (_dynCode != null && value != null && value[DynCode] == null)
            value[DynCode] = _dynCode;
        return value;
    }

    // ReSharper disable once InconsistentNaming
    public override ICodeApiService _CodeApiSvc => base._CodeApiSvc ?? DynCodeRootMain;

    #endregion

    #region Dynamic Model / MyModel

    public dynamic DynamicModel => _dynamicModel ??= owner.GetService<CodeDataWrapper>()
        .FromObject(_overridePageData ?? owner.Model, WrapperSettings.Dyn(false, false));
    private dynamic _dynamicModel;
    private object _overridePageData;

    public void SetDynamicModel(object data) => _overridePageData = data;

    public TypedCode16Helper CodeHelper => _codeHelper ??= CreateCodeHelper();
    private TypedCode16Helper _codeHelper;

    private TypedCode16Helper CreateCodeHelper() =>
        new(
            owner: this,
            helperSpecs: new(_CodeApiSvc, true, owner.Path),
            getRazorModel: () => _overridePageData ?? owner.Model,
            getModelDic: () => (_overridePageData ?? owner.Model)?.ToDicInvariantInsensitive()
        );

    #endregion

    #region GetCode / Create Instance

    protected override string GetCodeNormalizePath(string virtualPath)
    {
        var directory = Path.GetDirectoryName(owner.Path)
                        ?? throw new("Current directory seems to be null");
        return Path.Combine(directory, virtualPath);
    }

    /// <summary>
    /// Cshtml CreateInstance - just throw error, as not supported in Oqtane
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    protected override object GetCodeCshtml(string path) =>
        throw new NotSupportedException(
            "CreateInstance with .cshtml files is not supported in Oqtane. Use a .cs file instead.");


    protected override string GetCodeFullPathForExistsCheck(string path)
    {
        var l = Log.Fn<string>(path);
        var pathFinder = _CodeApiSvc.GetService<IServerPaths>();
        var fullPath = pathFinder.FullAppPath(path);
        return l.ReturnAndLog(fullPath);
    }

    #endregion
}