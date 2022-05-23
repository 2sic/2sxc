using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ToSic.Eav.Apps;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.Toolbar
{
    /// <inheritdoc />
    public partial class ToolbarBuilder: HybridHtmlString, IEnumerable<string>, IToolbarBuilder, IHasLog
    {

        #region Constructors

        public class Dependencies
        {
            public Dependencies(Lazy<AppStates> appStatesLazy)
            {
                AppStatesLazy = appStatesLazy;
            }
            internal readonly Lazy<AppStates> AppStatesLazy;
        }

        public ToolbarBuilder(Dependencies deps) => _deps = deps;
        private readonly Dependencies _deps;
        public ILog Log { get; } = new Log(Constants.SxcLogName + ".TlbBld");

        public IToolbarBuilder Init(IAppIdentity currentApp)
        {
            _currentAppIdentity = currentApp;
            return this;
        }

        private IAppIdentity _currentAppIdentity;

        #endregion
        public List<ToolbarRuleBase> Rules { get; } = new List<ToolbarRuleBase>();

        /// <inheritdoc />
        [PrivateApi]
        public IToolbarBuilder Add(params string[] rules) => InnerAdd(rules?.Cast<object>());

        /// <inheritdoc />
        public IToolbarBuilder Add(params object[] rules) => InnerAdd(rules);

        private IToolbarBuilder InnerAdd(params object[] newRules)
        {
            // Create clone before starting to log so it's in there too
            var clone = new ToolbarBuilder(_deps).Init(Log);
            clone.Init(_currentAppIdentity);
            
            var callLog = Log.Fn<IToolbarBuilder>();
            clone.Rules.AddRange(Rules);
            if (!newRules.Any()) return callLog.Return(clone, "no new rules");
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
            string contentTypes,
            string noParamOrder = Eav.Parameters.Protector,
            string ui = null,
            string parameters = null,
            string context = null
        ) => Add(new ToolbarRuleMetadata(target, contentTypes, ui, parameters, context: GetContext(target, context)));

        [PrivateApi("WIP 13.11")]
        public IToolbarBuilder Image(
            object target,
            string noParamOrder = Eav.Parameters.Protector,
            string ui = null,
            string parameters = null
        ) => Add(new ToolbarRuleImage(target, ui, parameters));

        public IToolbarBuilder Settings(string noParamOrder = Eav.Parameters.Protector, string show = null,
            string hover = null, string follow = null, string classes = null, string autoAddMore = null, string ui = "", string parameters = "")
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
