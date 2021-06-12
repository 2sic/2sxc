using System.Collections.Generic;
using ToSic.Razor.Html5;
using ToSic.Razor.Internals.Page;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageService
    {
        /// <inheritdoc />
        public void AddIcon(string path, string doNotRelyOnParameterOrder = ToSic.Eav.Parameters.Protector, string rel = "",
            int size = 0, string type = null)
        {
            ToSic.Eav.Parameters.ProtectAgainstMissingParameterNames(doNotRelyOnParameterOrder, nameof(AddIcon), $"{nameof(path)}, {nameof(rel)}, {nameof(size)}, {nameof(type)}");
            AddToHead(new Icon(path, rel, size, type));
        }

        /// <inheritdoc />
        public void AddIconSet(string path, string doNotRelyOnParameterOrder = ToSic.Eav.Parameters.Protector,
            object favicon = null, IEnumerable<string> rels = null, IEnumerable<int> sizes = null)
        {
            ToSic.Eav.Parameters.ProtectAgainstMissingParameterNames(doNotRelyOnParameterOrder, nameof(AddIcon), $"{nameof(path)}, {nameof(favicon)}, {nameof(rels)}, {nameof(sizes)}");
            foreach (var s in IconSet.GenerateIconSet(path, favicon, rels, sizes))
                AddToHead(s);
        }

    }
}
