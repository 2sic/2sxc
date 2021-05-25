using Oqtane.Models;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Oqt.Server.Run;

namespace ToSic.Sxc.Oqt.Server.LookUps
{
    public class ModuleLookUp : LookUpBase
    {
        private readonly OqtState _oqtState;
        private Module Module { get; set; }

        public ModuleLookUp(OqtState oqtState)
        {
            Name = "Module";

            _oqtState = oqtState;
        }

        public Module GetSource()
        {
            var ctx = _oqtState.GetContext();
            var module = (OqtModule)ctx.Module;
            return module.UnwrappedContents;
        }

        public override string Get(string key, string format)
        {
            try
            {
                Module ??= GetSource();

                return key.ToLowerInvariant() switch
                {
                    "id" => $"{Module.ModuleId}",
                    _ => string.Empty
                };
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}