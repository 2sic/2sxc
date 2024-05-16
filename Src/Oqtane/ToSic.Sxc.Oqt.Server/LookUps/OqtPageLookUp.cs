using ToSic.Eav.LookUp;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Oqt.Server.Context;
using static ToSic.Sxc.LookUp.LookUpConstants;

namespace ToSic.Sxc.Oqt.Server.LookUps;

internal class OqtPageLookUp(ISxcContextResolver ctxResolver) : LookUpBase(SourcePage, "LookUp in Oqtane Page")
{
    protected Oqtane.Models.Page Page { get; set; }

    public Oqtane.Models.Page GetSource()
    {
        if (_alreadyTried) return null;
        _alreadyTried = true;
        var ctx = ctxResolver.BlockContextOrNull();
        return ((OqtPage)ctx?.Page)?.GetContents();
    }
    private bool _alreadyTried;

    public override string Get(string key, string format)
    {
        try
        {
            Page ??= GetSource();

            return key.ToLowerInvariant() switch
            {
                KeyId => $"{Page.PageId}",
                OldDnnPageId => $"Warning: '{OldDnnPageId}' was requested, but the {nameof(OqtPageLookUp)} source can only answer to '{KeyId}'",
                "url" => $"{Page.Url}",
                _ => string.Empty
            };
        }
        catch
        {
            return string.Empty;
        }
    }
}