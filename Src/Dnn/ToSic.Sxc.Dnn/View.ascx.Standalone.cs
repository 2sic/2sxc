using System.Web;

namespace ToSic.Sxc.Dnn;

partial class View
{
    public bool RenderNaked
        => _renderNaked ??= Request.QueryString["standalone"] == "true";
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