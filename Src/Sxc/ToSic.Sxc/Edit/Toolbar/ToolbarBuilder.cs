using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.DI;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Sxc.Code;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.Toolbar
{
    /// <inheritdoc />
    public partial class ToolbarBuilder: HybridHtmlString, IEnumerable<string>, IToolbarBuilder, INeedsDynamicCodeRoot
    {

        #region Constructors and Init

        public class Dependencies
        {

            public Dependencies(Lazy<IAppStates> appStatesLazy, LazyInitLog<ToolbarButtonDecoratorHelper> toolbarButtonHelper)
            {
                ToolbarButtonHelper = toolbarButtonHelper;
                AppStatesLazy = appStatesLazy;
            }
            internal readonly Lazy<IAppStates> AppStatesLazy;
            public LazyInitLog<ToolbarButtonDecoratorHelper> ToolbarButtonHelper { get; }

            internal Dependencies InitLogIfNotYet(ILog parentLog)
            {
                if (_alreadyInited) return this;
                _alreadyInited = true;
                ToolbarButtonHelper.SetLog(parentLog);
                return this;
            }

            private bool _alreadyInited;
        }

        public ToolbarBuilder(Dependencies deps) => _deps = deps.InitLogIfNotYet(Log);
        private readonly Dependencies _deps;

        /// <summary>
        /// Clone-constructor
        /// </summary>
        private ToolbarBuilder(ToolbarBuilder parent): this(parent._deps)
        {
            this.Init(parent.Log);
            _currentAppIdentity = parent._currentAppIdentity;
            _codeRoot = parent._codeRoot;
            _params = parent._params;
            Rules.AddRange(parent.Rules);
        }

        public ILog Log { get; } = new Log(Constants.SxcLogName + ".TlbBld");

        private IAppIdentity _currentAppIdentity;

        public void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            if (codeRoot == null) return;
            _codeRoot = codeRoot;
            _currentAppIdentity = codeRoot.App;
        }
        private IDynamicCodeRoot _codeRoot;

        #endregion

        private ToolbarBuilderParams _params;

        public IToolbarBuilder With(
            string noParamOrder = Eav.Parameters.Protector,
            string mode = null,
            object target = null
        )
        {
            // Create clone before starting to log so it's in there too
            var clone = new ToolbarBuilder(this);
            var p = clone._params = new ToolbarBuilderParams(_params);
            if (mode != null) p.Mode = mode;
            if (target != null) p.Target = target;
            return clone;
        }

        public List<ToolbarRuleBase> Rules { get; } = new List<ToolbarRuleBase>();




        public IToolbarBuilder Settings(
            string noParamOrder = Eav.Parameters.Protector, 
            string show = null,
            string hover = null, 
            string follow = null, 
            string classes = null, 
            string autoAddMore = null, 
            string ui = "",
            string parameters = "")
            => Add(new ToolbarRuleSettings(show: show, hover: hover, follow: follow, classes: classes, autoAddMore: autoAddMore,
                ui: ui, parameters: parameters));



        #region Enumerators

        [PrivateApi]
        public IEnumerator<string> GetEnumerator() => Rules.Select(r => r.ToString()).GetEnumerator();
        [PrivateApi]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

    }
}
