using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Logging;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    public partial class DynamicJacket
    {
        private const string _dumpSourceName = "Dynamic";
        [PrivateApi("internal")]
        public override List<PropertyDumpItem> _Dump(string[] languages, string path, ILog parentLogOrNull)
        {
            if (_contents == null || !_contents.Any()) return new List<PropertyDumpItem>();

            if (string.IsNullOrEmpty(path)) path = _dumpSourceName;

            var allProperties = _contents.ToList();

            var simpleProps = allProperties.Where(p => !(p.Value is JsonObject));
            var resultDynChildren = simpleProps.Select(p => new PropertyDumpItem
                {
                    Path = path + PropertyDumpItem.Separator + p.Key,
                    Property = FindPropertyInternal(p.Key, languages, parentLogOrNull, new PropertyLookupPath().Add("DynJacket", p.Key)),
                    SourceName = _dumpSourceName
            })
                .ToList();

            var objectProps = allProperties
                .Where(p => p.Value is JsonObject)
                .SelectMany(p =>
                {
                    var jacket = new DynamicJacket(p.Value.AsObject());
                    return jacket._Dump(languages, path + PropertyDumpItem.Separator + p.Key, parentLogOrNull);
                })
                .Where(p => !(p is null));

            resultDynChildren.AddRange(objectProps);

            // TODO: JArrays

            return resultDynChildren.OrderBy(p => p.Path).ToList();
        }

    }
}
