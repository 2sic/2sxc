using ToSic.Eav;
using ToSic.Eav.DI;
using ToSic.Eav.Documentation;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Services
{
    [PrivateApi("Hide implementation")]
    public class ToolbarService: ServiceForDynamicCode, IToolbarService, INeedsDynamicCodeRoot
    {
        #region Constructor & Init

        public ToolbarService(GeneratorLog<IToolbarBuilder> toolbarGenerator) : base($"{Constants.SxcLogName}.TlbSvc")
            => ConnectServices(_toolbarGenerator = toolbarGenerator);
        private readonly GeneratorLog<IToolbarBuilder> _toolbarGenerator;

        #endregion

        /// <inheritdoc />
        public IToolbarBuilder Default(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            object prefill = null
        ) => ToolbarBuilder(noParamOrder, ToolbarRuleToolbar.Default, ui, parameters, prefill, null, target: target);


        /// <inheritdoc />
        public IToolbarBuilder Empty(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            object prefill = null
        ) => ToolbarBuilder(noParamOrder, ToolbarRuleToolbar.Empty, ui, parameters, prefill, null, target: target);


        /// <inheritdoc />
        public IToolbarBuilder Metadata(object target,
            string contentTypes = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            object prefill = null,
            string context = null
        ) => Empty().Metadata(target, contentTypes, noParamOrder, ui, parameters, prefill, context: context);


        private IToolbarBuilder ToolbarBuilder(string noParamOrder, string toolbarTemplate, object ui, object parameters, object prefill, string context, object target = null)
        {
            var callLog = Log.Fn<IToolbarBuilder>($"{nameof(toolbarTemplate)}:{toolbarTemplate}");
            Parameters.ProtectAgainstMissingParameterNames(noParamOrder, "Toolbar", $"{nameof(ui)}");
            // The following lines must be just as this, because it's a functional object, where each call may return a new copy
            var tlb = _toolbarGenerator.New();
            tlb.ConnectToRoot(_DynCodeRoot);
            tlb = tlb.Toolbar(toolbarTemplate, target, ui, parameters, prefill);

            if (_defaultUi.HasValue())
                tlb = tlb.Settings(ui: _defaultUi);

            if (context.HasValue()) tlb = tlb.AddInternal(new ToolbarRuleGeneric($"context?{context}"));
            return callLog.Return(tlb);
        }


        internal void _setDemoDefaults(string defaultUi)
        {
            _defaultUi = defaultUi;
        }

        private string _defaultUi;
    }
}
