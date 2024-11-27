using ToSic.Lib.Helpers;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal.Stack;
using ToSic.Sxc.Web.WebResources;
using static ToSic.Sxc.Web.WebResources.WebResourceConstants;

namespace ToSic.Sxc.Web.Internal.PageService;

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
    private string _overrideCdnSource;

    /// <inheritdoc />
    public string Activate(params string[] keys)
    {
        keys ??= [];
        var l = Log.Fn<string>($"{nameof(keys)}: '{string.Join(",", keys)}'");

        // 1. Try to add manual resources from WebResources
        // This must happen in the IPageService which is per-module
        // The PageServiceShared cannot do this, because it doesn't have the WebResources which vary by module
        if (WebResources is not null) // important: DynamicEntity null-compare isn't quite right, **do not** use `!=`
            keys = AddResourcesFromSettings(keys);

        // 2. If any keys are left, they are probably preconfigured keys, so add them now
        if (!keys.Any())
            return l.ReturnAsOk("");

        l.A($"Remaining keys: {string.Join(",", keys)}");
        var added = PageServiceShared.Activate(keys);

        // also add to this specific module, as we need a few module-level features to activate in case...
        ((ICodeApiServiceInternal)_CodeApiSvc)?._Block?.BlockFeatureKeys.AddRange(added);

        return l.ReturnAsOk(""); // empty string, just so it can be used as `@Kit.Page.Activate(...)` and not produce anything
    }

    /// <inheritdoc />
    public string Activate(
        NoParamOrder noParamOrder = default,
        bool condition = true,
        params string[] features)
    {
        var l = Log.Fn<string>();

        // Check condition - default is true - so if it's false, this overload was called
        if (!condition)
            return l.ReturnNull("condition false");
            
        // Todo: unclear what to do with the parameter protector
        // Maybe must check the parameter protector, because we're not sure if the user may be calling this overload with a feature name
        // Reason is we're not 100% sure it takes the simple overload vs. this one if 
        // only one string is given, but ATM that's the case.

        return l.Return(Activate(features), "condition true, added");
    }

    private string[] AddResourcesFromSettings(string[] keys)
    {
        var l = Log.Fn<string[]>();
        var keysToRemove = new List<string>();
        var processor = new WebResourceProcessor(features.Value, _overrideCdnSource ?? CdnSource, Log);
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
        }

        // drop keys which were already taken care of
        keys = keys.Where(k => !keysToRemove.Contains(k)).ToArray();
        return l.Return(keys);
    }

    private string CdnSource => _cdnSource.Get(() => WebResources.Get<string>(CdnSourcePublicField));
    private readonly GetOnce<string> _cdnSource = new();

    private DynamicEntity WebResources => _webResources.Get(() => Settings?.Get(WebResourcesNode) as DynamicEntity);
    private readonly GetOnce<DynamicEntity> _webResources = new();

    private DynamicStack Settings => _settings.Get(() => _CodeApiSvc?.Settings as DynamicStack);
    private readonly GetOnce<DynamicStack> _settings = new();
}