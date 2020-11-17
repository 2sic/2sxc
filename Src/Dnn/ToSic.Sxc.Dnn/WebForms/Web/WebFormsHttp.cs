using System.Web;
using System.Collections.Specialized;
using ToSic.Sxc.Web;


namespace ToSic.Sxc.WebForms.Web
{
    public class WebFormsHttp: HttpAbstractionBase, IHttp
    {
        /// <summary>
        /// Empty constructor for DI
        /// </summary>
        public WebFormsHttp() => Current = HttpContext.Current;

        #region Request and properties thereof

        public override NameValueCollection QueryStringParams
        {
            get
            {
                if (_queryStringValues != null) return _queryStringValues;
                return Request == null
                    ? _queryStringValues = new NameValueCollection()
                    : _queryStringValues = Request.QueryString;
            }
        }

        private NameValueCollection _queryStringValues;
        #endregion Request

    }
}
