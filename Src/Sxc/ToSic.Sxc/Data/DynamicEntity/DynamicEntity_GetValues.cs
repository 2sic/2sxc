using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        [PrivateApi]
        protected override GetInternalResult GetInternal(string field, string language = null, bool lookup = true)
        {
            // Check special cases #1 Toolbar - only in DNN, not available in Oqtane
#if NETFRAMEWORK
            // ReSharper disable once ConvertIfStatementToSwitchStatement
#pragma warning disable 618 // ignore Obsolete
            if (field == "Toolbar") return new GetInternalResult(Toolbar.ToString(), true);
#pragma warning restore 618
#endif

            // Check #2 Presentation which the EAV doesn't know
            // but only pre V16 (Pro) code. Newer code MUST use the .Presentation
            if (_Services.AsC.CompatibilityLevel < Constants.CompatibilityLevel16 && field == ViewParts.Presentation)
                return new GetInternalResult(Presentation, Presentation != null);

            return base.GetInternal(field, language, lookup);
        }

        [PrivateApi("Internal")]
        public override PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path)
        {
            specs = specs.SubLog("Sxc.DynEnt", Debug);
            var l = specs.LogOrNull.Fn<PropReqResult>(specs.Dump(), "DynEntity");
            // check Entity is null (in cases where null-objects are asked for properties)
            if (Entity == null) return l.ReturnNull("no entity");
            if (!specs.Field.HasValue()) return l.ReturnNull("no path");

            path = path.KeepOrNew().Add("DynEnt", specs.Field);
            var isPath = specs.Field.Contains(".");
            var propRequest = !isPath
                ? Entity.FindPropertyInternal(specs, path)
                : PropertyStack.TraversePath(specs, path, Entity);
            return l.Return(propRequest, $"{nameof(isPath)}: {isPath}");
        }

        [PrivateApi("WIP / internal")]
        public override List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path) =>
            Entity == null || !Entity.Attributes.Any()
                ? new List<PropertyDumpItem>()
                : Entity._Dump(specs, path);
    }
}
