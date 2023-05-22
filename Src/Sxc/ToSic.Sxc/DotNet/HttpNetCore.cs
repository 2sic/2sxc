#if !NETFRAMEWORK
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.AspNetCore.Http;
using ToSic.Sxc.Web;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DotNet
{

    public class HttpNetCore : HttpAbstractionBase, IHttp
    {
        public HttpNetCore(IHttpContextAccessor contextAccessor) => Current = contextAccessor.HttpContext;

        public override NameValueCollection QueryStringParams
        {
            get
            {
                if (_queryStringValues != null) return _queryStringValues;
                // ReSharper disable once ConvertIfStatementToReturnStatement
                if (Request == null) 
                    return _queryStringValues = new NameValueCollection();

                var paramList = new NameValueCollection();
                Request.Query.ToList().ForEach(i => paramList.Add(i.Key, i.Value));
                return _queryStringValues = paramList;
            }
        }
        private NameValueCollection _queryStringValues;

        //public override IDictionary<object, object> Items => Current.Items;


    }
}

#endif