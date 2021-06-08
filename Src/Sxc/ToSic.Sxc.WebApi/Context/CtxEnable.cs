using System;

namespace ToSic.Sxc.WebApi.Context
{
    [Flags] public enum CtxEnable
    {
        // ReSharper disable once UnusedMember.Global
        None = 0,
        AppPermissions = 1,
        CodeEditor = 2,
        Query = 4,
        FormulaSave = 8,
        All = AppPermissions | CodeEditor | Query| FormulaSave,
    }
}
