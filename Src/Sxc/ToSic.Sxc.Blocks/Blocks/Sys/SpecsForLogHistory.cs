using ToSic.Razor.Blade;
using ToSic.Sxc.Apps;
using ToSic.Sys.Utils;
using static System.StringComparer;

namespace ToSic.Sxc.Blocks.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class SpecsForLogHistory
{
    public IDictionary<string, string> BuildSpecsForLogHistory(IBlock block, IApp? app = default, string? entry = default, bool addView = true)
    {
        try
        {
            // use app provided or try to use from block
            app ??= block?.AppOrNull;

            // No context - neither blog nor App, exit
            var specs = new Dictionary<string, string>(InvariantCultureIgnoreCase);
            if (block == default && app == default)
                return specs;

            if (entry != default)
                specs.Add("Entry", entry);

            string? bestTitle = null;

            if (block != null)
            {
                specs.Add(nameof(block.ContentBlockId), block.ContentBlockId.ToString());

                var view = block.ViewIsReady ? block.View : null; 
                if (addView && view != null)
                {
                    specs.Add("ViewId", view.Id.ToString());
                    specs.Add("ViewEdition", view.Edition ?? "");
                    specs.Add("ViewPath", view.Path);
                }

                var ctx = block.Context;
                if (ctx != null!)
                {
                    var site = ctx.Site;
                    if (site != null!)
                    {
                        specs.Add("SiteId", site.Id.ToString());
                        specs.Add("SiteUrl", site.Url);
                    }

                    var page = ctx.Page;
                    if (page != null!)
                    {
                        var pageParams = page.Parameters.ToString();
                        var pageUrl = page.Url + (string.IsNullOrEmpty(pageParams) ? "" : "?" + pageParams);
                        specs.Add("PageId", page.Id.ToString());
                        specs.Add("PageUrl", pageUrl);
                        var urlToShow = Text.After(Text.After(pageUrl, "//"), "/")
                                            .NullOrGetWith(v => "/" + v)
                                        ?? pageUrl;
                        bestTitle = urlToShow;
                    }

                    var module = ctx.Module;
                    if (module != null!)
                    {
                        var mid = module.Id.ToString();
                        specs.Add("ModuleId", mid);
                    }

                    var usr = ctx.User;
                    if (usr != null!)
                    {
                        var uid = usr.Id.ToString();
                        specs.Add("UserId", uid);
                    }
                }

            }

            if (app != null)
            {
                var appId = app.AppId.ToString();
                specs.Add(nameof(app.AppId), appId);
                specs.Add("AppPath", app.Path);
                specs.Add("AppName", app.Name);
            }

            if (bestTitle != null)
                specs.Add(LogStoreEntry.TitleKey, bestTitle);

            return specs;
        }
        catch
        {
            /* fail silently */
            return new Dictionary<string, string>(InvariantCultureIgnoreCase);
        }
    }
}