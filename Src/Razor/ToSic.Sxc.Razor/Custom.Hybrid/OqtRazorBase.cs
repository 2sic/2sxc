﻿using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Razor;
using IHasLog = ToSic.Lib.Logging.IHasLog;
using ILog = ToSic.Lib.Logging.ILog;
using ToSic.Sxc.Engines;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

[PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
        SysHlp = new(this);
        //Log.Rename(logName);
    }

    [PrivateApi] public int CompatibilityLevel { get; }

    /// <summary>
    /// Special helper to move all Razor logic into a separate class.
    /// For architecture of Composition over Inheritance.
    /// </summary>
    [PrivateApi]
    internal OqtRazorHelper<TModel> SysHlp { get; }

    #endregion

    #region GetService / Logs / DevTools

    /// <inheritdoc cref="ToSic.Eav.Code.ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => _CodeApiSvc.GetService<TService>();

    /// <inheritdoc cref="IHasCodeLog.Log" />
    public ICodeLog Log => SysHlp.CodeLog;

    [PrivateApi] ILog IHasLog.Log => SysHlp.Log;

    [PrivateApi("Not yet ready")]
    public IDevTools DevTools => _CodeApiSvc.DevTools;

    #endregion

    #region DynCode Root

    [PrivateApi]
    public ICodeApiService _CodeApiSvc => SysHlp.DynCodeRootMain;

    [PrivateApi]
    public void ConnectToRoot(ICodeApiService parent) => SysHlp.ConnectToRoot(parent);

    [RazorInject]
    [PrivateApi]
    public new ViewDataDictionary<TModel> ViewData
    {
        get => base.ViewData;
        set => base.ViewData = SysHlp.HandleViewDataInject(value);
    }

    #endregion

    #region Dynamic Model

    void ISetDynamicModel.SetDynamicModel(object data) => SysHlp.SetDynamicModel(data);

    #endregion



    #region CreateInstance

    /// <inheritdoc cref="ICreateInstance.CreateInstancePath"/>
    [PrivateApi]
    // Note: The path for CreateInstance / GetCode - unsure if this is actually used anywhere on this object
    string IGetCodePath.CreateInstancePath
    {
        get => _createInstancePath ?? Path;
        set => _createInstancePath = value;
    }
    private string _createInstancePath;
        
    #endregion





}