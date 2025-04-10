﻿using ToSic.Lib.Helpers;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Internal;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TServiceKit"></typeparam>
/// <param name="services"></param>
/// <param name="logPrefix"></param>
/// <remarks>
/// Note that it used to be abstract, but we changed that so it can be used in unit tests.
///
/// Both Dnn and Oqtane have their own version of this class, but the changes are minimal.
/// </remarks>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeApiService<TModel, TServiceKit>(CodeApiService.MyServices services, string logPrefix)
    : CodeApiService(services, logPrefix), ICodeApiService<TModel, TServiceKit>
    where TModel : class
    where TServiceKit : ServiceKit
{
    // Model not in use ATM, may never be. 
    //public TModel Model => default;

    /// <summary>
    /// The primary kit for this service.
    /// Other kit versions can be accessed using `GetKit{TKit}`
    /// </summary>
    public TServiceKit Kit => _kit.Get(((ICodeApiServiceInternal)this).GetKit<TServiceKit>);
    private readonly GetOnce<TServiceKit> _kit = new();

}