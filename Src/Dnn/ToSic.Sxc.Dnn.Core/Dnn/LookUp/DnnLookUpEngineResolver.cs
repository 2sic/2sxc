using DotNetNuke.Entities.Portals;
using ToSic.Eav.Context;
using ToSic.Eav.LookUp;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.LookUp.Internal;
using ToSic.Sxc.Web.Internal.DotNet;
using static ToSic.Sxc.LookUp.LookUpConstants;

namespace ToSic.Sxc.Dnn.LookUp;

/// <summary>
/// Retrieves the current engine for a specific module. <br/>
/// Internally it asks DNN for the current Property-Access objects and prepares them for use in EAV.
/// </summary>
internal class DnnLookUpEngineResolver(IZoneCultureResolver cultureResolver, LazySvc<IHttp> httpLazy, LazySvc<IEnumerable<ILookUp>> lookUps)
    : LookUpEngineResolverBase(lookUps, "Dnn.LookUp", connect: [cultureResolver, httpLazy])
{
    protected override LookUpEngine BuildLookupEngine(int moduleId)
    {
        var l = Log.Fn<LookUpEngine>($"{nameof(moduleId)}:{moduleId}");
        return PortalSettings.Current == null
            ? l.Return(base.BuildLookupEngine(moduleId), "no context, use base")
            : l.Return(BuildDnnBasedLookupEngine(PortalSettings.Current, moduleId), "with site");
    }

    [PrivateApi]
    public LookUpEngine LookUpEngineOfPortalSettings(PortalSettings portalSettings, int moduleId)
    {
        var l = Log.Fn<LookUpEngine>($"{nameof(moduleId)}: {moduleId}");

        //// if we already have a list of shared sources, return that
        //// as the sources don't change per request, but per module
        if (TryReuseFromCache(moduleId, out var cached))
            return l.Return(cached, $"reuse {cached.Sources.Count} sources");

        var lookupEngine = BuildDnnBasedLookupEngine(portalSettings, moduleId);

        return l.ReturnAsOk(AddToCache(moduleId, lookupEngine));
    }

    [PrivateApi]
    private LookUpEngine BuildDnnBasedLookupEngine(PortalSettings portalSettings, int moduleId)
    {
        var l = Log.Fn<LookUpEngine>($"{nameof(moduleId)}: {moduleId}");

        // Otherwise build using Dnn Built-In Sources and HttpSources and more
        var dnnUsr = portalSettings.UserInfo;
        var dnnCult = cultureResolver.SafeCurrentCultureInfo();
        var dnn = new DnnTokenReplace(moduleId, portalSettings, dnnUsr);
        var stdSources = dnn.PropertySources;

        var lookUps = stdSources
            .Select(s => new LookUpInDnnPropertyAccess(s.Key, s.Value, dnnUsr, dnnCult) as ILookUp)
            .ToList();

        // must already add, as we'll later check if some specific ones exist
        var lookupEngine = new LookUpEngine(Log);
        lookupEngine.Add(lookUps);
        AddHttpAndDiSources(lookupEngine).DoIfNotNull(lookupEngine.Add);

        // Expand the Lookup for "module" to also have an "id" property
        var additions = new List<ILookUp>();
        if (lookupEngine.HasSource(SourceModule))
        {
            var original = lookupEngine.Sources[SourceModule];
            var modAdditional = new LookUpInDictionary(SourceModule, new Dictionary<string, string>
            {
                { KeyId, original.Get(OldDnnModuleId) }
            });
            additions.Add(new LookUpInLookUps(SourceModule, modAdditional, original));
        }

        // Create the lookup for "site" based on the "portal" and only give it "id" & "guid"
        if (lookupEngine.HasSource(OldDnnSiteSource))
            additions.Add(new LookUpInDictionary(SourceSite, new Dictionary<string, string>
            {
                { KeyId, lookupEngine.Sources[OldDnnSiteSource].Get(OldDnnSiteId) },
                { KeyGuid, $"{DotNetNuke.Common.Globals.GetPortalSettings()?.GUID}" }
            }));

        // Create the lookup for "page" based on the "tab" and only give it "id" & "guid"
        if (lookupEngine.HasSource(OldDnnPageSource))
            additions.Add(new LookUpInDictionary(SourcePage, new Dictionary<string, string>
            {
                { KeyId, lookupEngine.Sources[OldDnnPageSource].Get(OldDnnPageId) },
                { KeyGuid, $"{DotNetNuke.Common.Globals.GetPortalSettings()?.ActiveTab?.UniqueId}" }
            }));

        lookupEngine.Add(additions);

        if (!lookupEngine.HasSource("form"))
            additions.Add(new LookUpInNameValueCollection("form", httpLazy.Value.Request?.Form)); // request is sometimes null (eg. in DNN scheduled task)

        // Note: Not implemented in Dnn: "Tenant" source

        return l.ReturnAsOk(lookupEngine);
    }
}