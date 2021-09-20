using Oqtane.Models;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Run;

namespace ToSic.Sxc.Oqt.Server.LookUps
{
    // TODO: @STV pls test, I changed oqtState to CtxResolver
    // Afterwards pls Rename to OqtModuleLookUp
    public class ModuleLookUp : LookUpBase
    {
        private Module Module { get; set; }

        public ModuleLookUp(IContextResolver ctxResolver)
        {
            Name = "Module";
            _ctxResolver = ctxResolver;
        }

        private readonly IContextResolver _ctxResolver;

        public Module GetSource()
        {
            if (_alreadyTried) return null;
            _alreadyTried = true;
            var ctx = _ctxResolver.BlockOrNull();
            var module = (OqtModule)ctx?.Module;
            return module?.UnwrappedContents;
        }

        private bool _alreadyTried;

        public override string Get(string key, string format)
        {
            try
            {
                Module ??= GetSource();

                if (Module == null) return null;

                return key.ToLowerInvariant() switch
                {
                    "id" => $"{Module.ModuleId}",       // Todo: must ensure ID would also work in Dnn
                    "moduleid" => "Warning: 'PageId' was requested, but the page source can only answer to Id", // warning for compatibility with older Dnn implementation
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