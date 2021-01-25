using ToSic.Eav.Context;

namespace IntegrationSamples.SxcEdit01.Adam
{
    public class IntSite : ISite
    {
        public int Id { get; set; }= 1;

        public string Name => "Demo Integration Site";

        public string AppsRootPhysical => "todo apps root phys not set";

        public string AppsRootPhysicalFull => "todo apps root physical full not set";

        public string AppsRootLink => "todo apps root link not set";

        public string ContentPath => "wwwroot";

        public string Url => "" ; // "/";

        public int ZoneId => Id;

        public string CurrentCultureCode => "";

        public string DefaultCultureCode => "";

        public ISite Init(int siteId)
        {
            Id = siteId;
            return this;
        }
    }
}
