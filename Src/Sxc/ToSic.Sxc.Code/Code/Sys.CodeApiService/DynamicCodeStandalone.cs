using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Context;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Sys.CodeApiService;

internal class DynamicCodeStandalone(IExecutionContext exCtx, ICodeDynamicApiHelper apiHelper): IDynamicCode12
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

    public dynamic? CreateInstance(string virtualPath, NoParamOrder npo = default, string? name = null,
        string? relativePath = null, bool throwOnError = true)
        => (
                apiHelper
                //(ICreateInstance)exCtx
                ?? throw new InvalidOperationException(
                    "CreateInstance can only be set on a DynamicCode12Proxy which implements ICreateInstance")
            )
            .CreateInstance(virtualPath, npo, name, relativePath, throwOnError);

    public int CompatibilityLevel => apiHelper.Cdf?.CompatibilityLevel ?? CompatibilityLevels.CompatibilityLevel12;

    public TService GetService<TService>() where TService : class
        => exCtx.GetService<TService>();

    public IApp App => apiHelper.App;

    public IDataSource Data => apiHelper.Data;

    public dynamic? Content => apiHelper.Content;

    public dynamic? Header => apiHelper.Header;

    IFolder IDynamicCode.AsAdam(ICanBeEntity item, string fieldName)
        => apiHelper.AsAdam(item, fieldName);

    IFolder IDynamicCode12.AsAdam(ICanBeEntity item, string fieldName)
        => apiHelper.AsAdam(item, fieldName);

    public ILinkService Link => apiHelper.Link;

    public IEditService Edit => apiHelper.Edit;

    dynamic? IDynamicCode.AsDynamic(string json, string? fallback)
        => apiHelper.Cdf.Json2Jacket(json, fallback);

    dynamic IDynamicCode12.AsDynamic(IEntity entity)
        => apiHelper.Cdf.CodeAsDyn(entity);

    dynamic? IDynamicCode12.AsDynamic(object dynamicEntity)
        => apiHelper.Cdf.AsDynamicFromObject(dynamicEntity);

    IEntity IDynamicCode12.AsEntity(object dynamicEntity)
        => apiHelper.Cdf.AsEntity(dynamicEntity);

    IEnumerable<dynamic>? IDynamicCode12.AsList(object list)
        => apiHelper.Cdf.CodeAsDynList(list);

    T IDynamicCode12.CreateSource<T>(IDataStream source)
        => apiHelper.CreateSource<T>(source);

    T IDynamicCode12.CreateSource<T>(IDataSource? inSource, ILookUpEngine? configurationProvider)
        => apiHelper.CreateSource<T>(inSource, configurationProvider);

    dynamic? IDynamicCode12.AsDynamic(string json, string? fallback)
        => apiHelper.Cdf.Json2Jacket(json, fallback);

    dynamic IDynamicCode.AsDynamic(IEntity entity)
        => apiHelper.Cdf.CodeAsDyn(entity);

    dynamic? IDynamicCode.AsDynamic(object dynamicEntity)
        => apiHelper.Cdf.AsDynamicFromObject(dynamicEntity);

    IEntity IDynamicCode.AsEntity(object dynamicEntity)
        => apiHelper.Cdf.AsEntity(dynamicEntity); // ((IDynamicCode)exCtx).AsEntity(dynamicEntity);

    IEnumerable<dynamic>? IDynamicCode.AsList(object list)
        => apiHelper.Cdf.CodeAsDynList(list);// ((IDynamicCode)exCtx).AsList(list);

    T IDynamicCode.CreateSource<T>(IDataStream source)
        => apiHelper.CreateSource<T>(source);

    T IDynamicCode.CreateSource<T>(IDataSource? inSource, ILookUpEngine? configurationProvider)
        => apiHelper.CreateSource<T>(inSource, configurationProvider);

    public ICmsContext CmsContext
        => apiHelper.CmsContext;

    public dynamic? AsDynamic(params object[] entities)
        => apiHelper.Cdf.MergeDynamic(entities);

    public IConvertService Convert => ((ExecutionContext)exCtx).Convert;

    public dynamic Resources => apiHelper.Resources;

    public dynamic Settings => apiHelper.Settings;

    public IDevTools DevTools => apiHelper.DevTools;
}