//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using ToSic.Eav.Data.Debug;
//using ToSic.Eav.Data.PropertyLookup;
//using ToSic.Lib.Documentation;

//namespace ToSic.Sxc.Data.DynamicWrapper
//{
//    internal class WrapObjectHelpers
//    {
//        public static Dictionary<string, PropertyInfo> CreateDictionary(object original)
//        {
//            var dic = new Dictionary<string, PropertyInfo>(StringComparer.InvariantCultureIgnoreCase);
//            if (original is null) return dic;
//            var itemType = original.GetType();
//            foreach (var propertyInfo in itemType.GetProperties())
//                dic[propertyInfo.Name] = propertyInfo;
//            return dic;
//        }

//        public const string DumpSourceName = "DynamicRead";

//        [PrivateApi]
//        internal List<PropertyDumpItem> Dump(DynamicReadObject parent, DynamicWrapperFactory WrapperFactory, PropReqSpecs specs, string path)
//        {
//            if (parent.GetContents() == null) return new List<PropertyDumpItem>();

//            if (string.IsNullOrEmpty(path)) path = DumpSourceName;

//            //var allProperties = _ignoreCaseLookup.ToList();

//            //var simpleProps = allProperties;
//            var keys = (parent as IHasKeys).Keys(); // simpleProps.Select(p => p.Key);
//            var resultDynChildren = keys.Select(key => new // simpleProps.Select(p => new
//                {
//                    Field = key,
//                    Pdi = new PropertyDumpItem
//                    {
//                        Path = path + PropertyDumpItem.Separator + key,
//                        Property = parent.FindPropertyInternal(specs.ForOtherField(key), new PropertyLookupPath().Add("DynReadObject", key)),
//                        SourceName = DumpSourceName
//                    }
//                })
//                .ToList();

//            var deeperProperties = resultDynChildren
//                .Where(r =>
//                {
//                    var result = r.Pdi.Property.Result;
//                    return result != null && !(result is string) && !result.GetType().IsValueType;
//                }).Select(p => new
//                {
//                    p.Field,
//                    CanDump = WrapperFactory.WrapIfPossible(value: p.Pdi.Property.Result, wrapRealObjects: false, wrapChildren: true, wrapRealChildren: true, wrapIntoTyped: false) as IPropertyLookup
//                })
//                .Where(p => !(p.CanDump is null))
//                .ToList();
//            var deeperLookups = deeperProperties.SelectMany(p =>
//                p.CanDump._Dump(specs, path + PropertyDumpItem.Separator + p.Field));

//            var final = resultDynChildren
//                .Where(r => deeperProperties.All(dp => dp.Field != r.Field))
//                .Select(r => r.Pdi)
//                .ToList();

//            final.AddRange(deeperLookups);

//            return final.OrderBy(p => p.Path).ToList();
//        }
//    }
//}
