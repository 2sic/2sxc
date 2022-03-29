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
            var logOrNull = parentLogOrNull.SubLogOrNull("Sxc.DynEnt", Debug);
            var safeWrap = logOrNull.SafeCall<PropertyRequest>($"{nameof(field)}: {field}", "DynEntity");
            // check Entity is null (in cases where null-objects are asked for properties)
            if (Entity == null) return safeWrap("no entity", null);
            var propRequest = Entity.FindPropertyInternal(field, dimensions, logOrNull);

            // new 12.05, very experimental
            //ApplyDynamicDataFeaturesToResult(field, propRequest);

            return safeWrap(null, propRequest);
        }

        // V12.10? This is just PoC to show that we could auto-dynamic data. Will not be available yet
        private void ApplyDynamicDataFeaturesToResult(string field, PropertyRequest propRequest)
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
        public override List<PropertyDumpItem> _Dump(string[] languages, string path, ILog parentLogOrNull)
        {
            if (Entity == null || !Entity.Attributes.Any()) return new List<PropertyDumpItem>();
            return Entity._Dump(languages, path, parentLogOrNull);
        }

    }
}
