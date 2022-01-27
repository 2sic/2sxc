using ToSic.Eav.Documentation;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Beta.LightSpeed;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Beta.Lightspeed
{
    [PrivateApi]
    public class OutputCacheConfig: INeedsCodeRoot
    {
        internal const string DynCodePiggyBackId = "OutputCacheConfig";

        internal OutputCacheConfigState State => _state ?? (_state = new OutputCacheConfigState());
        private OutputCacheConfigState _state;
        
        public bool Enable { get => State.Enabled; set => State.Enabled = value; }

        public void DependOn(IApp app)
        {
            if(app == null) return;
            State.Apps.Add(app.AppId);
        }

        #region Connect to DynamicCodeRoot

        public void AddBlockContext(IDynamicCodeRoot codeRoot)
        {
            _codeRoot = (DynamicCodeRoot)codeRoot;

            // Check if there is already a state object, otherwise create one
            _state = _codeRoot.PiggyBack.GetOrGenerate(DynCodePiggyBackId, () => State);
        }
        private DynamicCodeRoot _codeRoot;

        #endregion
    }
}
