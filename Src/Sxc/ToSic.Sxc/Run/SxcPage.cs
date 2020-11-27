using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Run
{
    public class SxcPage: IPage
    {
        private readonly Lazy<IHttp> _httpLazy;

        /// <summary>
        /// Empty constructor for DI
        /// </summary>
        public SxcPage(Lazy<IHttp> httpLazy)
        {
            _httpLazy = httpLazy;
        }

        public IPage Init(int id)
        {
            Id = id;
            return this;
        }

        public int Id { get; private set; } = Eav.Constants.NullId;

        public List<KeyValuePair<string, string>> Parameters
        {
            get => _parameters ?? (_parameters = _httpLazy.Value?.QueryStringKeyValuePairs() ?? new List<KeyValuePair<string, string>>());
            set => _parameters = value ?? _parameters;
        }
        private List<KeyValuePair<string, string>> _parameters;

        public string Url { get; set; } = Eav.Constants.UrlNotInitialized;
    }
}
