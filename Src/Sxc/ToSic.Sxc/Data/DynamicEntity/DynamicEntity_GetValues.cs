using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        [PrivateApi]
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // Check special cases #1 Toolbar - only in DNN and only on the explicit dynamic entity; not available in Oqtane
#if NETFRAMEWORK
#pragma warning disable 618 // ignore Obsolete
            if (binder.Name == "Toolbar")
            {
                result = Toolbar.ToString();
                return true;
            }
#pragma warning restore 618
#endif
            // 2023-08-11 2dm disabled this, don't think it's ever hit, because Presentation is a real property so it's
            // picked up before the binder. Disable and monitor, remove ca. end of August 2023
            //// Check #2 Presentation which the EAV doesn't know
            //// but only pre V16 (Pro) code. Newer code MUST use the .Presentation
            //if (_Cdf.CompatibilityLevel < Constants.CompatibilityLevel16 && binder.Name == ViewParts.Presentation)
            //{
            //    result = Presentation;
            //    return true;
            //}

            return base.TryGetMember(binder, out result);
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
            var isPath = specs.Field.Contains(PropertyStack.PathSeparator);
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
