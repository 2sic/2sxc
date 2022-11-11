using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data
{
    public partial class DynamicReadObject
    {
        private const string _dumpSourceName = "DynamicRead";

        [PrivateApi]
        public List<PropertyDumpItem> _Dump(string[] languages, string path, ILog parentLogOrNull)
        {
            if (_contents == null) return new List<PropertyDumpItem>();

            if (string.IsNullOrEmpty(path)) path = _dumpSourceName;

            var allProperties = _ignoreCaseLookup.ToList();

            var simpleProps = allProperties;
            var resultDynChildren = simpleProps.Select(p => new
                {
                    Field = p.Key,
                    Pdi = new PropertyDumpItem
                    {
                        Path = path + PropertyDumpItem.Separator + p.Key,
                        Property = FindPropertyInternal(p.Key, languages, parentLogOrNull, new PropertyLookupPath().Add("DynReadObject", p.Key)),
                        SourceName = _dumpSourceName
                    }
                })
                .ToList();

            var deeperProperties = resultDynChildren
                .Where(r =>
                {
                    var result = r.Pdi.Property.Result;
                    return result != null && !(result is string) && !result.GetType().IsValueType;
                }).Select(p => new
                {
                    p.Field,
                    CanDump = DynamicHelpers.WrapIfPossible(p.Pdi.Property.Result, false, true, true) as IPropertyLookup
                })
                .Where(p => !(p.CanDump is null))
                .ToList();
            var deeperLookups = deeperProperties.SelectMany(p =>
                p.CanDump._Dump(languages, path + PropertyDumpItem.Separator + p.Field, null));

            var final = resultDynChildren
                .Where(r => deeperProperties.All(dp => dp.Field != r.Field))
                .Select(r => r.Pdi)
                .ToList();

            final.AddRange(deeperLookups);

            return final.OrderBy(p => p.Path).ToList();
        }

    }
}
