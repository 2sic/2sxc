﻿using ToSic.Eav.Context;

namespace ToSic.Sxc.Context
{
    // ReSharper disable once InconsistentNaming
    public static class ICmsContextExtensions
    {
        public static string[] SafeLanguagePriorityCodes(this ICmsContext context)
        {
            var site = (context as CmsContext)?.Context.Site;
            return site.SafeLanguagePriorityCodes();
        }
    }
}
