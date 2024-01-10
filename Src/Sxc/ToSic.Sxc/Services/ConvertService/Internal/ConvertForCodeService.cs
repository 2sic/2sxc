﻿using System;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Coding;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;

// ReSharper disable MethodOverloadWithOptionalParameter

namespace ToSic.Sxc.Services.Internal;

[PrivateApi("Hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ConvertForCodeService: ServiceBase
{
    private readonly ConvertValueService _cnvSvc;

    public ConvertForCodeService(ConvertValueService cnvSvc): base("Sxc.CnvSrv")
    {
        ConnectServices(
            _cnvSvc = cnvSvc
        );
    }
        
    public string ForCode(object value, NoParamOrder noParamOrder = default, string fallback = default)
    {
        //Protect(noParamOrder, nameof(fallback));
        if (value == null) return fallback;

        // Pre-check special case of date-time which needs ISO encoding without time zone
        if (value.GetType().UnboxIfNullable() == typeof(DateTime))
        {
            var dt = ((DateTime)value).ToString("O").Substring(0, 23) + "z";
            return dt;
        }

        var result = _cnvSvc.To(value, fallback: fallback);
        if (result is null) return null;

        // If the original value was a boolean, we will do case changing as js expects "true" or "false" and not "True" or "False"
        if (value.GetType().UnboxIfNullable() == typeof(bool)) result = result.ToLowerInvariant();

        return result;
    }

}