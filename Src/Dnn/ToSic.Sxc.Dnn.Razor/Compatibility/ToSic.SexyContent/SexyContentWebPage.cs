using Custom.Hybrid;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.CodeApi.Internal;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Compatibility.RazorPermissions;
using ToSic.Sxc.Compatibility.Sxc;
using ToSic.Sxc.Data.Sys.Wrappers;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Code.Help;
using IApp = ToSic.Sxc.Apps.IApp;


// ReSharper disable InheritdocInvalidUsage

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Razor;

/// <summary>
/// The core page type for delivering a 2sxc page
/// Provides context infos like the Dnn object, helpers like Edit and much more. 
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class SexyContentWebPage : 
    RazorComponentBase,
    ICreateInstance,
    IHasDnn,
    // #RemovedV20 #ModulePublish
    //#pragma warning disable CS0618 // Type or member is obsolete
    //    IDnnRazorCustomize, 
    //#pragma warning restore CS0618 // Type or member is obsolete
    // #RemovedV20 #IAppAndDataHelpers
    //IDynamicCodeBeforeV10
    //#pragma warning disable 618
    //    IAppAndDataHelpers
    //#pragma warning restore 618
    // Remainders after removing IDnnRazorCustomize
    IDynamicCode,
    // new,
    IHasCodeHelp
{
    internal ICodeDynamicApiHelper CodeApi => field ??= ExCtx.GetDynamicApi();

    #region Core Properties which should appear in docs

    /// <inheritdoc />
    public override ICodeLog Log => RzrHlp.CodeLog;

    /// <inheritdoc />
    public override IHtmlHelper Html => RzrHlp.Html;

    #endregion

    #region Helpers linked through AppAndData Helpers

    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => CodeApi.Link;

    [PrivateApi]
    public dynamic DynamicModel => throw new NotSupportedException($"{nameof(DynamicModel)} not implemented on {nameof(SexyContentWebPage)}. {RazorComponent.NotImplementedUseCustomBase}");

    /// <inheritdoc cref="IDynamicCode.Edit" />
    public IEditService Edit => CodeApi.Edit;

    public IDnnContext Dnn => (ExCtx as IHasDnn)?.Dnn;

#pragma warning disable 612
    /// <inheritdoc />
    [PrivateApi("never public, shouldn't be in use elsewhere")]
    [Obsolete]
    [field: Obsolete]
    public SxcHelper Sxc => field
        ??= new(CodeApi.Block?.Context.Permissions.IsContentAdmin ?? false, GetService<IConvertToEavLight>());
#pragma warning restore 612

    /// <summary>
    /// Old API - probably never used, but we shouldn't remove it as we could break some existing code out there
    /// </summary>
    [PrivateApi] public IBlock Block => CodeApi.Block;

    /// <inheritdoc cref="ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class => CodeApi.GetService<TService>();

    [PrivateApi] public override int CompatibilityLevel => CompatibilityLevels.CompatibilityLevel9Old;

    /// <inheritdoc />
    public new IApp App => CodeApi.App;

    #region Data - with old interface #DataInAddWontWork

    /// <inheritdoc />
    public IDataSource Data => /*(IBlockDataSource)*/CodeApi.Data;

    //// This is explicitly implemented so the interfaces don't complain
    //// but actually we're not showing this - in reality we're showing the Old (see above)
    //IBlockDataSource IAppAndDataHelpers.Data => (IBlockDataSource)_CodeApiSvc.Data;
        
    #endregion

    //// Explicit implementation of expected interface, but it should not work in the normal code
    //// as the old code sometimes expects Data.Cache.GetContentType
    ///// <inheritdoc />
    //IDataSource IDynamicCode.Data => CodeApi.Data;

    public RazorPermissions Permissions => new(CodeApi.Block?.Context.Permissions.IsContentAdmin ?? false);

    #region AsDynamic in many variations

    /// <inheritdoc />
    [Obsolete]
    public dynamic AsDynamic(IEntity entity) => CodeApi.Cdf.CodeAsDyn(entity);


    /// <inheritdoc />
    public dynamic AsDynamic(object dynamicEntity) => CodeApi.Cdf.AsDynamicFromObject(dynamicEntity);

    /// <inheritdoc />
    [PublicApi("Careful - still Experimental in 12.02")]
    public dynamic AsDynamic(params object[] entities) => CodeApi.Cdf.MergeDynamic(entities);

    // todo: only in "old" controller, not in new one
    /// <inheritdoc />
    [Obsolete]
    public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) => CodeApi.Cdf.CodeAsDyn(entityKeyValuePair.Value);



    /// <inheritdoc />
    [Obsolete]
    public IEnumerable<dynamic> AsDynamic(IDataStream stream) => CodeApi.Cdf.CodeAsDynList(stream.List);

    /// <inheritdoc />
    public IEntity AsEntity(object dynamicEntity) => CodeApi.Cdf.AsEntity(dynamicEntity);


    /// <inheritdoc />
    [Obsolete]
    public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) => CodeApi.Cdf.CodeAsDynList(entities);

    #endregion

    #region Future features: AsList / Settings / Resources

    [PrivateApi]
    public IEnumerable<dynamic> AsList(object list)
        => throw new("AsList is a newer feature in 2sxc. To use it, change your template type to " + nameof(Razor12) + " see https://go.2sxc.org/RazorComponent");

    [PrivateApi]
    public dynamic Resources
        => throw new("Resources is a newer feature in 2sxc. To use it, change your template type to " + nameof(Razor12) + " see https://go.2sxc.org/RazorComponent");

    [PrivateApi]
    public dynamic Settings
        => throw new("Settings is a newer feature in 2sxc. To use it, change your template type to " + nameof(Razor12) + " see https://go.2sxc.org/RazorComponent");

    #endregion

    #region Not supported, not captured new features (like Convert-Service) - because they would break old code
    //[PrivateApi] 
    //public IConvertService Convert => throw new NotSupportedException($"{nameof(Convert)} not implemented on {nameof(SexyContentWebPage)}. {RazorComponent.NotImplementedUseCustomBase}");
    #endregion


    #region Compatibility with Eav.Interfaces.IEntity - introduced in 10.10 - Removed in v20
    //[PrivateApi]
    //[Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
    //public dynamic AsDynamic(Eav.Interfaces.IEntity entity) => CodeApi.Cdf.CodeAsDyn(entity as IEntity);


    //[PrivateApi]
    //[Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
    //public dynamic AsDynamic(KeyValuePair<int, Eav.Interfaces.IEntity> entityKeyValuePair) => CodeApi.Cdf.CodeAsDyn(entityKeyValuePair.Value as IEntity);

    //[PrivateApi]
    //[Obsolete("for compatibility only, avoid using this and cast your entities to ToSic.Eav.Data.IEntity")]
    //public IEnumerable<dynamic> AsDynamic(IEnumerable<Eav.Interfaces.IEntity> entities) => CodeApi.Cdf.CodeAsDynList(entities.Cast<IEntity>());
    #endregion


    #region Data Source Stuff
    /// <inheritdoc />
    [Obsolete]
    public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, ILookUpEngine configurationProvider = null)
        => new CodeApiServiceObsolete(ExCtx).CreateSource(typeName, inSource, configurationProvider);

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
    [Obsolete("this is the old implementation with ILookUp Engine, don't think it was ever used publicly because people couldn't create these engines")]
    public T CreateSource<T>(IDataSource inSource = default, ILookUpEngine configurationProvider = default) where T : IDataSource
        => CodeApi.CreateSource<T>(inSource, configurationProvider);

    /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
    public T CreateSource<T>(IDataStream source) where T : IDataSource
        => CodeApi.CreateSource<T>(source);

    #endregion


    #region Content, Header, etc. and List
    /// <inheritdoc cref="IDynamicCode.Content" />
    public dynamic Content => CodeApi.Content;

    [Obsolete("use Content.Presentation instead")]
    [PrivateApi]
    public dynamic Presentation => CodeApi.Content?.Presentation;

    /// <summary>
    /// We are blocking this property on purpose, so that people will want to migrate to the new RazorComponent
    /// </summary>
    [PrivateApi]
    public dynamic Header => throw new("The header property is a new feature in 2sxc 10.20. " +
                                       "To use it, change your template type to inherit from " +
                                       nameof(RazorComponent) + " see https://go.2sxc.org/RazorComponent");

#pragma warning disable 618
    [Obsolete("Use Header instead")]
    public dynamic ListContent => CodeApi.Header;

    [Obsolete("Use Header.Presentation instead")]
    public dynamic ListPresentation => CodeApi.Header?.Presentation;

    // #RemovedV20 #Element
    //[Obsolete("This is an old way used to loop things - shouldn't be used any more - will be removed in a future version")]
    //[field: Obsolete("don't use any more")]
    //public List<Element> List => field ??= new CodeApiServiceObsolete(ExCtx).ElementList;
#pragma warning restore 618

    /// <inheritdoc cref="IDynamicCode.AsDynamic(string, string)" />
    public dynamic AsDynamic(string json, string fallback = WrapperConstants.EmptyJson)
        => throw new("The AsDynamic(string) is a new feature in 2sxc 10.20. To use it, change your template type to inherit from " 
                     + nameof(RazorComponent) + " see https://go.2sxc.org/RazorComponent");

    #endregion

    #endregion

    #region Customize Data & Search - all have been removed in v20 // #RemovedV20 #ModulePublish

    ///// <inheritdoc />
    //public virtual void CustomizeData() {}

    ///// <inheritdoc />
    //public virtual void CustomizeSearch(Dictionary<string, List<ISearchItem>> searchInfos, IModule moduleInfo, DateTime beginDate) { }

    // #RemovedV20 #ModulePublish
    //[PrivateApi("this is the old signature, should still be supported")]
    //public virtual void CustomizeSearch(Dictionary<string, List<ISearchInfo>> searchInfos, ModuleInfo moduleInfo, DateTime beginDate) { }

    // #RemovedV20 #ModulePublish
    //[Obsolete("should not be used any more")]
    //public Purpose Purpose { get; internal set; }

    // #RemovedV20 #ModulePublish
    //[Obsolete("should not be used any more")]
    //public InstancePurposes InstancePurpose { get; internal set; }

    #endregion


    #region Adam 

    /// <inheritdoc cref="IDynamicCode.AsAdam" />
    public IFolder AsAdam(ICanBeEntity item, string fieldName) => CodeApi.AsAdam(item, fieldName);

    #endregion

    #region CmsContext

    /// <inheritdoc cref="IDynamicCode.CmsContext" />
    public ICmsContext CmsContext => CodeApi.CmsContext;

    #endregion

    #region CreateInstance

    [PrivateApi] string IGetCodePath.CreateInstancePath { get; set; }

    /// <inheritdoc />
    public virtual dynamic CreateInstance(string virtualPath, NoParamOrder noParamOrder = default, string name = null, string relativePath = null, bool throwOnError = true)
        => RzrHlp.CreateInstance(virtualPath, noParamOrder, name, throwOnError: throwOnError);

    #endregion

    // Added this in v20 to show uses of GetBestValue; but much of it may not be applicable, in which case we should create a separate list for SexyContentWebPage and Dnn.RazorComponent
    [PrivateApi] List<CodeHelp> IHasCodeHelp.ErrorHelpers => HelpForRazor12.Compile12;

}