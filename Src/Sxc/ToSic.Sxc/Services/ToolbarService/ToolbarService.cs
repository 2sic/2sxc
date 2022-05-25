using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Services
{
    [PrivateApi("Hide implementation")]
    public class ToolbarService: HasLog, IToolbarService, INeedsDynamicCodeRoot
    {
        #region Constructor & Init
        public ToolbarService(GeneratorLog<IToolbarBuilder> toolbarGenerator): base($"{Constants.SxcLogName}.TlbSvc")
        {
            _toolbarGenerator = toolbarGenerator.SetLog(Log);
        }
        private readonly GeneratorLog<IToolbarBuilder> _toolbarGenerator;

        public void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            _codeRoot = codeRoot;
            this.Init(codeRoot.Log);
        }
        private IDynamicCodeRoot _codeRoot;


        #endregion

        public IToolbarBuilder Default(
            string noParamOrder = Eav.Parameters.Protector,
            string ui = null
        ) => NewBuilder(noParamOrder, ToolbarRuleToolbar.Default, ui, null);

        public IToolbarBuilder Empty(
            string noParamOrder = Eav.Parameters.Protector,
            string ui = null
        ) => NewBuilder(noParamOrder, ToolbarRuleToolbar.Empty, ui, null);

        public IToolbarBuilder Metadata(
            object target,
            string contentTypes,
            string noParamOrder = Eav.Parameters.Protector,
            string ui = null,
            string parameters = null,
            string context = null
        ) => Empty().Metadata(target, contentTypes, ui: ui, parameters: parameters, context: context);

        private IToolbarBuilder NewBuilder(string noParamOrder, string toolbarTemplate, string ui, string context)
        {
            var callLog = Log.Fn<IToolbarBuilder>($"{nameof(toolbarTemplate)}:{toolbarTemplate}");
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, "Toolbar", $"{nameof(ui)}");
            var tlb = _toolbarGenerator.New.Init(_codeRoot?.App) // new ToolbarBuilder()
                .Add(new ToolbarRuleToolbar(toolbarTemplate, ui: ui));
            if (context.HasValue())
                tlb = tlb.Add(new ToolbarRuleGeneric($"context?{context}"));
            return callLog.Return(tlb);
        }

    }
}
