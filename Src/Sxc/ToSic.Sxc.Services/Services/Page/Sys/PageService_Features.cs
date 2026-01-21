using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys;
using ToSic.Sxc.Data.Sys.DynamicStack;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Utils;
using static ToSic.Sxc.Web.Sys.WebResources.WebResourceConstants;

namespace ToSic.Sxc.Services.Page.Sys;

partial class PageService
{
    /// <summary>
    /// Test code to modify the CDN source, not for public use
    /// </summary>
    /// <param name="cdnSource"></param>
    public void TestCdn(string cdnSource)
    {
        _overrideCdnSource = cdnSource;
    }
    private string? _overrideCdnSource;

    /// <inheritdoc />
    public string Activate(params string[] keys)
    {
        keys ??= [];
        var l = Log.Fn<string>($"{nameof(keys)}: '{string.Join(",", keys)}'");
        FeatureKeysAdded.AddRange(keys);

        // #PartialCaching must know about all activated features
        var doFirst = false;
        if (doFirst)
            Listeners.Activate(keys);

        // 1. Try to add manual resources from WebResources
        // This must happen in the IPageService which is per-module
        // The PageServiceShared cannot do this, because it doesn't have the WebResources which vary by module
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (WebResources is not null /* paranoid */) // important: DynamicEntity null-compare isn't quite right, **do not** use `!=`
            keys = AddResourcesFromSettings(keys);

        // 2. If any keys are left, they are probably preconfigured keys, so add them now
        if (!keys.Any())
            return l.ReturnAsOk("");

        l.A($"Remaining keys: {string.Join(",", keys)}");
        var added = PageServiceShared.PageFeatures.Activate(keys).ToArray();

        // WIP #PartialCaching
        if (!doFirst)
            Listeners.Activate(added);

        // also add to this specific module, as we need a few module-level features to activate in case...
        ExCtxOrNull?.GetState<IBlock>()?.BlockFeatureKeys.AddRange(added);

        return l.ReturnAsOk(""); // empty string, just so it can be used as `@Kit.Page.Activate(...)` and not produce anything
    }

    /// <inheritdoc />
    public string? Activate(
        NoParamOrder npo = default,
        bool condition = true,
        params string[] features)
    {
        var l = Log.Fn<string>();

        // Check condition - default is true - so if it's false, this overload was called
        return !condition
            ? l.ReturnNull("condition false")
            : l.Return(Activate(features), "condition true, added");
    }

    private string[] AddResourcesFromSettings(string[] keys)
    {
        var l = Log.Fn<string[]>();
        var keysToRemove = new List<string>();
        var processor = new WebResourceProcessor(featuresSvc.Value, _overrideCdnSource ?? CdnSource, Log);
        foreach (var key in keys)
        {
            l.A($"Key: {key}");
            if (WebResources.Get(key) is not DynamicEntity webRes) // special problem: DynamicEntity null-compare isn't quite right, don't! use ==
                continue;

            // Found - make sure we remove the key, no matter what decisions are made below
            keysToRemove.Add(key);

            var pageFeature = processor.Process(key, webRes);
            if (pageFeature == null)
                continue;

            l.A("Found html and everything, will register");
            // all ok so far
            PageServiceShared.PageFeatures.FeaturesFromSettingsAdd(pageFeature);

            // Wip #PartialCaching
            Listeners.AddResource(pageFeature);
        }

        // drop keys which were already taken care of
        keys = keys.Where(k => !keysToRemove.Contains(k)).ToArray();
        return l.Return(keys);
    }

    /// <summary>
    /// List of all feature keys which were ever added. Will never be cleared.
    /// </summary>
    public List<string> FeatureKeysAdded { get; } = [];

    public bool HasFeature(string featureKey)
        => FeatureKeysAdded.Any(f => f.EqualsInsensitive(featureKey));


    private string? CdnSource => _cdnSource.Get(() => WebResources.Get<string>(CdnSourcePublicField));
    private readonly GetOnce<string?> _cdnSource = new();

    private DynamicEntity WebResources => _webResources.Get(() => (DynamicEntity)Settings.Get(WebResourcesNode)!)!;
    private readonly GetOnce<DynamicEntity> _webResources = new();

    private DynamicStack Settings => _settings.Get(() => (ExCtx.GetDataStack<IDynamicStack>(ExecutionContextStateNames.Settings) as DynamicStack)!)!;
    private readonly GetOnce<DynamicStack> _settings = new();

}