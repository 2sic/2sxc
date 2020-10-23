using System;
using ToSic.Sxc.Web;

// TODO: #Oqtane - doesn't really do anything yet

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtaneRenderingHelper: RenderingHelper
    {
        public OqtaneRenderingHelper(IHttp http) : base(http, "Mvc.RndHlp") { }

        protected override void LogToEnvironmentExceptions(Exception ex)
        {
            // Don't do anything
        }
    }
}
