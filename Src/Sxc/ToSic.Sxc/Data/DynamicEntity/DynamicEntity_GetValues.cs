using System.Collections.Generic;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Documentation;

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
            => PreWrap.FindPropertyInternal(specs, path);

        [PrivateApi("WIP / internal")]
        public override List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path)
            => PreWrap._Dump(specs, path);
    }
}
