using ToSic.Sxc.Blocks;
using ToSic.Sxc.Code;
using ToSic.Sxc.Web;

namespace ToSic.SexyContent.Interfaces
{
    public interface IWebFactoryTemp
    {
        DynamicCodeRoot AppAndDataHelpers(ICmsBlock cms);
    }
}
