using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Sys.CodeApi;

internal class CodeDynamicApiHelper(ExecutionContext exCtx) : CodeAnyApiHelper(exCtx), ICodeDynamicApiHelper
{
    public dynamic? Content => ExCtx.Content;
    public dynamic? Header => ExCtx.Header;
    public IApp App => ExCtx.App;
    public IDynamicStack Resources => ExCtx.Resources;
    public IDynamicStack Settings => ExCtx.Settings;

    [field: AllowNull, MaybeNull]
    public IEditService Edit => field
        ??= ExCtx.GetService<IEditService>(reuse: true);

    public IFolder AsAdam(ICanBeEntity item, string fieldName)
        => ExCtx.AsAdam(item, fieldName);

    public T CreateSource<T>(IDataStream source) where T : IDataSource
        => ExCtx.CreateSource<T>(source);

    public T CreateSource<T>(IDataSource? inSource = null, ILookUpEngine? configurationProvider = default) where T : IDataSource
        => ExCtx.CreateSource<T>(inSource, configurationProvider);

    public ServiceKit14 ServiceKit14 => ExCtx.GetKit<ServiceKit14>();

    public string CreateInstancePath
    {
        get => ExCtx.CreateInstancePath;
        set => ExCtx.CreateInstancePath = value;
    }

    public dynamic? CreateInstance(string virtualPath, NoParamOrder noParamOrder = default, string? name = null,
        string? relativePath = null, bool throwOnError = true) =>
        ExCtx.CreateInstance(virtualPath, noParamOrder, name, relativePath, throwOnError);

}