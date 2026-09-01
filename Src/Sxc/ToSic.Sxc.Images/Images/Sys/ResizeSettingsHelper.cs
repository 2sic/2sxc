using ToSic.Sys.Caching.PiggyBack;
using ToSic.Sys.Utils;
using static ToSic.Sxc.Sys.Plumbing.ParseObject;

namespace ToSic.Sxc.Images.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal static class ResizeSettingsExtensions
{
    internal static Recipe? Find(this ResizeSettings.ResizeSettings? resizeSettings, SrcSetType srcSetType, bool useFactors, string? cssFramework)
    {
        if (resizeSettings == null)
            return null;
        var advanced = resizeSettings.Advanced;
        var mainRecipe = advanced?.Recipe;
        if (mainRecipe == null)
            return null;
        var subRecipes = advanced!.AllSubRecipes;

        // No sub-recipes - return main
        if (subRecipes.SafeNone())
            return mainRecipe;

        // Prepare list of frameworks, targets and factors to use in the loops
        string?[] frameworks = cssFramework == null
            ? [null]
            : [cssFramework, null];

        var primaryTarget = srcSetType == SrcSetType.Img ? "img" : "source";
        string[] targetsToTest = [primaryTarget, Recipe.RuleForDefault];

        var factor = resizeSettings.FactorToUse; // DNearZero(resizeSettings.Factor) ? 1 : resizeSettings.Factor;
        double?[] factorsToTest = useFactors
            ? [factor, null]
            : [null];

        // Loop all combinations
        foreach (var cssFw in frameworks)
        {
            var cssKey = cssFw.AsKey();
            var cssRecipes = advanced.PiggyBackGet(cssKey, () => subRecipes
                .Where(r => r.ForCss == cssFw)
                .ToList()
            );
            if (!cssRecipes.Any())
                continue;
            foreach (var f in factorsToTest)
            {
                var factorKey = cssKey + "-" + (f == null
                        ? ((string?)null).AsKey()
                        : f.ToString().AsKey()
                    );
                var recList = advanced.PiggyBackGet(factorKey, () => cssRecipes
                    .Where(m => f == null ? m.FactorParsed == 0 : DNearZero(m.FactorParsed - f.Value))
                    .ToList()
                );
                foreach (var target in targetsToTest)
                {
                    var match = recList.FirstOrDefault(m => m.ForTag == target);
                    if (match != null)
                        return match;
                }
            }
        }

        return mainRecipe;
    }
}

internal enum SrcSetType
{
    Img,
    Source
}