using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Helpers;
using ToSic.Sxc.Backend.Context;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.WebApi.ActionFilters;
using Log = ToSic.Lib.Logging.Log;

namespace ToSic.Sxc.Oqt.Server.Controllers;

/// <summary>
/// Api controllers normally should inherit ControllerBase but we have a special case of inhering from Controller.
/// It is because our custom dynamic 2sxc app api controllers (without constructor), depends on event OnActionExecuting
/// to provide dependencies (without DI in constructor).
/// </summary>
[SystemTextJsonFormatter] // This is needed to preserve compatibility with previous api usage
[ServiceFilter(typeof(OptionalBodyFilter))] // Instead of global options.AllowEmptyInputInBodyModelBinding = true;
[ServiceFilter(typeof(HttpResponseExceptionFilter))]
public abstract class OqtControllerBase : ControllerBase, IHasLog, IActionFilter
{
    #region Setup

    private readonly bool _withBlockContext;

    protected OqtControllerBase(bool withBlockContext, string logSuffix)
    {
        _withBlockContext = withBlockContext;
        Log = new Log($"Api.{logSuffix}", null, GetType().Name);
        _helper = new(this);

        if (withBlockContext) _ctxHlp = new(this, _helper);
    }

    #endregion


    /// <summary>
    /// The group name for log entries in insights.
    /// Helps group various calls by use case.
    /// </summary>
    protected virtual string HistoryLogGroup => EavWebApiConstants.HistoryNameWebApi;

    /// <inheritdoc />
    public ILog Log { get; }
        
    /// <summary>
    /// The helper to assist in timing and common operations of WebApi Controllers
    /// </summary>
    private readonly NetCoreControllersHelper _helper;

    /// <summary>
    /// Special helper to move all Razor logic into a separate class.
    /// For architecture of Composition over Inheritance.
    /// </summary>
    [PrivateApi]
    internal NetCoreWebApiContextHelper CtxHlp 
        => _ctxHlp ?? throw new($"This controller doesn't have a {nameof(CtxHlp)}. Check your constructor.");
    private readonly NetCoreWebApiContextHelper _ctxHlp;

    /// <summary>
    /// Initializer - just ensure SiteState is initialized thanks to our paths
    /// </summary>
    /// <param name="context"></param>
    [NonAction]
    public virtual void OnActionExecuting(ActionExecutingContext context)
    {
        var l = Log.Fn();
        _helper.OnActionExecuting(context, HistoryLogGroup);

        // background processes can pass in an alias using the SiteState service
        GetService<AliasResolver>().InitIfEmpty();
            
        if (_withBlockContext) CtxHlp.InitializeBlockContext(context);
        l.Done();
    }

    /// <inheritdoc/>
    [NonAction]
    public virtual void OnActionExecuted(ActionExecutedContext context)
    {
        var l = Log.Fn();
        _helper.OnActionExecuted(context);
        l.Done();
    }

    protected TService GetService<TService>() where TService : class => _helper.GetService<TService>();
}