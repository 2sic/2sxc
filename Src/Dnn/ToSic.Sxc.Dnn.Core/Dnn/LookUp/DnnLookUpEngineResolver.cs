using DotNetNuke.Entities.Portals;
using ToSic.Eav.Context;
using ToSic.Eav.LookUp;
using ToSic.Sxc.LookUp.Internal;
using ToSic.Sxc.Web.Internal.DotNet;
using static ToSic.Sxc.LookUp.LookUpConstants;

namespace ToSic.Sxc.Dnn.LookUp;

/// <summary>
/// Retrieves the current engine for a specific module. <br/>
/// Internally it asks DNN for the current Property-Access objects and prepares them for use in EAV.
/// </summary>
internal class DnnLookUpEngineResolver(IZoneCultureResolver cultureResolver, LazySvc<IHttp> httpLazy, LazySvc<IEnumerable<ILookUp>> builtInSources)
    : LookUpEngineResolverBase(builtInSources, "Dnn.LookUp", connect: [cultureResolver, httpLazy])
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
            return l.Return(cached, $"reuse {cached.Sources} sources");

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
        var httpAndDiAdditions = AddHttpAndDiSources(lookUps);
        var sources = lookUps.Concat(httpAndDiAdditions).ToList();

        //var lookupEngine = new LookUpEngine(Log, sources: sources);
        //AddHttpAndDiSources(/*lookupEngine,*/ lookUps).DoIfNotNull(lookupEngine.Add);

        // Expand the Lookup for "module" to also have an "id" property
        var additions = new List<ILookUp>();
        if (sources.HasSource(SourceModule))
        {
            var original = sources.GetSource(SourceModule);
            var modAdditional = new LookUpInDictionary(SourceModule, new Dictionary<string, string>
            {
                { KeyId, original.Get(OldDnnModuleId) }
            });
            additions.Add(new LookUpInLookUps(SourceModule, [modAdditional, original]));
        }

        // Create the lookup for "site" based on the "portal" and only give it "id" & "guid"
        if (sources.HasSource(OldDnnSiteSource))
            additions.Add(new LookUpInDictionary(SourceSite, new Dictionary<string, string>
            {
                { KeyId, sources.GetSource(OldDnnSiteSource).Get(OldDnnSiteId) },
                { KeyGuid, $"{DotNetNuke.Common.Globals.GetPortalSettings()?.GUID}" }
            }));

        // Create the lookup for "page" based on the "tab" and only give it "id" & "guid"
        if (sources.HasSource(OldDnnPageSource))
            additions.Add(new LookUpInDictionary(SourcePage, new Dictionary<string, string>
            {
                { KeyId, sources.GetSource(OldDnnPageSource).Get(OldDnnPageId) },
                { KeyGuid, $"{DotNetNuke.Common.Globals.GetPortalSettings()?.ActiveTab?.UniqueId}" }
            }));


        //lookupEngine.Add(additions);

        // 2024-05-06 2dm - this code never had an effect, so I believe I can assume
        // ...that it hasn't worked for a very long time, so I'll comment out to clean up EOY 2024
        //// Add "form" source if it's not already there, and we have a request.Form
        //// request is sometimes null (e.g. in DNN scheduled task)
        //// Note that "Form" is only available on Dnn, not on Oqtane
        //if (!lookupEngine.HasSource("form") && httpLazy.Value.Request?.Form != null)
        //    additions.Add(new LookUpInNameValueCollection("form", httpLazy.Value.Request.Form));

        var lookupEngine = new LookUpEngine(Log, sources: [.. sources, .. additions]);


        // Note: Not implemented in Dnn: "Tenant" source
        return l.ReturnAsOk(lookupEngine);
    }
}