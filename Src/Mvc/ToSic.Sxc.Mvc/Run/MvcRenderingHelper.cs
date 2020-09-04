using System;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcRenderingHelper: RenderingHelper
    {
        public MvcRenderingHelper(IHttp http) : base(http, "Mvc.RndHlp") { }

        protected override void LogToEnvironmentExceptions(Exception ex)
        {
            // Don't do anything
        }
    }
}
