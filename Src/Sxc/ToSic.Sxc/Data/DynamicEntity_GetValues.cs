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
            #region check the two special cases Toolbar / Presentation which the EAV doesn't know
#if NETFRAMEWORK
            // ReSharper disable once ConvertIfStatementToSwitchStatement
#pragma warning disable 618
            if (field == "Toolbar") return Toolbar.ToString();
#pragma warning restore 618
#endif
            if (field == ViewParts.Presentation) return Presentation;

            #endregion

            return base.GetInternal(field, language, lookup);
        }

        [PrivateApi("Internal")]
        public override PropertyRequest FindPropertyInternal(string field, string[] dimensions, ILog parentLogOrNull)
        {
            var logOrNull = parentLogOrNull.SubLogOrNull("Sxc.DynEnt");
            var wrapLog = logOrNull.SafeCall<PropertyRequest>(null);
            // check Entity is null (in cases where null-objects are asked for properties)
            if (Entity == null) return wrapLog("no entity", null);
            var t = Entity.FindPropertyInternal(field, dimensions, logOrNull);
            t.Name = "dynamic";
            return wrapLog(null, t);
        }

    }
}
