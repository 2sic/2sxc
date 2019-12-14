using System.Web.Http;

namespace ToSic.Sxc.WebApi.System
{
    public partial class InsightsController
    {

        [HttpGet]
        public string Help()
        {
            var logWrap = Log.Call();
            ThrowIfNotSuperuser();

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
                       + a("blog post", "https://2sxc.org/en/blog/post/using-2sxc-insights", true))
                   + "<ol>"
                   + li("browse the in-memory " + a("cache", "cache"))
                   + "<li>look at <a href='help'>help</a> (this screen)</li>"
                   + "<li>ping the system to see if it's alive <a href='isalive'>isalive</a></li>\n"
                   + li("look at all kinds of logs: " + a(logs, logs))
                   + "<li>look at the load-log of an app-cache: <a href='loadlog?appid='>loadlog?appid=</a></li>\n"
                   + "<li>look at the cache-stats of an app: <a href='stats?appid='>stats?appid=</a></li>\n"
                   + li("flush an app cache: " + a(purgeapp, purgeapp))
                   + li("global types in cache: " + a(globTypes, globTypes))
                   + li("global types loading log: " + a(globTypesLog, globTypesLog))
                   + "<li>look at the content-types of an app: <a href='types?appid='>types?appid=</a></li>\n"
                   + li("look at attributes of a type: " + a(typeattribs, typeattribs))
                   + li("look at type metadata:" + a(typeMeta, typeMeta))
                   + li("look at type permissions:" + a(typePerms, typePerms)) 
                   + li("look at attribute Metadata :" + a(attribMeta, attribMeta)) 
                   + li("look at attribute permissions:" + a(attribPerms, attribPerms))
                   + li("look at entities of a type:" + a(entities, entities))
                   + li("look at all entities:" + a(entitiesAll, entitiesAll))
                   + li("look at a sigle entity by id:" + a(entity, entity))
                   + li("look at entity metadata using entity-id:" + a(entityMeta, entityMeta))
                   + li("look at entity permissions using entity-id:" + a(entityPerms, entityPerms))
                   + "<ol>"
                ;
            logWrap("ok");
            return result;
        }
        
    }
}