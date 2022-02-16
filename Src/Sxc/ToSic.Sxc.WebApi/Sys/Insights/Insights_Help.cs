using ToSic.Eav;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using static ToSic.Razor.Blade.Tag;

namespace ToSic.Sxc.WebApi.Sys
{
    public partial class Insights
    {
        private string Help()
        {
            var logWrap = Log.Call();

            const string typeattribs = "typeattributes?appid=&type=";
            const string typeMeta = "typemetadata?appid=&type=";
            const string typePerms = "typepermissions?appid=&type=";
            const string attribMeta = "attributemetadata?appid=&type=&attribute=";
            const string attribPerms = "attributepermissions?appid=&type=&attribute=";

            var result =
                    H1("2sxc Insights - Commands")
                    + P(
                        "In most cases you'll just browse the cache and use the links from there. "
                        + "The other links are listed here so you know what they would be, "
                        + "in case something is preventing you from browsing the normal way. "
                        + "Read more about 2sxc insights in the "
                        + Tag.A("blog post").Href("https://2sxc.org/en/blog/post/using-2sxc-insights").Target("_blank")
                    )

                    + H2("Most used")
                    + Ol(
                        Li(LinkTo("Help (this screen", nameof(Help))), 
                        Li(LinkTo("All Logs", nameof(Logs))),
                        Li(LinkTo("In Memory Cache", nameof(Cache))),
                        Li(LinkTo("ping the system / IsAlive", nameof(IsAlive)))
                    )

                    + H2("Global Data &amp; Types")
                    + Ol(
                        Li(LinkTo("Global Types in cache", nameof(GlobalTypes))),
                        Li(LinkTo("Global Types loading log", nameof(GlobalTypesLog))),
                        Li(LinkTo("Global logs", nameof(Logs), key: LogNames.LogHistoryGlobalTypes)),
                        Li(LinkTo("Licenses &amp; Features", nameof(Licenses)))
                    )

                    + H2("Manual links to access debug information")
                    + Ol(
                        Li("flush an app cache: " + DemoLink("purge?appid=")),
                        Li($"look at the load-log of an app-cache: <a href='{nameof(LoadLog)}?appid='>{nameof(LoadLog)}?appid=</a>"),
                        Li($"look at the cache-stats of an app: <a href='{nameof(Stats)}?appid='>{nameof(Stats)}?appid=</a>"),
                        Li($"look at the content-types of an app: <a href='{nameof(Types)}?appid='>{nameof(Types)}?appid=</a>"),
                        Li("look at attributes of a type: " + DemoLink(typeattribs)),
                        Li("look at type metadata:" + DemoLink(typeMeta)),
                        Li("look at type permissions:" + DemoLink(typePerms)),
                        Li("look at attribute Metadata :" + DemoLink(attribMeta)),
                        Li("look at attribute permissions:" + DemoLink(attribPerms)),
                        Li("look at entities of a type:" + DemoLink($"{nameof(Entities)}?appid=&type=")),
                        Li("look at all entities:" + DemoLink($"{nameof(Entities)}?appid=&type=all")),
                        Li("look at a single entity by id:" + DemoLink($"{nameof(Entity)}?appId=&entity=")),
                        Li("look at entity metadata using entity-id:" + DemoLink($"{nameof(EntityMetadata)}?appid=&entity=")),
                        Li("look at entity permissions using entity-id:" + DemoLink($"{nameof(EntityPermissions)}?appid=&entity="))
                    )
                ;
            logWrap("ok");
            return result.ToString();
        }

        protected A DemoLink(string labelAndLink) => Tag.A(labelAndLink).Href(labelAndLink);
        
        protected A LinkTo(string label, string view, 
            int? appId = null, string noParamOrder = Eav.Parameters.Protector, 
            string key = null, string type = null, string nameId = null, string more = null)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, nameof(LinkTo), "...");
            var link = UrlTo(view, appId, key: key, type: type, nameId: nameId, more: more);
            return Tag.A(label).Href(link);
        }

        protected bool NiceLink = true;

        protected string UrlTo(string view, int? appId = null, string noParamOrder = Eav.Parameters.Protector, 
            string key = null, string type = null, string nameId = null, string more = null)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, nameof(UrlTo), "...");
            var link = (NiceLink ? $"./{view}?" : $"details?view={view}")
                       + (appId != null ? "&appid=" + appId : "")
                       + (type != null ? "&type=" + type : "")
                       + (key != null ? "&key=" + key : "")
                       + (nameId != null ? "&nameId=" + nameId : "")
                       + (more != null ? "&" + more : "");
            return link;
        }
    }
}
