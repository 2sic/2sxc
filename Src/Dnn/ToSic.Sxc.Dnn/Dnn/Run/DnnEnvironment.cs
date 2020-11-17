using ToSic.Eav.Logging;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnEnvironment: HasLog, IEnvironment
    {
        #region Constructor and Init

        /// <summary>
        /// Constructor for DI, you must always call Init(...) afterwards
        /// </summary>
        public DnnEnvironment(ISite site) : base("DNN.Enviro")
        {
            _site = site;
        }

        private readonly ISite _site;

        public IEnvironment Init(ILog parent)
        {
            Log.LinkTo(parent);
            if (_site.Id == Eav.Constants.NullId)
                Log.Add("Warning - tenant isn't ready - will probably cause errors");
            return this;
        }
        #endregion

        public IUser User { get; } = new DnnUser();

        public string DefaultLanguage => _site.DefaultLanguage;

    }
}