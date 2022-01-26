using Oqtane.Modules;
using Oqtane.UI;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Client.Services
{
    public interface IPrerenderService
    {
        PrerenderService Init(PageState pageState, ModuleBase.Logger logger);
        string GetSystemHtml();
    }
}