using DotNetNuke.Entities.Portals;
using ToSic.Eav.Context;
using ToSic.Eav.LookUp;
using ToSic.Lib.Services;
using static ToSic.Sxc.LookUp.LookUpConstants;

namespace ToSic.Sxc.Dnn.LookUp;

/// <summary>
/// Retrieves the current engine for a specific module. <br/>
/// Internally it asks DNN for the current Property-Access objects and prepares them for use in EAV.
/// </summary>
internal class DnnLookUpEngineResolver : ServiceBase, ILookUpEngineResolver
{
    #region Constructor / Dependency Injection

    public DnnLookUpEngineResolver(IZoneCultureResolver cultureResolver) : base("Dnn.LookUp")
    {
        ConnectServices(
            _cultureResolver = cultureResolver
        );
    }
    private readonly IZoneCultureResolver _cultureResolver;

    #endregion

    /// <inheritdoc />
    public ILookUpEngine GetLookUpEngine(int moduleId)
    {
        var l = Log.Fn<ILookUpEngine>($"{nameof(moduleId)}:{moduleId}");
        return PortalSettings.Current == null
            ? l.Return(new LookUpEngine(Log), "no context")
            : l.Return(GenerateDnnBasedLookupEngine(PortalSettings.Current, moduleId), "with site");
    }

    [PrivateApi]
    public LookUpEngine GenerateDnnBasedLookupEngine(PortalSettings portalSettings, int moduleId)
    {
        var l = Log.Fn<LookUpEngine>($"{nameof(moduleId)}: {moduleId}");
        var providers = new LookUpEngine(Log);
        var dnnUsr = portalSettings.UserInfo;
        var dnnCult = _cultureResolver.SafeCurrentCultureInfo();
        var dnn = new DnnTokenReplace(moduleId, portalSettings, dnnUsr);
        var stdSources = dnn.PropertySources;

        var lookUps = stdSources
            .Select(s => new LookUpInDnnPropertyAccess(s.Key, s.Value, dnnUsr, dnnCult) as ILookUp)
            .ToList();

        // must already add, as we'll later check if some specific ones exist
        providers.Add(lookUps);
        lookUps.Clear();

        // Expand the Lookup for "module" to also have an "id" property
        if (providers.HasSource(SourceModule))
        {
            var original = providers.Sources[SourceModule];
            var modAdditional = new LookUpInDictionary(SourceModule, new Dictionary<string, string>
            {
                { KeyId, original.Get(OldDnnModuleId) }
            });
            lookUps.Add(new LookUpInLookUps(SourceModule, modAdditional, original));
        }

        // Create the lookup for "site" based on the "portal" and only give it "id" & "guid"
        if (providers.HasSource(OldDnnSiteSource))
            lookUps.Add(new LookUpInDictionary(SourceSite, new Dictionary<string, string>
            {
                { KeyId, providers.Sources[OldDnnSiteSource].Get(OldDnnSiteId) },
                { KeyGuid, $"{DotNetNuke.Common.Globals.GetPortalSettings()?.GUID}" }
            }));

        // Create the lookup for "page" based on the "tab" and only give it "id" & "guid"
        if (providers.HasSource(OldDnnPageSource))
            lookUps.Add(new LookUpInDictionary(SourcePage, new Dictionary<string, string>
            {
                { KeyId, providers.Sources[OldDnnPageSource].Get(OldDnnPageId) },
                { KeyGuid, $"{DotNetNuke.Common.Globals.GetPortalSettings()?.ActiveTab?.UniqueId}" }
            }));

        providers.Add(lookUps);

        // Not implemented in Dnn: "Tenant" source

        return l.ReturnAsOk(providers);
    }
}