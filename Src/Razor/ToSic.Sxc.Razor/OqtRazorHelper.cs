using System.Diagnostics.CodeAnalysis;
using Custom.Hybrid;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ToSic.Eav.Environment.Sys.ServerPaths;
using ToSic.Sxc.Code.Sys.CodeRunHelpers;
using ToSic.Sxc.Data.Sys.Wrappers;
using ToSic.Sxc.Engines.Sys;
using ToSic.Sxc.Render.Sys.Specs;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Razor;

internal class OqtRazorHelper<TModel>(OqtRazorBase<TModel> owner) : RazorHelperBase("Oqt.RzrHlp"), ISetDynamicModel
{
    #region DynamicCode Attachment / Handling through ViewData

    public override void ConnectToRoot(IExecutionContext codeRoot)
    {
        base.ConnectToRoot(codeRoot);
        _exCtx = codeRoot;
        owner.LinkLog(codeRoot.Log);
        Log.A("OqtRazorHelper connect Log");
    }

    private const string DynCode = "_dynCode";

    public IExecutionContext ExCtxRoot
    {
        get
        {
            // 2026-05-23 2dm - original code, looks buggy because it would often return null, which should never happen IMHO
            //// Child razor page will have _dynCode == null, so it is provided via ViewData from parent razor page.
            //if (_dynCode != null || owner.ViewData?[DynCode] is not IExecutionContext cdRt)
            //    return _dynCode;

            if (_exCtx != null)
                return _exCtx;
            
            // Child razor page will have _dynCode == null, so it is provided via ViewData from parent razor page.
            // changed 2025-06-23 2dm to throw if null, because I don't think it's correct
            // If I'm wrong, pls discuss.
            if (owner.ViewData?[DynCode] is not IExecutionContext cdRt)
                throw new InvalidOperationException(
                    "OqtRazorHelper: ExCtxRoot is null, and ViewData does not contain _dynCode. " +
                    "This means the parent razor page did not set the _dynCode in ViewData, which is required for child razor pages.");

            // Connect to root, which also sets the _exCtx field
            ConnectToRoot(cdRt);
            Log.A( "DynCode attached from ViewData");

            return _exCtx!;
        }
    }
    private IExecutionContext? _exCtx { get; set; }

    [return: NotNullIfNotNull(nameof(value))]
    public ViewDataDictionary<TModel>? HandleViewDataInject(ViewDataDictionary<TModel>? value)
    {
        // Store _dynCode in ViewData, for child razor page.
        if (_exCtx != null && value != null && value[DynCode] == null)
            value[DynCode] = _exCtx;
        return value;
    }

    internal new IExecutionContext ExCtx => ExCtxOrNull ?? ExCtxRoot!;

    #endregion

    #region Dynamic Model / MyModel

    [field: AllowNull, MaybeNull]
    public dynamic DynamicModel => field ??= owner.GetService<ICodeDataPoCoWrapperService>()
        .DynamicFromObject(_overridePageData ?? owner.Model!, WrapperSettings.Dyn(false, false));

    private object? _overridePageData;

    public void SetDynamicModel(RenderSpecs viewData)
    {
        var l = Log.Fn();
        _overridePageData = viewData.Data;
        l.Done();
    }

    [field: AllowNull, MaybeNull]
    public TypedCode16Helper CodeHelper
        => field ??= CreateCodeHelper();

    private TypedCode16Helper CreateCodeHelper()
        => new(
            helperSpecs: new(ExCtx, true, owner.Path),
            getRazorModel: () => _overridePageData ?? owner.Model,
            getModelDic: () => (_overridePageData ?? owner.Model)?.ToDicInvariantInsensitive()!
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
        var pathFinder = ExCtx.GetService<IServerPaths>();
        var fullPath = pathFinder.FullAppPath(path);
        return l.ReturnAndLog(fullPath);
    }

    #endregion
}