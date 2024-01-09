#if NETFRAMEWORK
using System.Web;
using System.Collections.Specialized;
using ToSic.Sxc.Web;


namespace ToSic.Sxc.DotNet
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class HttpNetFramework: HttpAbstractionBase, IHttp
    {
        /// <summary>
        /// Empty constructor for DI
        /// </summary>
        public HttpNetFramework() => Current = HttpContext.Current;

        #region Request and properties thereof

        public override NameValueCollection QueryStringParams
        {
            get
            {
                if (_queryStringValues != null) return _queryStringValues;
                return _queryStringValues = Request == null
                    ? new NameValueCollection()
                    : Request.QueryString;
            }
        }
        private NameValueCollection _queryStringValues;
        #endregion Request

        //public override IDictionary<object, object> Items => (IDictionary<object, object>)Current.Items;
    }
}
#endif