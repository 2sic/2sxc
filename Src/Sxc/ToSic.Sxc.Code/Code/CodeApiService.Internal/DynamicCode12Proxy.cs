using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Context;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code;

internal class DynamicCode12Proxy(ICodeApiService parent): IDynamicCode12
{
    public ILog Log => parent.Log;

    public string CreateInstancePath
    {
        get => ((IGetCodePath)parent).CreateInstancePath;
        set
        {
            if (parent is not IGetCodePath getCodePath)
                throw new InvalidOperationException("CreateInstancePath can only be set on a DynamicCode12Proxy which implements IGetCodePath");
            getCodePath.CreateInstancePath = value;
        }
    }

    public dynamic CreateInstance(string virtualPath, NoParamOrder noParamOrder = default, string name = null,
        string relativePath = null, bool throwOnError = true) =>
        (
            (ICreateInstance)parent
            ?? throw new InvalidOperationException(
                "CreateInstance can only be set on a DynamicCode12Proxy which implements ICreateInstance")
        )
        .CreateInstance(virtualPath, noParamOrder, name, relativePath, throwOnError);

    public int CompatibilityLevel => ((ICompatibilityLevel)parent)?.CompatibilityLevel ?? 12;

    TService IDynamicCode.GetService<TService>()
        => parent.GetService<TService>();

    TService IDynamicCode12.GetService<TService>()
        => parent.GetService<TService>();

    public IApp App => parent.App;

    public IDataSource Data => parent.Data;

    public dynamic Content => parent.Content;

    public dynamic Header => parent.Header;

    IFolder IDynamicCode.AsAdam(ICanBeEntity item, string fieldName)
        => parent.AsAdam(item, fieldName);

    IFolder IDynamicCode12.AsAdam(ICanBeEntity item, string fieldName)
        => parent.AsAdam(item, fieldName);

    public ILinkService Link => parent.Link;

    public IEditService Edit => parent.Edit;

    dynamic IDynamicCode.AsDynamic(string json, string fallback)
        => parent.Cdf.Json2Jacket(json, fallback);

    dynamic IDynamicCode12.AsDynamic(IEntity entity)
        => parent.Cdf.CodeAsDyn(entity);

    dynamic IDynamicCode12.AsDynamic(object dynamicEntity)
        => parent.Cdf.AsDynamicFromObject(dynamicEntity);

    IEntity IDynamicCode12.AsEntity(object dynamicEntity)
        => parent.Cdf.AsEntity(dynamicEntity);

    IEnumerable<dynamic> IDynamicCode12.AsList(object list)
        => parent.Cdf.CodeAsDynList(list);

    T IDynamicCode12.CreateSource<T>(IDataStream source)
        => parent.CreateSource<T>(source);

    T IDynamicCode12.CreateSource<T>(IDataSource inSource, ILookUpEngine configurationProvider)
        => parent.CreateSource<T>(inSource, configurationProvider);

    dynamic IDynamicCode12.AsDynamic(string json, string fallback)
        => parent.Cdf.Json2Jacket(json, fallback);

    dynamic IDynamicCode.AsDynamic(IEntity entity)
        => parent.Cdf.CodeAsDyn(entity);

    dynamic IDynamicCode.AsDynamic(object dynamicEntity)
        => parent.Cdf.AsDynamicFromObject(dynamicEntity);

    IEntity IDynamicCode.AsEntity(object dynamicEntity)
        => ((IDynamicCode)parent).AsEntity(dynamicEntity);

    IEnumerable<dynamic> IDynamicCode.AsList(object list)
        => ((IDynamicCode)parent).AsList(list);

    T IDynamicCode.CreateSource<T>(IDataStream source)
        => parent.CreateSource<T>(source);

    T IDynamicCode.CreateSource<T>(IDataSource inSource, ILookUpEngine configurationProvider)
        => parent.CreateSource<T>(inSource, configurationProvider);

    public ICmsContext CmsContext
        => parent.CmsContext;

    public dynamic AsDynamic(params object[] entities)
        => parent.Cdf.MergeDynamic(entities);

    public IConvertService Convert => ((CodeApiService)parent).Convert;

    public dynamic Resources => parent.Resources;

    public dynamic Settings => parent.Settings;

    public IDevTools DevTools => parent.DevTools;
}