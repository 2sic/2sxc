using ToSic.Eav.Data;
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
            var safeWrap = logOrNull.SafeCall<PropertyRequest>($"{nameof(field)}", "DynEntity");
            // check Entity is null (in cases where null-objects are asked for properties)
            if (Entity == null) return safeWrap("no entity", null);
            var propRequest = Entity.FindPropertyInternal(field, dimensions, logOrNull);

            // Note 2021-06-22 believe this isn't necessary any more, because it already happens at the EntityWithStackNav level
            //if (!propRequest.IsFinal)
            //{
            //    var result = Entity.TryToNavigateToEntityInList(field, this, logOrNull);
            //    logOrNull?.SafeAdd(result == null ? "not-found" : "entity-in-list");
            //    propRequest = result ?? propRequest;
            //}
            //else
                propRequest.Name = "dynamic";

            //// In special edge-cases (Settings with Sub-List Navigation) the result
            //if (propRequest.Result is IEntity entityResult) propRequest.Result = SubDynEntity(entityResult);
            return safeWrap(null, propRequest);
        }

    }
}
