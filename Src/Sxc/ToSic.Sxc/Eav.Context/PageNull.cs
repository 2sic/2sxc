using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace ToSic.Eav.Context
{
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
