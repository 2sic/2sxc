using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Client.Services
{
    public interface ISxcOqtaneService
    {
        Task<SxcOqtaneDto> PrepareAsync(int aliasId, int siteId, int pageId, int moduleId, Dictionary<string, StringValues> originalParameters);

        //SxcOqtaneDto Prepare(int aliasId, int siteId, int pageId, int moduleId);
    }
}
