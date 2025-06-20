using ToSic.Eav.DataSource;
using ToSic.Lib.LookUp.Engines;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.CodeApi;

internal class CodeDynamicApiHelper(ExecutionContext parent) : CodeAnyApiHelper(parent), ICodeDynamicApiHelper
{
    public dynamic? Content => Parent.Content;
    public dynamic? Header => Parent.Header;
    public IApp App => Parent.App;
    public IDynamicStack Resources => Parent.Resources;
    public IDynamicStack Settings => Parent.Settings;

    [field: AllowNull, MaybeNull]
    public IEditService Edit => field
        ??= Parent.GetService<IEditService>(reuse: true);

    public IFolder AsAdam(ICanBeEntity item, string fieldName)
        => Parent.AsAdam(item, fieldName);

    public T CreateSource<T>(IDataStream source) where T : IDataSource
        => Parent.CreateSource<T>(source);

    public T CreateSource<T>(IDataSource? inSource = null, ILookUpEngine? configurationProvider = default) where T : IDataSource
        => Parent.CreateSource<T>(inSource, configurationProvider);

    public ServiceKit14 ServiceKit14 => Parent.GetKit<ServiceKit14>();

    public string CreateInstancePath
    {
        get => Parent.CreateInstancePath;
        set => Parent.CreateInstancePath = value;
    }

    public dynamic CreateInstance(string virtualPath, NoParamOrder noParamOrder = default, string? name = null,
        string? relativePath = null, bool throwOnError = true) =>
        Parent.CreateInstance(virtualPath, noParamOrder, name, relativePath, throwOnError);

}