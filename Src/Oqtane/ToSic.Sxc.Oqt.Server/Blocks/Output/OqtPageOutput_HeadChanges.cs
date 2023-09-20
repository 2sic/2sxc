using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Logging;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Server.Blocks.Output;

public partial class OqtPageOutput
{
    public IEnumerable<OqtHeadChange> GetHeadChanges()
    {
        var wrapLog = Log.Fn<IEnumerable<OqtHeadChange>>();

        var changes = RenderResult.HeadChanges;

        var result = changes.Select(p => new OqtHeadChange
        {
            PropertyOperation = GetOp(p.ChangeMode),
            Tag = p.Tag.ToString(),
            ReplacementIdentifier = p.ReplacementIdentifier,
        });

        var count = changes.Count;

        return wrapLog.ReturnAndLog(result, $"Changes: {count}");
    }
}