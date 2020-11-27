using System.Collections.Generic;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Run.Context
{
    public class PageNull: IPageInternal
    {
        public int Id { get; private set; } = Eav.Constants.NullId;
        public string Url => null;

        public List<KeyValuePair<string, string>> Parameters
        {
            get => _parameters ?? (_parameters = new List<KeyValuePair<string, string>>());
            set => _parameters = value;
        }

        public IPageInternal Init(int id)
        {
            Id = id;
            return this;
        }

        private List<KeyValuePair<string, string>> _parameters;
    }
}
