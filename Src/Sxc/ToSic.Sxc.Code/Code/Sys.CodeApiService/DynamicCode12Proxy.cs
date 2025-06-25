using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code;

internal class DynamicCode12Proxy(IExecutionContext exCtx, ICodeDynamicApiHelper dynamicApi): IDynamicCode12
{
    public ILog Log => exCtx.Log!;

    public string CreateInstancePath
    {
        get => ((IGetCodePath)exCtx).CreateInstancePath;
        set
        {
            if (exCtx is not IGetCodePath getCodePath)
                throw new InvalidOperationException("CreateInstancePath can only be set on a DynamicCode12Proxy which implements IGetCodePath");
            getCodePath.CreateInstancePath = value;
        }
    }

    public dynamic? CreateInstance(string virtualPath, NoParamOrder noParamOrder = default, string? name = null,
        string? relativePath = null, bool throwOnError = true)
        => (
                dynamicApi
                //(ICreateInstance)exCtx
                ?? throw new InvalidOperationException(
                    "CreateInstance can only be set on a DynamicCode12Proxy which implements ICreateInstance")
            )
            .CreateInstance(virtualPath, noParamOrder, name, relativePath, throwOnError);

    public int CompatibilityLevel => dynamicApi.Cdf?.CompatibilityLevel ?? 12; // ((ICompatibilityLevel?)exCtx)?.CompatibilityLevel ?? 12;

    TService IDynamicCode.GetService<TService>()
        => exCtx.GetService<TService>();

    TService IDynamicCode12.GetService<TService>()
        => exCtx.GetService<TService>();

    public IApp App => dynamicApi.App;

    public IDataSource Data => dynamicApi.Data;

    public dynamic? Content => dynamicApi.Content;

    public dynamic? Header => dynamicApi.Header;

    IFolder IDynamicCode.AsAdam(ICanBeEntity item, string fieldName)
        => dynamicApi.AsAdam(item, fieldName);

    IFolder IDynamicCode12.AsAdam(ICanBeEntity item, string fieldName)
        => dynamicApi.AsAdam(item, fieldName);

    public ILinkService Link => dynamicApi.Link;

    public IEditService Edit => dynamicApi.Edit;

    dynamic? IDynamicCode.AsDynamic(string json, string? fallback)
        => dynamicApi.Cdf.Json2Jacket(json, fallback);

    dynamic IDynamicCode12.AsDynamic(IEntity entity)
        => dynamicApi.Cdf.CodeAsDyn(entity);

    dynamic? IDynamicCode12.AsDynamic(object dynamicEntity)
        => dynamicApi.Cdf.AsDynamicFromObject(dynamicEntity);

    IEntity IDynamicCode12.AsEntity(object dynamicEntity)
        => dynamicApi.Cdf.AsEntity(dynamicEntity);

    IEnumerable<dynamic>? IDynamicCode12.AsList(object list)
        => dynamicApi.Cdf.CodeAsDynList(list);

    T IDynamicCode12.CreateSource<T>(IDataStream source)
        => dynamicApi.CreateSource<T>(source);

    T IDynamicCode12.CreateSource<T>(IDataSource? inSource, ILookUpEngine? configurationProvider)
        => dynamicApi.CreateSource<T>(inSource, configurationProvider);

    dynamic? IDynamicCode12.AsDynamic(string json, string? fallback)
        => dynamicApi.Cdf.Json2Jacket(json, fallback);

    dynamic IDynamicCode.AsDynamic(IEntity entity)
        => dynamicApi.Cdf.CodeAsDyn(entity);

    dynamic? IDynamicCode.AsDynamic(object dynamicEntity)
        => dynamicApi.Cdf.AsDynamicFromObject(dynamicEntity);

    IEntity IDynamicCode.AsEntity(object dynamicEntity)
        => dynamicApi.Cdf.AsEntity(dynamicEntity); // ((IDynamicCode)exCtx).AsEntity(dynamicEntity);

    IEnumerable<dynamic>? IDynamicCode.AsList(object list)
        => dynamicApi.Cdf.CodeAsDynList(list);// ((IDynamicCode)exCtx).AsList(list);

    T IDynamicCode.CreateSource<T>(IDataStream source)
        => dynamicApi.CreateSource<T>(source);

    T IDynamicCode.CreateSource<T>(IDataSource? inSource, ILookUpEngine? configurationProvider)
        => dynamicApi.CreateSource<T>(inSource, configurationProvider);

    public ICmsContext CmsContext
        => dynamicApi.CmsContext;

    public dynamic? AsDynamic(params object[] entities)
        => dynamicApi.Cdf.MergeDynamic(entities);

    public IConvertService Convert => ((ExecutionContext)exCtx).Convert;

    public dynamic Resources => dynamicApi.Resources;

    public dynamic Settings => dynamicApi.Settings;

    public IDevTools DevTools => dynamicApi.DevTools;
}