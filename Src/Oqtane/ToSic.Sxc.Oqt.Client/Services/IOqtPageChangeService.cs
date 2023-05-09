using System.Threading.Tasks;
using Oqtane.UI;
using ToSic.Sxc.Oqt.App;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Web.ContentSecurityPolicy;

namespace ToSic.Sxc.Oqt.Client.Services;

public interface IOqtPageChangeService
{
    Task AttachScriptsAndStyles(OqtViewResultsDto viewResults, PageState pageState, SxcInterop sxcInterop, ModuleProBase page);
    int ApplyHttpHeaders(OqtViewResultsDto result, ModuleProBase page);
    Task UpdatePageProperties(OqtViewResultsDto viewResults, PageState pageState, SxcInterop sxcInterop, ModuleProBase page);
    string UpdateProperty(string original, OqtPagePropertyChanges change, ModuleProBase page);
    CspOfPage PageCsp(bool enforced, ModuleProBase page);
}