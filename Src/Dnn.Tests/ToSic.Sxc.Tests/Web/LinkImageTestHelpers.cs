using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToSic.Sxc.Data;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Tests.Web
{
    internal static class LinkImageTestHelpers
    {
        public static ImgResizeLinker GetLinker() => new ImgResizeLinker();

        /// <summary>
        /// Use this accessor to avoid having hundreds of code-uses of the constructor, when we're only using it for tests
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public static DynamicReadObject ToDyn(object contents) => new DynamicReadObject(contents);

    }
}
