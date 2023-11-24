using System.Collections.Generic;
using ToSic.Sxc.Apps;
using static System.StringComparer;

namespace ToSic.Sxc.Blocks;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class SpecsForLogHistory
{
    internal IDictionary<string, string> BuildSpecsForLogHistory(IBlock block, IApp app = default, string entry = default, bool addView = true)
    {
        // use app provided or try to use from block
        app = app ?? block?.App;

        if (block == default && app == default) return null;

        var specs = new Dictionary<string, string>(InvariantCultureIgnoreCase);
        if (entry != default)
            specs.Add("Entry", entry);

        if (block != null)
        {
            specs.Add(nameof(block.ContentBlockId), block.ContentBlockId.ToString());

            var view = block.View;
            if (addView && view != null)
            {
                specs.Add("ViewId", view.Id.ToString());
                specs.Add("ViewEdition", view.Edition);
                specs.Add("ViewPath", view.Path);
            }

            var ctx = block.Context;
            if (ctx != null)
            {
                var site = ctx.Site;
                if (site != null)
                {
                    specs.Add("SiteId", site.Id.ToString());
                    specs.Add("SiteUrl", site.Url);
                }

                var page = ctx.Page;
                if (page != null)
                {
                    specs.Add("PageId", page.Id.ToString());
                    specs.Add("PageUrl", page.Url);
                }

                var module = ctx.Module;
                if (module != null) specs.Add("ModuleId", module.Id.ToString());

                var usr = ctx.User;
                if (usr != null)
                {
                    specs.Add("UserId", usr.Id.ToString());
                }
            }

        }

        if (app != null)
        {
            specs.Add(nameof(app.AppId), app.AppId.ToString());
            specs.Add("AppPath", app.Path);
            specs.Add("AppName", app.Name);
        }



        return specs;
    }
}