using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Sys.CodeApi;

internal class CodeDynamicApiHelper(ExecutionContext exCtx) : ICodeDynamicApiHelper
{
    #region Common State

    public IBlock Block => exCtx.Block!; // If accessed, the code doing it must have the block

    public ICmsContext CmsContext => exCtx.CmsContext;

    public IDataSource Data => exCtx.Block!.Data; // If accessed, it must be working
    
    // Fails - was invisible because of MEL
    //public IApp App => (IApp)ExCtx.App!;
    // works
    public IApp App => field ??= exCtx.GetService<Generator<App, IAppForCode>>().New(exCtx.App!);

    #endregion


    #region Common Services

    public TService GetService<TService>(NoParamOrder npo = default, bool reuse = false, Type? type = default)
        where TService : class
        => exCtx.GetService<TService>(npo, reuse, type);

    public ServiceKit14 ServiceKit14 => exCtx.GetKit<ServiceKit14>();

    public IDevTools DevTools => throw new NotImplementedException("This is a future feature, we're just reserving the object name");

    public ICodeDataFactory Cdf => exCtx.Cdf;

    public ILinkService Link => field ??= GetService<ILinkService>(reuse: true);

    public IConvertService Convert => field ??= GetService<IConvertService>(reuse: true);

    [field: AllowNull, MaybeNull]
    public IEditService Edit => field
        ??= exCtx.GetService<IEditService>(reuse: true);

    #endregion


    #region Common APIs

    public IFolder AsAdam(ICanBeEntity item, string fieldName)
        => Cdf.Folder(item, fieldName, null);

    public T CreateSource<T>(IDataStream source) where T : IDataSource
        => exCtx.CreateSource<T>(source);

    public T CreateSource<T>(IDataSource? inSource = null, ILookUpEngine? configurationProvider = default) where T : IDataSource
        => exCtx.CreateSource<T>(inSource, configurationProvider);

    #endregion


    #region Common Local Data

    public IDynamicStack Resources => exCtx.Resources;

    public IDynamicStack Settings => exCtx.Settings;

    /// <inheritdoc cref="IDynamicCodeDocs.Content" />
    public IDynamicEntity? Content => _contentGo.Get(() => TryToBuildFirstOfStream(DataSourceConstants.StreamDefaultName));
    private readonly LazyGet<IDynamicEntity?> _contentGo = new();


    /// <inheritdoc cref="IDynamicCodeDocs.Header" />
    public IDynamicEntity? Header => _header.Get(GetHeaderOrNull);
    private readonly LazyGet<IDynamicEntity?> _header = new();


    private IDynamicEntity? GetHeaderOrNull()
    {
        var l = exCtx.Log.Fn<IDynamicEntity?>();
        if (TryToBuildFirstOfStream(ViewParts.StreamHeader) is { } header)
            return l.Return(header, "found");
        // If header isn't found, it could be that an old query is used which attached the stream to the old name
        l.A($"Header not yet found in {ViewParts.StreamHeader}, will try {ViewParts.StreamHeaderOld}");
        return l.Return(TryToBuildFirstOfStream(ViewParts.StreamHeaderOld), "old query");

    }

    /// <summary>
    /// Note: can be null
    /// </summary>
    /// <param name="sourceStream"></param>
    /// <returns></returns>
    private IDynamicEntity? TryToBuildFirstOfStream(string sourceStream)
    {
        var l = exCtx.Log.Fn<IDynamicEntity>(sourceStream);
        if (Block is not { DataIsReady: true } block)
            return l.ReturnNull("no data/view");

        if (block.Data.GetStream(sourceStream, nullIfNotFound: true) is not { } stream)
            return l.ReturnNull("stream not found");

        var list = stream.List.ToList();
        return !list.Any()
            ? l.ReturnNull("first is null")
            : l.Return(Cdf.AsDynamicFromEntities(list, new() { ItemIsStrict = false }), "found");
    }

    #endregion


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