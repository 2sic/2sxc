using System;
using System.Collections.Specialized;
using ToSic.Sxc.Web.Url;

namespace ToSic.Sxc.Services
{
    public class NothingRewriteService : IImageflowRewriteService
    {
        public NameValueCollection QueryStringRewrite(NameValueCollection queryString)
        {
            Console.WriteLine($"NothingRewriteService:{queryString.NvcToString()}");
            return queryString;
        }
    }
}
