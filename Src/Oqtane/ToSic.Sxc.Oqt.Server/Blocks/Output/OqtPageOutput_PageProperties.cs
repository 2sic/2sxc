using System;
using System.Collections.Generic;
using ToSic.Sxc.Oqt.Shared.Models;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Oqt.Server.Blocks.Output
{
    public partial class OqtPageOutput
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
            var wrapLog = Log.Call<IEnumerable<OqtPagePropertyChanges>>();

            var result = new List<OqtPagePropertyChanges>();
            foreach (var p in props)
                switch (p.Property)
                {
                    case PageProperties.Base:
                        result.Add(new OqtPagePropertyChanges { Property = OqtPageProperties.Base, Value = p.Value });
                        break;
                    case PageProperties.Title:
                        result.Add(new OqtPagePropertyChanges
                        {
                            Property = OqtPageProperties.Title, Value = p.Value, Placeholder = p.ReplacementIdentifier,
                            Change = GetOp(p)
                        });
                        break;
                    case PageProperties.Description:
                        result.Add(new OqtPagePropertyChanges
                        {
                            Property = OqtPageProperties.Description, Value = p.Value, Placeholder = p.ReplacementIdentifier,
                            Change = GetOp(p)
                        });
                        break;
                    case PageProperties.Keywords:
                        result.Add(new OqtPagePropertyChanges
                        {
                            Property = OqtPageProperties.Keywords, Value = p.Value, Placeholder = p.ReplacementIdentifier,
                            Change = GetOp(p)
                        });
                        break;
                    default: // ignore
                        break;
                }

            var count = props.Count;

            return wrapLog($"Changes: {count}", result);
        }

        private static OqtPagePropertyOperation GetOp(PagePropertyChange change)
        {

            switch (change.ChangeMode)
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
}
