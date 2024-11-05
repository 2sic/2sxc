using System;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Web.Internal.PageService;

namespace ToSic.Sxc.Oqt.Server.Blocks.Output;

partial class OqtPageOutput
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <remarks>New in 12.02</remarks>
    //public IEnumerable<OqtPagePropertyChanges> GetPagePropertyChanges()
    //{
    //    var wrapLog = Log.Call<IEnumerable<OqtPagePropertyChanges>>();
    //    // If we get something invalid, return 0 (nothing changed)
    //    if (!(PageServiceShared is IChangeQueue changes)) return wrapLog($"not {nameof(IChangeQueue)}", new OqtPagePropertyChanges[] { });

    //    var props = changes.GetPropertyChangesAndFlush();
    //    var result = GetOqtPagePropertyChangesList(props);

    //    var count = props.Count;

    //    return wrapLog($"Changes: {count}", result);
    //}

    public IEnumerable<OqtPagePropertyChanges> GetOqtPagePropertyChangesList(IList<PagePropertyChange> props)
    {
        var l = Log.Fn<IEnumerable<OqtPagePropertyChanges>>();

        var result = new List<OqtPagePropertyChanges>();
        foreach (var p in props)
            switch (p.Property)
            {
                case PageProperties.Base:
                    result.Add(new() { Property = OqtPageProperties.Base, Value = p.Value });
                    break;
                case PageProperties.Title:
                    result.Add(new()
                    {
                        Property = OqtPageProperties.Title, Value = p.Value, Placeholder = p.ReplacementIdentifier,
                        Change = GetOp(p.ChangeMode)
                    });
                    break;
                case PageProperties.Description:
                    result.Add(new()
                    {
                        Property = OqtPageProperties.Description, Value = p.Value, Placeholder = p.ReplacementIdentifier,
                        Change = GetOp(p.ChangeMode)
                    });
                    break;
                case PageProperties.Keywords:
                    result.Add(new()
                    {
                        Property = OqtPageProperties.Keywords, Value = p.Value, Placeholder = p.ReplacementIdentifier,
                        Change = GetOp(p.ChangeMode)
                    });
                    break;
                default: // ignore
                    break;
            }

        var count = props.Count;

        return l.Return(result, $"Changes: {count}");
    }

    private static OqtPagePropertyOperation GetOp(PageChangeModes changeMode)
    {

        switch (changeMode)
        {
            // The core 3 properties default to prefix
            case PageChangeModes.Default:
            case PageChangeModes.Auto:
                return OqtPagePropertyOperation.Prefix;
            case PageChangeModes.Replace:
                return OqtPagePropertyOperation.Replace;
            case PageChangeModes.Append:
                return OqtPagePropertyOperation.Suffix;
            case PageChangeModes.Prepend:
                return OqtPagePropertyOperation.Prefix;
            case PageChangeModes.ReplaceOrSkip:
                return OqtPagePropertyOperation.ReplaceOrSkip;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}