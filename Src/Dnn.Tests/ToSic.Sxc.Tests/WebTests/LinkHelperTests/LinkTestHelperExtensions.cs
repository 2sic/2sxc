using ToSic.Sxc.Web;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    public static class LinkTestHelperExtensions
    {
        /// <summary>
        /// Special helper to avoid accessing the real To so many times
        /// </summary>
        public static string TestTo(this ILinkHelper link,
            string noParamOrder = Eav.Parameters.Protector,
            int? pageId = null,
            object parameters = null,
            string api = null,
            string part = null
        ) => link.To(noParamOrder: noParamOrder, pageId: pageId, parameters: parameters, api: api, part: part);
    }
}
