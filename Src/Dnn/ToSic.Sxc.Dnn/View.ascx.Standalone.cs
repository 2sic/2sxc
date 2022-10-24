using System.Web;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Dnn
{
    public partial class View
    {
        public bool RenderNaked
            => _renderNaked ?? (_renderNaked = Request.QueryString["standalone"] == "true").Value;
        private bool? _renderNaked;

        private void SendStandalone(string renderedTemplate)
        {
            Response.Clear();
            Response.Write(renderedTemplate);
            Response.Flush();
            Response.SuppressContent = true;
            LogTimer.Done("Standalone");
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

    }
}