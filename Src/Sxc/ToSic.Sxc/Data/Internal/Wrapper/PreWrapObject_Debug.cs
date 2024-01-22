using System.Reflection;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;

namespace ToSic.Sxc.Data.Internal.Wrapper;

partial class PreWrapObject
{
    public const string DumpSourceName = "DynamicRead";

    [PrivateApi]
    public override List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path)
    {
        if (_innerObject == null) return [];

        if (string.IsNullOrEmpty(path)) path = DumpSourceName;

        var allProperties = PropDic.ToList<KeyValuePair<string, PropertyInfo>>();

        var simpleProps = allProperties;
        var resultDynChildren = simpleProps.Select(p => new
            {
                Field = p.Key,
                Pdi = new PropertyDumpItem
                {
                    Path = path + PropertyDumpItem.Separator + p.Key,
                    Property = FindPropertyInternal(specs.ForOtherField(p.Key), new PropertyLookupPath().Add("DynReadObject", p.Key)),
                    SourceName = DumpSourceName
                }
            })
            .ToList();

        var deeperProperties = resultDynChildren
            .Where(r =>
            {
                var result = r.Pdi.Property.Result;
                return result != null && result is not string && !result.GetType().IsValueType;
            }).Select(p =>
            {
                var maybeDump = _wrapper.ChildNonJsonWrapIfPossible(data: p.Pdi.Property.Result, wrapNonAnon: false,
                    WrapperSettings.Dyn(children: true, realObjectsToo: true));
                return new
                {
                    p.Field,
                    CanDump = maybeDump as IPropertyLookup ?? (maybeDump as IHasPropLookup)?.PropertyLookup,
                };
            })
            .Where(p => p.CanDump is not null)
            .ToList();
        var deeperLookups = deeperProperties.SelectMany(p =>
            p.CanDump._Dump(specs, path + PropertyDumpItem.Separator + p.Field));

        var final = resultDynChildren
            .Where(r => deeperProperties.All(dp => dp.Field != r.Field))
            .Select(r => r.Pdi)
            .ToList();

        final.AddRange(deeperLookups);

        return final.OrderBy(p => p.Path).ToList();
    }

}