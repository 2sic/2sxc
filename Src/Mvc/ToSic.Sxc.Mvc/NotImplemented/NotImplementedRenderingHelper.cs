using System;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Mvc.NotImplemented
{
    public class NotImplementedRenderingHelper: RenderingHelper
    {
        public NotImplementedRenderingHelper(ILinkPaths linkPaths) : base(linkPaths, "Mvc.RndHlp") { }

        protected override void LogToEnvironmentExceptions(Exception ex)
        {
            // Don't do anything
        }
    }
}
