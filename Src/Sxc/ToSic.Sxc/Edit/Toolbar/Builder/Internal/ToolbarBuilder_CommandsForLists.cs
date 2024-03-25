using System.Runtime.CompilerServices;
using static ToSic.Sxc.Edit.Toolbar.EntityEditInfo;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar.Internal;

partial class ToolbarBuilder
{
    private IToolbarBuilder AddListAction(
        string commandName,
        object target,
        NoParamOrder noParamOrder,
        string contentType,
        object ui,
        object parameters,
        string operation,
        Func<ITweakButton, ITweakButton> tweak,
        [CallerMemberName] string methodName = default)
    {
        TargetCheck(target);
        var pars = PreCleanParams(tweak, defOp: OprAuto, operation: operation, ui: ui, parameters: parameters, methodName: methodName);
        var command = new ToolbarRuleForEntity(commandName, target, 
            contentType: contentType,
            ui: pars.Ui,
            parameters: pars.Parameters,
            propsKeep: KeysOfLists, 
            operation: pars.Operation);
        return this.AddInternal(command);

    }

    public IToolbarBuilder Add(
        object target = null,
        NoParamOrder noParamOrder = default,
        string contentType = null,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    ) => AddListAction("add", target, noParamOrder, contentType, ui, parameters, operation, tweak);

    public IToolbarBuilder AddExisting(
        object target = null,
        NoParamOrder noParamOrder = default,
        string contentType = null,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    ) => AddListAction("add-existing", target, noParamOrder, contentType, ui, parameters, operation, tweak);

    public IToolbarBuilder List(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    ) => AddListAction("list", target, noParamOrder, null, ui, parameters, operation, tweak);


    public IToolbarBuilder MoveDown(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    ) => AddListAction("movedown", target, noParamOrder, null, ui, parameters, operation, tweak);

    public IToolbarBuilder MoveUp(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    ) => AddListAction("moveup", target, noParamOrder, null, ui, parameters, operation, tweak);

    public IToolbarBuilder Remove(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    ) => AddListAction("remove", target, noParamOrder, null, ui, parameters, operation, tweak);

    public IToolbarBuilder Replace(
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null
    ) => AddListAction("replace", target, noParamOrder, null, ui, parameters, operation, tweak);
}