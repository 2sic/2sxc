using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Data
{
    public partial class DynamicJacket
    {
        private const string _dumpSourceName = "Dynamic";
        [PrivateApi("internal")]
        public override List<PropertyDumpItem> _Dump(string[] languages, string path, ILog parentLogOrNull)
        {
            if (_contents == null || !_contents.HasValues) return new List<PropertyDumpItem>();

            if (string.IsNullOrEmpty(path)) path = _dumpSourceName;

            var allProperties = _contents.Properties().ToList();

            var simpleProps = allProperties.Where(p => !(p.Value is JObject));
            var resultDynChildren = simpleProps.Select(p => new PropertyDumpItem
                {
                    Path = path + PropertyDumpItem.Separator + p.Name,
                    Property = FindPropertyInternal(p.Name, languages, parentLogOrNull, new PropertyLookupPath().Add("DynJacket", p.Name)),
                    SourceName = _dumpSourceName
            })
                .ToList();

            var objectProps = allProperties
                .Where(p => p.Value is JObject)
                .SelectMany(p =>
                {
                    var jacket = new DynamicJacket((JObject)p.Value);
                    return jacket._Dump(languages, path + PropertyDumpItem.Separator + p.Name, parentLogOrNull);
                })
                .Where(p => !(p is null));

            resultDynChildren.AddRange(objectProps);

            // TODO: JArrays

            return resultDynChildren.OrderBy(p => p.Path).ToList();
        }

    }
}
