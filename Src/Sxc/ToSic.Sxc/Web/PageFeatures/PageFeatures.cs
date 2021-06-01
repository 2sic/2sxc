using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Web.PageFeatures
{
    [PrivateApi("Hide implementation")]
    public class PageFeatures: IPageFeatures
    {
        /// <inheritdoc />
        public void Activate(params string[] keys) => ActiveKeys.AddRange(keys.Where(k => !string.IsNullOrWhiteSpace(k)));

        public List<string> ActiveKeys { get; } = new List<string>();
        
        
        public List<string> GetKeysAndFlush()
        {
            var keys = ActiveKeys.ToArray().ToList();
            ActiveKeys.Clear();
            return keys;
        }
    }
}
