using System.Linq;
using System.Net.Http;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.ContentBlock;
using ToSic.SexyContent.Interfaces;

namespace ToSic.SexyContent.WebApi
{
    public static class HttpRequestMessageExtensions
    {
        internal static SxcInstance GetSxcOfModuleContext(this HttpRequestMessage request)
        {
            var moduleInfo = request.FindModuleInfo();
            IContentBlock contentBlock = new ModuleContentBlock(moduleInfo);

            // check if we need an inner block
            var cbidh = request.Headers.GetValues("ContentBlockId").FirstOrDefault();
            int cbid;
            int.TryParse(cbidh, out cbid);
            if (cbid < 0)   // negative id, so it's an inner block
                contentBlock = new EntityContentBlock(contentBlock, cbid);
                //return _cbm = new EntityContentBlockManager(SxcContext);


            return contentBlock.SxcInstance;
        }

    }
}
