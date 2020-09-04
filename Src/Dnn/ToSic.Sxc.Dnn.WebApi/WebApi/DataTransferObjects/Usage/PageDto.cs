using DotNetNuke.Entities.Tabs;

namespace ToSic.Sxc.WebApi.DataTransferObjects.Usage
{
    public class PageDto
    {
        public int Id;
        public string CultureCode;
        public string Name;
        public string Title;
        public string Url;
        public bool Visible;
        public PortalDto Portal;

        public PageDto(TabInfo page)
        {
            Id = page.TabID;
            Url = page.FullUrl;
            Name = page.TabName;
            CultureCode = page.CultureCode;
            Visible = page.IsVisible;
            Title = page.Title;
            Portal = new PortalDto(page.PortalID);
        }
    }
}
