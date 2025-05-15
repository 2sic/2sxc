﻿using ToSic.Sxc.Code.Internal.CodeRunHelpers;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Internal;

[PrivateApi]
// #NoEditorBrowsableBecauseOfInheritance
//[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class CustomCodeBase : ServiceForDynamicCode, ICompatibilityLevel
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
    protected internal CodeHelper CodeHlp => field
        ??= ExCtx.GetService<CodeHelper>().Init(this as IGetCodePath); // inheriting classes must implement IGetCodePath


    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public override void ConnectToRoot(IExecutionContext exCtx)
    {
        base.ConnectToRoot(exCtx);
        //GetCodeHlp.ConnectToRoot(codeRoot);
    }

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public abstract int CompatibilityLevel { get; }

    #endregion
}