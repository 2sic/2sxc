using ToSic.Eav.Data.Sys;

namespace ToSic.Sxc.Code.Generate.Data;

internal static class ContentTypeModelGenExtensions
{
    #region Helper Methods for Derived Classes

    extension(IContentType type)
    {
        public string? GenerateCommonScopeRemarks()
        {
            var scope = type.Scope;
            var scopeIsSpecial = scope != ScopeConstants.Default;
            return scopeIsSpecial ? $"This Content-Type is NOT in the default scope, so you may not see it in the Admin UI. It's in the scope {scope}." : null;
        }

        public string GetScopeDescription()
        {
            var scope = type.Scope;
            var scopeIsSpecial = scope != ScopeConstants.Default;
            return scopeIsSpecial ? $"(scope: {scope})" : "";
        }
    }

    #endregion

}
