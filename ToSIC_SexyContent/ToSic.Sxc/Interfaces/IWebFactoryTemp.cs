using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Web;

namespace ToSic.SexyContent.Interfaces
{
    public interface IWebFactoryTemp
    {
        DynamicCodeBase AppAndDataHelpers(ICmsBlock cms);
    }
}
