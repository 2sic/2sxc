using System;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

// TODO: #Oqtane - doesn't really do anything yet

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtRenderingHelper: RenderingHelper
    {
        public OqtRenderingHelper(ILinkPaths linkPaths) : base(linkPaths, "Mvc.RndHlp") { }

        protected override void LogToEnvironmentExceptions(Exception ex)
        {
            // Don't do anything
        }
    }
}
