using Oqtane.Models;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Context;
using static ToSic.Sxc.LookUp.LookUpConstants;

namespace ToSic.Sxc.Oqt.Server.LookUps
{
    // TODO: @STV pls test, I changed oqtState to CtxResolver
    public class OqtModuleLookUp : LookUpBase
    {
        private Module Module { get; set; }

        public OqtModuleLookUp(IContextResolver ctxResolver)
        {
            Name = SourceModule;
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
                    KeyId => $"{Module.ModuleId}",
                    OldDnnModuleId => $"Warning: '{OldDnnModuleId}' was requested, but the {nameof(OqtModuleLookUp)} source can only answer to '{KeyId}'",
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