using System.Collections.Generic;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context
{
    [PrivateApi]
    public class PageNull: IPage
    {
        public int Id { get; private set; } = Eav.Constants.NullId;
        public string Url => null;

        public List<KeyValuePair<string, string>> Parameters
        {
            get => _parameters ?? (_parameters = new List<KeyValuePair<string, string>>());
            set => _parameters = value;
        }

        public IPage Init(int id)
        {
            Id = id;
            return this;
        }

        private List<KeyValuePair<string, string>> _parameters;
    }
}
