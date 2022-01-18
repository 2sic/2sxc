using ToSic.Eav;
using static ToSic.Razor.Blade.Tag;

namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Insights
    {
        public string Help()
        {
            var logWrap = Log.Call();
            ThrowIfNotSuperUser();

            const string typeattribs = "typeattributes?appid=&type=";
            const string typeMeta = "typemetadata?appid=&type=";
            const string typePerms = "typepermissions?appid=&type=";
            const string attribMeta = "attributemetadata?appid=&type=&attribute=";
            const string attribPerms = "attributepermissions?appid=&type=&attribute=";
            const string entities = "entities?appid=&type=";
            const string entitiesAll = "entities?appid=&type=all";
            const string entity = "entity?appId=&entity=";
            const string entityMeta = "entitymetadata?appid=&entity=";
            const string entityPerms = "entitypermissions?appid=&entity=";
            const string purgeapp = "purge?appid=";
            const string globTypes = "globaltypes";
            const string globTypesLog = "globaltypeslog";
            const string logs = "logs";
            var result = "" + H1("2sxc Insights - Commands")
                            + P(
                                "In most cases you'll just browse the cache and use the links from there. "
                                + "The other links are listed here so you know what they would be, "
                                + "in case something is preventing you from browsing the normal way. "
                                + "Read more about 2sxc insights in the "
                                + A("blog post").Href("https://2sxc.org/en/blog/post/using-2sxc-insights").Target("_blank")
                            )

                            + H2("Most used")
                            + "<ol>"
                            + Li("<a href='help'>Help</a> (this screen)")
                            + Li("All logs: " + A(logs).Href(logs))
                            + Li("In Memory " + A("cache").Href("cache"))
                            + Li("ping the system <a href='isalive'>isalive</a>")
                            + "</ol>"

                            + H2("Global Data &amp; Types")
                            + "<ol>"
                            + Li(A("Global Types in cache").Href(globTypes))
                            + Li(A("Global Types loading log").Href(globTypesLog))
                            + Li(A("Global logs").Href("logs?key=" + LogNames.LogHistoryGlobalTypes))
                            + Li(A("Licenses &amp; Features").Href("details?view=licenses"))
                            + "</ol>"

                            + H2("Manual links to access debug information")
                            + "<ol>"
                            + Li("flush an app cache: " + A(purgeapp).Href(purgeapp))
                            + Li("look at the load-log of an app-cache: <a href='loadlog?appid='>loadlog?appid=</a>")
                            + Li("look at the cache-stats of an app: <a href='stats?appid='>stats?appid=</a>")
                            + Li("look at the content-types of an app: <a href='types?appid='>types?appid=</a>")
                            + Li("look at attributes of a type: " + A(typeattribs).Href(typeattribs))
                            + Li("look at type metadata:" + A(typeMeta).Href(typeMeta))
                            + Li("look at type permissions:" + A(typePerms).Href(typePerms))
                            + Li("look at attribute Metadata :" + A(attribMeta).Href(attribMeta))
                            + Li("look at attribute permissions:" + A(attribPerms).Href(attribPerms))
                            + Li("look at entities of a type:" + A(entities).Href(entities))
                            + Li("look at all entities:" + A(entitiesAll).Href(entitiesAll))
                            + Li("look at a single entity by id:" + A(entity).Href(entity))
                            + Li("look at entity metadata using entity-id:" + A(entityMeta).Href(entityMeta))
                            + Li("look at entity permissions using entity-id:" + A(entityPerms).Href(entityPerms))
                            + "<ol>"
                ;
            logWrap("ok");
            return result;
        }
    }
}
