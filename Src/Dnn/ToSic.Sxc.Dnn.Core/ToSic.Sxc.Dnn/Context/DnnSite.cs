using System.Web.Hosting;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneCulture;
using ToSic.Sys.Wrappers;

namespace ToSic.Sxc.Dnn.Context;

internal record DnnSite : ISite, IWrapper<PortalSettings>, IZoneCultureResolverProWIP
{
    internal required PortalSettings PortalSettings { get; init; }
    public PortalSettings GetContents() => PortalSettings;

    public required int Id { get; init; }

    public string Name => PortalSettings.PortalName;
    
    public string AppsRootPhysical
        => Path.Combine(PortalSettings.HomeDirectory, AppConstants.AppsRootFolder);
    public string AppsRootPhysicalFull
        => HostingEnvironment.MapPath(AppsRootPhysical) ?? "";
    public string AppAssetsLinkTemplate
        => $"{AppsRootPhysical}/{AppConstants.AppFolderPlaceholder}";
    public string ContentPath => PortalSettings.HomeDirectory;

    public required int ZoneId { get; init; }

    public required string Url { get; init; }

    public required string UrlRoot { get; init; }

    public required List<string>? CultureCodesWithFallbacks { get; init; }

    public required string CurrentCultureCode { get; init; }
    public required string DefaultCultureCode { get; init; }
}