using System;

namespace ToSic.Sxc.WebApi.Context
{
    [Flags] public enum Ctx
    {
        // ReSharper disable once UnusedMember.Global
        None = 0,
        AppBasic = 1,
        AppAdvanced = 2,
        Enable = 4,
        Language = 8,
        Page = 16,
        Site = 32,
        System = 64,
        All = AppBasic | AppAdvanced | Enable | Language | Page | Site | System,
    }

}
