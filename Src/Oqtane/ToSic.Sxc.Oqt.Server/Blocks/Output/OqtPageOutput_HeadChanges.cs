using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Server.Blocks.Output;

partial class OqtPageOutput
{
    public IEnumerable<OqtHeadChange> GetHeadChanges()
    {
        var l = Log.Fn<IEnumerable<OqtHeadChange>>();

        var changes = RenderResult.HeadChanges;

        var result = changes.Select(p => new OqtHeadChange
        {
            PropertyOperation = GetOp(p.ChangeMode),
            Tag = p.Tag.ToString(),
            ReplacementIdentifier = p.ReplacementIdentifier,
        });

        var count = changes.Count;

        return l.ReturnAndLog(result, $"Changes: {count}");
    }
}