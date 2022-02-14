using static System.StringComparison;

namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Insights
    {
        /// <summary>
        /// WIP
        /// Trying to simplify access to all the features of Insights
        /// Using a single API endpoint which Dnn/Oqtane etc. must implement
        /// This to ensure that the Insights-Endpoint doesn't need changes to support more commands
        /// </summary>
        /// <returns></returns>
        public string Details(string view, int? appId, string key, int? position, string type, bool? toggle, string nameId)
        {
            // This is really important
            ThrowIfNotSuperUser();

            view = view.ToLowerInvariant();

            if (view.Equals(nameof(Help), InvariantCultureIgnoreCase)) return Help();

            if (view.Equals(nameof(Licenses), InvariantCultureIgnoreCase)) return Licenses();

            if (view.Equals(nameof(IsAlive), InvariantCultureIgnoreCase)) return IsAlive();

            if (view.Equals(nameof(GlobalTypes), InvariantCultureIgnoreCase)) return GlobalTypes();
            if (view.Equals(nameof(GlobalTypesLog), InvariantCultureIgnoreCase)) return GlobalTypesLog();

            if (view.Equals(nameof(Logs), InvariantCultureIgnoreCase))
                return key == null
                    ? Logs()
                    : position == null
                        ? Logs(key)
                        : Logs(key, position.Value);

            if (view.Equals(nameof(LogsFlush), InvariantCultureIgnoreCase)) return LogsFlush(key);
            if (view.Equals(nameof(PauseLogs), InvariantCultureIgnoreCase)) return PauseLogs(toggle ?? true);

            // Cache and Cache-Details
            if (view.Equals(nameof(Cache), InvariantCultureIgnoreCase)) return Cache();
            if (view.Equals(nameof(LoadLog), InvariantCultureIgnoreCase)) return LoadLog(appId);
            if (view.Equals(nameof(Stats), InvariantCultureIgnoreCase)) return Stats(appId);
            if (view.Equals(nameof(Types), InvariantCultureIgnoreCase)) return Types(appId);
            if (view.Equals(nameof(Purge), InvariantCultureIgnoreCase)) return Purge(appId);

            // Cache: Entities and Details, Attributes, Metadata etc.
            if (view.Equals(nameof(Entities), InvariantCultureIgnoreCase)) return Entities(appId, type);
            if (view.Equals(nameof(Entity), InvariantCultureIgnoreCase)) return Entity(appId, nameId);
            if (view.Equals(nameof(EntityMetadata), InvariantCultureIgnoreCase)) return EntityMetadata(appId, int.Parse(nameId));
            if (view.Equals(nameof(EntityPermissions), InvariantCultureIgnoreCase)) return EntityPermissions(appId, int.Parse(nameId));

            if (view.Equals(nameof(Attributes), InvariantCultureIgnoreCase)) return Attributes(appId, type);
            if (view.Equals(nameof(AttributeMetadata), InvariantCultureIgnoreCase)) return AttributeMetadata(appId, type, nameId);
            if (view.Equals(nameof(AttributePermissions), InvariantCultureIgnoreCase)) return AttributePermissions(appId, type, nameId);
            if (view.Equals(nameof(TypeMetadata), InvariantCultureIgnoreCase)) return TypeMetadata(appId, type);
            if (view.Equals(nameof(TypePermissions), InvariantCultureIgnoreCase)) return TypePermissions(appId, type);

            return $"Error: View name {view} unknown";
        }
    }
}
