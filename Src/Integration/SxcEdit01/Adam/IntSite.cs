using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Sxc.Run;

namespace IntegrationSamples.SxcEdit01.Adam
{
    public class IntSite : ISite
    {
        public ISite Init(int siteId, ILog parentLog)
        {
            Id = siteId;
            return this;
        }

        public int Id { get; set; }= 1;

        public string Name => "Demo Integration Site";

        public string AppsRootPhysical => "todo apps root phys not set";

        public string AppsRootPhysicalFull => "todo apps root physical full not set";

        public string AppAssetsLinkTemplate => "todo apps root link not set/" + AppConstants.AppFolderPlaceholder;

        public string ContentPath => "wwwroot";

        public string Url => "https://" + UrlRoot;

        public string UrlRoot => "some-domain/en-us";

        public int ZoneId => Id;

        public string CurrentCultureCode => "";

        public string DefaultCultureCode => "";


    }
}
