using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Context.Query;

namespace ToSic.Sxc.Context
{
    [PrivateApi]
    public class PageUnknown: IPage
    {
        public IPage Init(int id)
        {
            Id = id;
            return this;
        }
        
        public int Id { get; private set; } = Eav.Constants.NullId;

        public string Url => Eav.Constants.UrlNotInitialized;

        public List<KeyValuePair<string, string>> ParametersInternalOld
        {
            get => _parameters ?? (_parameters = new List<KeyValuePair<string, string>>());
            set => _parameters = value;
        }
        private List<KeyValuePair<string, string>> _parameters;

        public IReadOnlyDictionary<string, string> Parameters => new Parameters(null);

    }
}
