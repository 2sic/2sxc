using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        [PrivateApi]
        protected override object GetInternal(string field, string language = null, bool lookup = true)
        {
            // Check special cases #1 Toolbar - only in DNN, not available in Oqtane
#if NETFRAMEWORK
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            #pragma warning disable 618 - ignore Obsolete
            if (field == "Toolbar") return Toolbar.ToString();
            #pragma warning restore 618
#endif

            // Check #2 Presentation which the EAV doesn't know
            if (field == ViewParts.Presentation) return Presentation;

            return base.GetInternal(field, language, lookup);
        }

        [PrivateApi("Internal")]
        public override PropertyRequest FindPropertyInternal(string field, string[] dimensions, ILog parentLogOrNull)
        {
            var logOrNull = parentLogOrNull.SubLogOrNull("Sxc.DynEnt");
            var safeWrap = logOrNull.SafeCall<PropertyRequest>($"{nameof(field)}: {field}", "DynEntity");
            // check Entity is null (in cases where null-objects are asked for properties)
            if (Entity == null) return safeWrap("no entity", null);
            var propRequest = Entity.FindPropertyInternal(field, dimensions, logOrNull);

            return safeWrap(null, propRequest);
        }

        [PrivateApi("WIP / internal")]
        public override List<PropertyDumpItem> _Dump(string[] languages, string path, ILog parentLogOrNull)
        {
            if (Entity == null || !Entity.Attributes.Any()) return new List<PropertyDumpItem>();

            return Entity._Dump(languages, path, parentLogOrNull);
            //var result =
            //    Entity.Attributes
            //        .Select(att => new PropertyDumpItem { Path = path + att.Key, Value = Get(att.Key) })
            //        .ToList();
            //return result;
        }

    }
}
