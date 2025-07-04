﻿using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ToSic.Lib.Coding;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Razor;
using IHasLog = ToSic.Lib.Logging.IHasLog;
using ILog = ToSic.Lib.Logging.ILog;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Sys.ExecutionContext;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

[PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class OqtRazorBase<TModel>: Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>, IHasCodeLog, IHasLog, ISetDynamicModel, IGetCodePath
{
    #region Constructor / DI / SysHelp

    /// <summary>
    /// Constructor - only available for inheritance
    /// </summary>
    [PrivateApi]
    protected OqtRazorBase(int compatibilityLevel, string logName)
    {
        CompatibilityLevel = compatibilityLevel;
        RzrHlp = new(this);
        //Log.Rename(logName);
    }

    [PrivateApi] public int CompatibilityLevel { get; }


    /// <summary>
    /// Special helper to move all Razor logic into a separate class.
    /// For architecture of Composition over Inheritance.
    /// </summary>
    [PrivateApi]
    internal OqtRazorHelper<TModel> RzrHlp { get; }

    #endregion

    #region GetService / Logs / DevTools

    /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class
        => RzrHlp.ExCtxRoot.GetTypedApi().GetService<TService>();

    [PrivateApi("WIP 17.06,x")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public TService GetService<TService>(NoParamOrder protector = default, string? typeName = default) where TService : class
        => RzrHlp.CodeHelper.GetService<TService>(protector, typeName);


    /// <inheritdoc cref="IHasCodeLog.Log" />
    public ICodeLog Log => RzrHlp.CodeLog;

    [PrivateApi] ILog IHasLog.Log => RzrHlp.Log;

    [PrivateApi("Not yet ready")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public IDevTools DevTools => RzrHlp.ExCtxRoot.GetTypedApi().DevTools;

    #endregion

    #region DynCode Root

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    internal IExecutionContext ExCtx => RzrHlp.ExCtxRoot!;

    [PrivateApi]
    public void ConnectToRoot(IExecutionContext exCtx)
        => RzrHlp.ConnectToRoot(exCtx);

    [RazorInject]
    [PrivateApi]
    public new ViewDataDictionary<TModel> ViewData
    {
        get => base.ViewData;
        set => base.ViewData = RzrHlp.HandleViewDataInject(value);
    }

    #endregion

    #region Dynamic Model

    void ISetDynamicModel.SetDynamicModel(object data) => RzrHlp.SetDynamicModel(data);

    #endregion



    #region CreateInstance

    /// <inheritdoc cref="ICreateInstance.CreateInstancePath"/>
    [PrivateApi]
    // Note: The path for CreateInstance / GetCode - unsure if this is actually used anywhere on this object
    string IGetCodePath.CreateInstancePath
    {
        get => field ?? Path;
        set;
    }

    #endregion





}