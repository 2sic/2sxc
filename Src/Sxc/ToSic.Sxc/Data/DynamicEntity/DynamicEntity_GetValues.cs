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
        protected override object GetInternal(string field, string language = null, bool lookup = true)
        {
            // Check special cases #1 Toolbar - only in DNN, not available in Oqtane
#if NETFRAMEWORK
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            #pragma warning disable 618 // ignore Obsolete
            if (field == "Toolbar") return Toolbar.ToString();
            #pragma warning restore 618
#endif

            // Check #2 Presentation which the EAV doesn't know
            if (field == ViewParts.Presentation) return Presentation;

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
            // var propRequest = Entity.FindPropertyInternal(specs, path);

            // new 12.05, very experimental
            //ApplyDynamicDataFeaturesToResult(field, propRequest);

            return l.Return(propRequest, $"{nameof(isPath)}: {isPath}");
        }


        // V12.10? This is just PoC to show that we could auto-dynamic data. Will not be available yet
        private void ApplyDynamicDataFeaturesToResult(string field, PropReqResult propRequest)
        {
            if (propRequest.Value != null && propRequest.Result is string strResult)
            {
                if (propRequest.Value.DynamicUseCache == null)
                {
                    // determine if we should use a cache
                    try
                    {
                        var attDef = Entity.Type[field];
                        if (attDef?.InputType()?.StartsWith("custom-json") ?? false)
                        {
                            // try to convert json object
                            var jsonJacket = DynamicJacket.AsDynamicJacket(strResult);
                            propRequest.Value.DynamicCache = jsonJacket;
                            // Set true after storing data, so if anything broke, it will catch and be set to false
                            propRequest.Value.DynamicUseCache = true;
                        }
                    }
                    catch
                    {
                        // anything broke? cancel and don't try again
                        propRequest.Value.DynamicUseCache = false;
                    }
                }

                if (propRequest.Value.DynamicUseCache == true) propRequest.Result = propRequest.Value.DynamicCache;
            }
        }

        [PrivateApi("WIP / internal")]
        public override List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path) =>
            Entity == null || !Entity.Attributes.Any()
                ? new List<PropertyDumpItem>()
                : Entity._Dump(specs, path);
    }
}
