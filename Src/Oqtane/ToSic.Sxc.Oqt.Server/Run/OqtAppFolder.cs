using System;
using ToSic.Eav.Logging;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtAppFolder: HasLog<OqtAppFolder>
    {
        private readonly Lazy<OqtState> _oqtState;

        public OqtAppFolder(Lazy<OqtState> oqtState) : base($"{OqtConstants.OqtLogPrefix}.AppFolder")
        {
            _oqtState = oqtState;
        }

        public string GetAppFolder()
        {
            var ctx = _oqtState.Value.GetContext();
            return ctx.AppState.Folder;
        }
    }
}
