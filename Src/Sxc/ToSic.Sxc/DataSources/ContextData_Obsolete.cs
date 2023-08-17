#if NETFRAMEWORK
using ToSic.Eav.Code.InfoSystem;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data;
using static ToSic.Eav.Code.Infos.CodeInfoObsolete;

namespace ToSic.Sxc.DataSources
{
    internal partial class ContextData: IBlockDataSource
    {
        private readonly LazySvc<CodeInfoService> _codeChanges;

        private readonly ToSic.Eav.Apps.IAppStates _appStates;

#pragma warning disable 618
        [System.Obsolete("Old property on this data source, should really not be used at all. Must add warning in v13, and remove ca. v15")]
        [PrivateApi]
        public Compatibility.CacheWithGetContentType Cache
        {
            get
            {
                if (_cache != null) return _cache;
                // on first access report problem
                _codeChanges.Value.Warn(CaV8To17("Data.Cache", "https://go.2sxc.org/brc-13-datasource-cache"));
                return _cache = new Compatibility.CacheWithGetContentType(_appStates.Get(this));
            }
        }

        [System.Obsolete]
        private Compatibility.CacheWithGetContentType _cache;
#pragma warning restore 618

        [PrivateApi("older use case, probably don't publish")]
        public DataPublishing Publish { get; } = new DataPublishing();


        #region Prepared code if we need to reactivate some old APIs #DataInAddWontWork

        // Background: We've made our Data object immutable, so you cannot call Data.In.Add(...) any more
        // This used to work a long time ago, and was used in CustomizeData calls
        // But we're not sure if it's actually in use anywhere
        // Because the code sample we found (FAQ with Categories) had so many other problems
        // that it just didn't work
        // So we believe that maybe it hasn't worked for so long, that this kind of code may actually
        // not be around any more
        // 
        // because we never got complaints on this...


        //public void ToggleOldMode() => _oldMode = true;
        //private bool _oldMode;

        //public override IReadOnlyDictionary<string, IDataStream> Out
        //{
        //    get
        //    {
        //        // Make sure this matches the .net core implementation
        //        var source = PickOut;
        //        if (!_oldMode) return source;

        //        // find added "in"
        //        var realIn = base.In;
        //        var myIn = EditableIn;
        //        if (base.In.Count == myIn.Count) return source;

        //        var added = myIn
        //            .Where(pair => !realIn.ContainsKey(pair.Key)
        //                           || realIn.TryGetValue(pair.Key, out var origStream) && origStream != pair.Value);

        //        var result = realIn.ToDictionary(pair => pair.Key, pair => pair.Value, InvariantCultureIgnoreCase);
        //        foreach (var addition in added) 
        //            result[addition.Key] = addition.Value;

        //        return new ReadOnlyDictionary<string, IDataStream>(result);
        //    }
        //}

        //IReadOnlyDictionary<string, IDataStream> IDataSource.In => _oldMode ? new ReadOnlyDictionary<string, IDataStream>(EditableIn) : base.In;

        ///// <summary>
        ///// on old Razor we'll provide this as the In, so it can be added to
        ///// </summary>
        //IDictionary<string, IDataStream> IBlockDataSourceOld.In => EditableIn;

        //private IDictionary<string, IDataStream> EditableIn => _in.Get(() =>
        //    new Dictionary<string, IDataStream>(PickOut.ToDictionary(pair => pair.Key, pair => pair.Value, InvariantCultureIgnoreCase)));
        //private readonly GetOnce<IDictionary<string, IDataStream>> _in = new GetOnce<IDictionary<string, IDataStream>>();

        #endregion

    }
}
#endif
