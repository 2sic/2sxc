using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Data;
using ToSic.Sxc.Data.Typed;
using ToSic.Sxc.Data.Wrapper;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    internal class PreWrapJsonArray: PreWrapJsonBase, IWrapper<JsonArray>
    {
        public PreWrapJsonArray(CodeJsonWrapper wrapper, JsonArray item): base(wrapper)
        {
            UnwrappedContents = item;
        }

        protected readonly JsonArray UnwrappedContents;

        public JsonArray GetContents() => UnwrappedContents;

        public override object JsonSource => UnwrappedContents;

        #region Keys

        public override IEnumerable<string> Keys(string noParamOrder = Protector, IEnumerable<string> only = default)
            => TypedHelpers.FilterKeysIfPossible(noParamOrder, only, UnwrappedContents?.Select((p, i) => i.ToString()));

        public override bool ContainsKey(string name)
        {
            if (name.IsEmpty() || UnwrappedContents == null) return false;
            var index = name.ConvertOrFallback<int>(fallback: -1, numeric: true);
            if (index == -1) return false;
            return index < UnwrappedContents.Count;
        }

        #endregion

        public override TryGetResult TryGetWrap(string name, bool wrapDefault = true)
        {
            if (UnwrappedContents == null || !UnwrappedContents.Any())
                return new TryGetResult(false, null, null);

            var found = UnwrappedContents.FirstOrDefault(p =>
            {
                if (!(p is JsonObject pJObject)) return false;
                return HasPropertyWithValue(pJObject, "Name", name)
                       || HasPropertyWithValue(pJObject, "Title", name);
            });

            return new TryGetResult(false, found,
                Wrapper.IfJsonGetValueOrJacket(found));
        }


        private static bool HasPropertyWithValue(JsonObject obj, string propertyName, string value)
        {
            if (obj.TryGetPropertyValue(propertyName, out var propVal) && propVal is JsonValue jVal && jVal.TryGetValue<string>(out var strVal))
                return strVal.EqualsInsensitive(value);

            return false;

        }

        public override List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path)
            => new List<PropertyDumpItem> { new PropertyDumpItem { Path = $"Not supported on {nameof(DynamicJacketList)}" } };


    }
}
