using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using ToSic.Sxc.Context.Query;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Context
{
    public class Page: IPage
    {
        #region Constructor / DI

        /// <summary>
        /// Constructor for DI
        /// </summary>
        public Page(Lazy<IHttp> httpLazy) => _httpLazy = httpLazy;
        private readonly Lazy<IHttp> _httpLazy;

        #endregion

        public IPage Init(int id)
        {
            Id = id;
            return this;
        }

        public int Id { get; private set; } = Eav.Constants.NullId;

        public List<KeyValuePair<string, string>> ParametersInternalOld
        {
            get => _paramsInternalOld ?? (_paramsInternalOld = _httpLazy.Value?.QueryStringKeyValuePairs() ?? new List<KeyValuePair<string, string>>());
            set => _paramsInternalOld = value ?? _paramsInternalOld;
        }
        private List<KeyValuePair<string, string>> _paramsInternalOld;

        public IReadOnlyDictionary<string, string> Parameters => _parameters ?? (_parameters = new Parameters(_httpLazy.Value?.QueryStringParams));
        private IReadOnlyDictionary<string, string> _parameters;


        public string Url { get; set; } = Eav.Constants.UrlNotInitialized;
    }
}
