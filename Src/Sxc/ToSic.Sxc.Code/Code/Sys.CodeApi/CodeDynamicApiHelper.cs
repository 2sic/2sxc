using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Sys.CodeApi;

internal class CodeDynamicApiHelper(ExecutionContext exCtx) : ICodeDynamicApiHelper
{
    #region ICodeAnyApiHelper

    public IBlock Block => exCtx.Block!; // If accessed, the code doing it must have the block

    public ICmsContext CmsContext => exCtx.CmsContext;
    public IDataSource Data => exCtx.Block!.Data; // If accessed, it must be working

    public TService GetService<TService>(NoParamOrder npo = default, bool reuse = false, Type? type = default)
        where TService : class
        => exCtx.GetService<TService>(npo, reuse, type);

    public IDevTools DevTools => exCtx.DevTools;

    public ICodeDataFactory Cdf => exCtx.Cdf;
    public ILinkService Link => exCtx.Link;

    public IConvertService Convert => exCtx.Convert;

    #endregion

    public dynamic? Content => exCtx.Content;
    public dynamic? Header => exCtx.Header;

    // Fails - was invisible because of MEL
    //public IApp App => (IApp)ExCtx.App!;
    // works
    public IApp App => field ??= exCtx.GetService<Generator<App, IAppForCode>>().New(exCtx.App!);

    public IDynamicStack Resources => exCtx.Resources;
    public IDynamicStack Settings => exCtx.Settings;

    [field: AllowNull, MaybeNull]
    public IEditService Edit => field
        ??= exCtx.GetService<IEditService>(reuse: true);

    public IFolder AsAdam(ICanBeEntity item, string fieldName)
        => exCtx.AsAdam(item, fieldName);

    public T CreateSource<T>(IDataStream source) where T : IDataSource
        => exCtx.CreateSource<T>(source);

    public T CreateSource<T>(IDataSource? inSource = null, ILookUpEngine? configurationProvider = default) where T : IDataSource
        => exCtx.CreateSource<T>(inSource, configurationProvider);

    public ServiceKit14 ServiceKit14 => exCtx.GetKit<ServiceKit14>();

    #region SharedCode Compiler

    private IClassCompiler CodeCompilerLazy => field ??= exCtx.GetService<IClassCompiler>();

    /// <inheritdoc />
    public string CreateInstancePath { get; set; } = null!;  // must be set as the context becomes clear

    //public dynamic? CreateInstance(string virtualPath, NoParamOrder npo = default, string? name = null,
    //    string? relativePath = null, bool throwOnError = true) =>
    //    ExCtx.CreateInstance(virtualPath, npo, name, relativePath, throwOnError);


    /// <inheritdoc />
    public object? CreateInstance(string virtualPath,
        NoParamOrder npo = default,
        string? name = null,
        string? relativePath = null,
        bool throwOnError = true)
    {
        var l = exCtx.Log.Fn<object>($"{virtualPath}, {name}, {relativePath}, {throwOnError}");

        // Compile
        var runtimeKey = exCtx.App!.AppReader.Specs.CacheKey;
        var spec = new HotBuildSpec(App.AppId, exCtx.EditionForHotBuild, exCtx.App.AppSpecs.Name, runtimeKey);
        var instance = CodeCompilerLazy.InstantiateClass(virtualPath, spec, className: name, relativePath: relativePath, throwOnError: throwOnError);

        if (instance == null)
            return l.ReturnNull("null / not found / error");

        var typeName = instance.GetType().FullName;

        // if it supports all our known context properties, attach them
        if (instance is INeedsExecutionContext needsRoot)
        {
            l.A($"will attach root / Kit to {typeName}");
            needsRoot.ConnectToRoot(exCtx);
        }
        else
            l.A($"no root / Kit for {typeName}");

        return l.Return(instance, instance.ToString());
    }

    #endregion

}