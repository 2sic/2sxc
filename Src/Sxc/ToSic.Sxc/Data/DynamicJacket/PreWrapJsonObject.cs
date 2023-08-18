using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json.Nodes;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Data;
using ToSic.Sxc.Data.Typed;
using ToSic.Sxc.Data.Wrapper;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    internal class PreWrapJsonObject: PreWrapJsonBase, IWrapper<JsonObject>
    {

        internal PreWrapJsonObject(CodeJsonWrapper wrapper, JsonObject item, WrapperSettings settings): base(wrapper, settings)
        {
            UnwrappedContents = item;
        }

        protected readonly JsonObject UnwrappedContents;

        public JsonObject GetContents() => UnwrappedContents;

        public override object JsonSource => UnwrappedContents;

        #region Keys

        public override IEnumerable<string> Keys(string noParamOrder = Protector, IEnumerable<string> only = default) 
            => TypedHelpers.FilterKeysIfPossible(noParamOrder, only, UnwrappedContents.Select(p => p.Key));

        public override bool ContainsKey(string name)
            => !name.IsEmpty() && UnwrappedContents.Any(p => name.EqualsInsensitive(p.Key));

        #endregion

        #region TryGet

        public override TryGetResult TryGetWrap(string name, bool wrapDefault = true)
        {
            if (name.IsEmptyOrWs() || UnwrappedContents == null || !UnwrappedContents.Any())
                return new TryGetResult(false, null, null);

            var isPath = name.Contains(PropertyStack.PathSeparator.ToString());
            if (!isPath)
                return TryGetFromNode(name, UnwrappedContents);

            var pathParts = PropertyStack.SplitPathIntoParts(name);
            var node = UnwrappedContents;
            for (var i = 0; i < pathParts.Length; i++)
            {
                var part = pathParts[i];
                var result = TryGetFromNode(part, node);
                // last one or not found - return a not-found
                if (i == pathParts.Length -1 || !result.Found) return result;
                node = result.Raw as JsonObject;
                if (node == null) return new TryGetResult(false, null, null);
            }
            return new TryGetResult(false, null, null);
        }

        private TryGetResult TryGetFromNode(string name, JsonObject node)
        {
            var result = node
                .FirstOrDefault(p => p.Key.EqualsInsensitive(name));

            var found = !result.Equals(default(KeyValuePair<string, JsonNode>));
            return new TryGetResult(found, result.Value, found ? Wrapper.IfJsonGetValueOrJacket(result.Value) : null);
        }

        #endregion

        #region Debug / Dump

        private const string DumpSourceName = "Dynamic";

        public override List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path)
        {
            if (UnwrappedContents == null || !UnwrappedContents.Any()) return new List<PropertyDumpItem>();

            if (string.IsNullOrEmpty(path)) path = DumpSourceName;

            var allProperties = UnwrappedContents.ToList();

            var simpleProps = allProperties.Where(p => !(p.Value is JsonObject));
            var resultDynChildren = simpleProps.Select(p => new PropertyDumpItem
                {
                    Path = path + PropertyDumpItem.Separator + p.Key,
                    Property = FindPropertyInternal(specs.ForOtherField(p.Key),
                        new PropertyLookupPath().Add("DynJacket", p.Key)),
                    SourceName = DumpSourceName
                })
                .ToList();

            var objectProps = allProperties
                .Where(p => p.Value is JsonObject)
                .SelectMany(p =>
                {
                    var jacket = Wrapper.CreateDynJacketObject(p.Value.AsObject());
                    return ((IHasPropLookup)jacket).PropertyLookup._Dump(specs, path + PropertyDumpItem.Separator + p.Key);
                })
                .Where(p => !(p is null));

            resultDynChildren.AddRange(objectProps);

            // TODO: JArrays

            return resultDynChildren.OrderBy(p => p.Path).ToList();
        }


        #endregion
    }
}
