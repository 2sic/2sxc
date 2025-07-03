using ToSic.Sxc.Code.Sys.CodeRunHelpers;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Sys;

[PrivateApi]
// #NoEditorBrowsableBecauseOfInheritance
//[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class CustomCodeBase : ServiceWithContext, ICompatibilityLevel
{
    #region Constructor / Setup

    /// <summary>
    /// Main constructor, NOT for DI may never have parameters, otherwise inheriting code will run into problems. 
    /// </summary>
    protected CustomCodeBase(string logName) : base(logName) { }

    /// <summary>
    /// Special helper to move all Razor logic into a separate class.
    /// For architecture of Composition over Inheritance.
    /// </summary>
    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    [field: AllowNull, MaybeNull]
    protected internal CodeHelper CodeHlp => field
        ??= ExCtx.GetService<CodeHelper>().Init(this as IGetCodePath ?? throw new($"Can't cast to {nameof(IGetCodePath)}, but inheriting classes must implement it."));


    //[PrivateApi]
    //[ShowApiWhenReleased(ShowApiMode.Never)]
    //public override void ConnectToRoot(IExecutionContext exCtx)
    //{
    //    base.ConnectToRoot(exCtx);
    //}

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public abstract int CompatibilityLevel { get; }

    #endregion
}