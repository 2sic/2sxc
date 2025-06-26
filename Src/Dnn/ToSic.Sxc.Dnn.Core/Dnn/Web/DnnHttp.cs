using System.Collections.Specialized;
using ToSic.Sxc.Web.Sys.Http;

namespace ToSic.Sxc.Dnn.Web;

/// <summary>
/// Special class to provide HttpRequest parameters in DNN
///
/// It needs this, because DNN automatically creates invisible parameters like tabid=323 and language=en-US
/// But if we want to create a link to the same page again, then this would result in wrong paths. 
/// </summary>
internal class DnnHttp: HttpHybrid
{
    public override NameValueCollection QueryStringParams
    {
        get
        {
            if (field != null)
                return field;

            // This would be the better way, but it doesn't work, because DNN will often create paths like
            // /value/27 instead of ?value=27
            //var separator = Request.RawUrl.IndexOf('?');
            //if (separator == -1) return _queryStringValues = new NameValueCollection();
            //var queryPart = Request.RawUrl.Substring(separator);
            //var lightList = HttpUtility.ParseQueryString(queryPart);

            var rewrapped = new NameValueCollection(base.QueryStringParams);
            return field = FilterOutDnnParams(rewrapped);
        }
    }

    private NameValueCollection FilterOutDnnParams(NameValueCollection original)
    {
        const string tabId = "TabId";
        const string language = "language";
        const string skinSrc = "SkinSrc"; // auto-added

        // DNN adds these automatically, but does it with exactly this spelling, so that's the only one we'll catch
        original.Remove(tabId);
        original.Remove(language);
        original.Remove(skinSrc);
        return original;
    }
}