﻿using ToSic.Lib.Helpers;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;
using ExecutionContext = ToSic.Sxc.Sys.ExecutionContext.ExecutionContext;

namespace ToSic.Sxc.Dnn.Code;

[PrivateApi]
internal class DnnExecutionContext<TModel, TServiceKit>(ExecutionContext.MyServices services)
    : ExecutionContext<TModel, TServiceKit>(services, DnnConstants.LogName), IHasDnn
    where TModel : class
    where TServiceKit : ServiceKit
{
    /// <summary>
    /// Dnn context with module, page, portal etc.
    /// </summary>
    public IDnnContext Dnn => _dnn.Get(GetService<IDnnContext>);
    private readonly GetOnce<IDnnContext> _dnn = new();
}