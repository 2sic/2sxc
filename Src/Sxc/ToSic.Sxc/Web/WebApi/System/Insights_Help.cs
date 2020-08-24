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
            var result = h1("2sxc Insights - Commands")
                   + p(
                       "In most cases you'll just browse the cache and use the links from there. "
                       + "The other links are listed here so you know what they would be, "
                       + "in case something is preventing you from browsing the normal way. "
                       + "Read more about 2sxc insights in the "
                       + a("blog post", "https://2sxc.org/en/blog/post/using-2sxc-insights", true)
                       )

                   + h2("Most used")
                   + "<ol>"
                   + li("<a href='help'>Help</a> (this screen)")
                   + li("All logs: " + a(logs, logs))
                   + li("In Memory " + a("cache", "cache"))
                   + li("ping the system <a href='isalive'>isalive</a>")
                   + "</ol>"

                   + h2("Global Types")
                   + "<ol>"
                   + li("global types in cache: " + a(globTypes, globTypes))
                   + li("global types loading log: " + a(globTypesLog, globTypesLog))
                   + "</ol>"

                   + h2("Manual links to access debug information")
                   + "<ol>"
                   + li("flush an app cache: " + a(purgeapp, purgeapp))
                   + li("look at the load-log of an app-cache: <a href='loadlog?appid='>loadlog?appid=</a>")
                   + li("look at the cache-stats of an app: <a href='stats?appid='>stats?appid=</a>")
                   + li("look at the content-types of an app: <a href='types?appid='>types?appid=</a>")
                   + li("look at attributes of a type: " + a(typeattribs, typeattribs))
                   + li("look at type metadata:" + a(typeMeta, typeMeta))
                   + li("look at type permissions:" + a(typePerms, typePerms))
                   + li("look at attribute Metadata :" + a(attribMeta, attribMeta))
                   + li("look at attribute permissions:" + a(attribPerms, attribPerms))
                   + li("look at entities of a type:" + a(entities, entities))
                   + li("look at all entities:" + a(entitiesAll, entitiesAll))
                   + li("look at a single entity by id:" + a(entity, entity))
                   + li("look at entity metadata using entity-id:" + a(entityMeta, entityMeta))
                   + li("look at entity permissions using entity-id:" + a(entityPerms, entityPerms))
                   + "<ol>"
                ;
            logWrap("ok");
            return result;
        }
    }
}
