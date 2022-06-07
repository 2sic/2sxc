using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ToSic.Eav.Apps;
using ToSic.Eav.DI;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.Toolbar
{
    /// <inheritdoc />
    public partial class ToolbarBuilder: HybridHtmlString, IEnumerable<string>, IToolbarBuilder
    {

        #region Constructors

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
        public ILog Log { get; } = new Log(Constants.SxcLogName + ".TlbBld");

        public IToolbarBuilder Init(IAppIdentity currentApp)
        {
            _currentAppIdentity = currentApp;
            return this;
        }
        private IAppIdentity _currentAppIdentity;

        public ToolbarContext Context()
        {
            // See if any rules have a context
            var rulesWithContext = Rules.Where(r => r.Context != null).ToArray();
            if (!rulesWithContext.Any()) return null;
            return rulesWithContext.FirstOrDefault()?.Context;
        }

        #endregion
        public List<ToolbarRuleBase> Rules { get; } = new List<ToolbarRuleBase>();

        /// <inheritdoc />
        [PrivateApi]
        public IToolbarBuilder Add(params string[] rules) 
            => InnerAdd(rules?.Cast<object>().ToArray());   // Must re-to-array, so that it's not re-wrapped

        /// <inheritdoc />
        public IToolbarBuilder Add(params object[] rules) 
            => InnerAdd(rules?.ToArray());                  // Must re-to-array, so that it's not re-wrapped

        private IToolbarBuilder InnerAdd(params object[] newRules)
        {
            // Create clone before starting to log so it's in there too
            var clone = new ToolbarBuilder(_deps).Init(Log);
            clone.Init(_currentAppIdentity);
            
            var callLog = Log.Fn<IToolbarBuilder>();
            clone.Rules.AddRange(Rules);
            if (newRules == null || !newRules.Any())
                return callLog.Return(clone, "no new rules");

            foreach (var rule in newRules)
            {
                if (rule is ToolbarRuleBase realRule)
                    clone.Rules.Add(realRule);
                else if (rule is string stringRule)
                    clone.Rules.Add(new ToolbarRuleGeneric(stringRule));
            }
            return callLog.Return(clone);
        }

        /// <inheritdoc />
        public IToolbarBuilder Metadata(
            object target,
            string contentTypes = null,
            string noParamOrder = Eav.Parameters.Protector,
            string ui = null,
            string parameters = null,
            string context = null
        )
        {
            var finalTypes = GetMetadataTypeNames(target, contentTypes);
            var realContext = GetContext(target, context);
            var result = this as IToolbarBuilder;
            foreach (var type in finalTypes)
                result = result.Add(new ToolbarRuleMetadata(target, type, ui, parameters, context: realContext, helper: _deps.ToolbarButtonHelper.Ready));

            return result;
        }

        /// <inheritdoc />
        public IToolbarBuilder Copy(
            object target,
            string noParamOrder = Eav.Parameters.Protector,
            string ui = null,
            string parameters = null,
            string context = null
        ) => Add(new ToolbarRuleCopy(target, ui, parameters, GetContext(target, context), _deps.ToolbarButtonHelper.Ready));


        [PrivateApi("WIP 13.11")]
        public IToolbarBuilder Image(
            object target,
            string noParamOrder = Eav.Parameters.Protector,
            string ui = null,
            string parameters = null
        ) => Add(new ToolbarRuleImage(target, ui, parameters, context: GetContext(target, null), helper: _deps.ToolbarButtonHelper.Ready));

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

        public override string ToString()
        {
            var rules = Rules.Select(r => r.ToString()).ToArray();
            return JsonConvert.SerializeObject(rules);
        }

        #region Enumerators

        [PrivateApi]
        public IEnumerator<string> GetEnumerator() => Rules.Select(r => r.ToString()).GetEnumerator();
        [PrivateApi]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

    }
}
