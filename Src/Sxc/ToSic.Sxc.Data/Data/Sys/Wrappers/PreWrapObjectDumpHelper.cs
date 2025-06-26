using System.Reflection;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.PropertyDump;

namespace ToSic.Sxc.Data.Sys.Wrappers;
internal class PreWrapObjectDumpHelper
{
    public const string DumpSourceName = "DynamicRead";


    public List<PropertyDumpItem> _Dump(PreWrapObject preWrap, Dictionary<string, PropertyInfo> propDic,
        ICodeDataPoCoWrapperService wrapperSvc, PropReqSpecs specs, string path, IPropertyDumpService dumpService)
    {
        if (string.IsNullOrEmpty(path))
            path = DumpSourceName;

        var allProperties = propDic.ToList();

        var simpleProps = allProperties;
        var resultDynChildren = simpleProps
            .Select(p => new
            {
                Field = p.Key,
                Pdi = new PropertyDumpItem
                {
                    Path = path + PropertyDumpItem.Separator + p.Key,
                    Property = preWrap.FindPropertyInternal(specs.ForOtherField(p.Key),
                        new PropertyLookupPath().Add("DynReadObject", p.Key)),
                    SourceName = DumpSourceName
                }
            })
            .ToList();

        var deeperProperties = resultDynChildren
            .Where(r =>
            {
                var result = r.Pdi.Property?.Result;
                return result != null && result is not string && !result.GetType().IsValueType;
            })
            .Select(p =>
            {
                var maybeDump = wrapperSvc.ChildNonJsonWrapIfPossible(data: p.Pdi.Property!.Result, wrapNonAnon: false,
                    WrapperSettings.Dyn(children: true, realObjectsToo: true));
                return new
                {
                    p.Field,
                    CanDump = maybeDump as IPropertyLookup ?? (maybeDump as IHasPropLookup)?.PropertyLookup,
                };
            })
            .Where(p => p.CanDump is not null)
            .ToList();
        var deeperLookups = deeperProperties
            .SelectMany(p => (p.CanDump as IPropertyDumpCustom)?._DumpProperties(specs, path + PropertyDumpItem.Separator + p.Field, dumpService) ?? []);

        var final = resultDynChildren
            .Where(r => deeperProperties.All(dp => dp.Field != r.Field))
            .Select(r => r.Pdi)
            .ToList();

        final.AddRange(deeperLookups);

        return final
            .OrderBy(p => p.Path)
            .ToList();
    }
}
