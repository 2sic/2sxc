using Oqtane.Modules;
using Oqtane.UI;

namespace ToSic.Sxc.Oqt.Client.Services
{
    public interface IOqtPrerenderService
    {
        IOqtPrerenderService Init(PageState pageState, ModuleBase.Logger logger);
        string GetSystemHtml();
    }
}