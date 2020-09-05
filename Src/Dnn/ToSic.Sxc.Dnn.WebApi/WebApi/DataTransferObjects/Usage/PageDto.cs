using DotNetNuke.Entities.Tabs;
using ToSic.Sxc.WebApi.Usage.Dto;

namespace ToSic.Sxc.WebApi.DataTransferObjects.Usage
{
    public class PageDto: Context.PageDto
    {
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
