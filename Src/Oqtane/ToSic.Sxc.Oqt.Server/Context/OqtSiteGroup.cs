using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Oqt.Server.Context;

public sealed class OqtSiteGroup(
    ISiteRepository siteRepository,
    ISiteGroupMemberRepository siteGroupMemberRepository)
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.SitGrp", connect: [/*siteRepository, siteGroupMemberRepository*/])
{
    internal Site GetPrimaryLocalizationSite(Site site)
    {
        if (site == null)
            return null;

        var primarySiteId = GetPrimaryLocalizationSiteId(site.SiteId);
        if (primarySiteId == site.SiteId && site.CultureCode.HasValue())
            return site;

        return siteRepository.GetSite(primarySiteId);
    }

    internal int GetPrimaryLocalizationSiteId(int siteId)
    {
        var memberships = GetLocalizationSiteGroupMemberships(siteId)
            .Select(member => member.SiteGroup.PrimarySiteId)
            .ToList();
        return memberships.Count > 0 ? memberships.First() : siteId;
    }

    internal List<string> GetLocalizationGroupCultureCodes(Site site)
    {
        var cultures = OqtCulture.GetCultures(site);

        return cultures.Count > 0
            ? cultures
            : BuildLocalizationGroupCultureCodes(site, cultures);
    }



    internal bool IsLocalizationGroupSite(Site site)
        => site != null && GetLocalizationSiteGroupMemberships(site.SiteId).Any();

    /// <summary>
    /// rebuilds cultures for sibling sites in the localization group
    /// </summary>
    /// <param name="site"></param>
    /// <param name="cultures"></param>
    /// <returns></returns>
    private List<string> BuildLocalizationGroupCultureCodes(Site site, List<string> cultures)
    {
        var memberships = GetLocalizationSiteGroupMemberships(site.SiteId);
        if (memberships.Count == 0)
            return [];

        var sitesById = SitesById();

        // site is part of a localized site group - get all languages from the site group
        foreach (var siteGroupId in memberships.Select(member => member.SiteGroupId).Distinct())
        {
            foreach (var groupMember in siteGroupMemberRepository.GetSiteGroupMembers(-1, siteGroupId))
            {
                var cultureCode = groupMember.SiteId == site.SiteId
                    ? site.CultureCode
                    : sitesById.TryGetValue(groupMember.SiteId, out var siteInfo)
                        ? siteInfo.CultureCode
                        : null;
                OqtCulture.AddCodeIfMissing(cultures, cultureCode);
            }
        }

        return cultures;
    }

    //private Dictionary<int, Site> SitesById => _sitesById.Get(BuildSitesById);
    //private readonly GetOnce<Dictionary<int, Site>> _sitesById = new();

    private Dictionary<int, Site> SitesById()
        => siteRepository
            .GetSites()
            .GroupBy(siteInfo => siteInfo.SiteId)
            .ToDictionary(group => group.Key, group => group.First());

    private List<SiteGroupMember> GetLocalizationSiteGroupMemberships(int siteId)
    {
        if (siteId == OqtConstants.Unknown)
            return [];

        return siteGroupMemberRepository
            .GetSiteGroupMembers(siteId, OqtConstants.Unknown)
            .Where(member => member.SiteGroup?.Type == SiteGroupTypes.Localization)
            .OrderBy(member => member.SiteGroupId)
            .ToList();
    }
}
