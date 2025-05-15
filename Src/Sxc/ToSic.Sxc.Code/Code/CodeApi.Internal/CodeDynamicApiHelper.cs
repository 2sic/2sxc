using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.CodeApi;

internal class CodeDynamicApiHelper: CodeAnyApiHelper, ICodeDynamicApiHelper
{
    public dynamic Content => Parent.Content;
    public dynamic Header => Parent.Header;
    public IApp App => Parent.App;
    public object Resources => Parent.Resources;
    public object Settings => Parent.Settings;
    public IEditService Edit => field
        ??= Parent.GetService<IEditService>(reuse: true);

    public IFolder AsAdam(ICanBeEntity item, string fieldName)
        => Parent.AsAdam(item, fieldName);

    public T CreateSource<T>(IDataStream source) where T : IDataSource
        => Parent.CreateSource<T>(source);

    public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
        => Parent.CreateSource<T>(inSource, configurationProvider);

    public ServiceKit14 ServiceKit14 => Parent.GetKit<ServiceKit14>();
}