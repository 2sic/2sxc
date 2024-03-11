using ToSic.Eav.Plumbing;
using static ToSic.Sxc.Internal.Plumbing.ParseObject;

namespace ToSic.Sxc.Images.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal static class ResizeSettingsExtensions
{
    internal static Recipe Find(this ResizeSettings resizeSettings, SrcSetType srcSetType, bool useFactors, string cssFramework)
    {
        var advanced = resizeSettings?.Advanced;
        var mainRecipe = advanced?.Recipe;
        if (mainRecipe == null) return null;
        var subRecipes = advanced.AllSubRecipes;

        // No sub-recipes - return main
        if (subRecipes.SafeNone()) return mainRecipe;

        // Prepare list of frameworks, targets and factors to use in the loops
        var frameworks = cssFramework == null ? [(string)null] : new[] { cssFramework, null };

        var primaryTarget = srcSetType == SrcSetType.Img ? "img" : "source";
        var targetsToTest = new[] { primaryTarget, Recipe.RuleForDefault };

        var factor = resizeSettings.FactorToUse; // DNearZero(resizeSettings.Factor) ? 1 : resizeSettings.Factor;
        var factorsToTest = useFactors
            ? [(double?)factor, null]
            : new[] { (double?)null };

        // Get PiggyBack cache to rarely rerun LINQ
        var pgb = advanced.PiggyBack;

        // Loop all combinations
        foreach (var cssFw in frameworks)
        {
            var cssKey = cssFw.AsKey();
            var cssRecipes = pgb.GetOrGenerate(cssKey, 
                () => subRecipes.Where(r => r.ForCss == cssFw).ToList());
            if (!cssRecipes.Any()) continue;
            foreach (var f in factorsToTest)
            {
                var factorKey = cssKey + "-" + (f == null ? ((string)null).AsKey() : f.ToString().AsKey());
                var recList = pgb.GetOrGenerate(factorKey, 
                    () => cssRecipes.Where(m => f == null ? m.FactorParsed == 0 : DNearZero(m.FactorParsed - f.Value)).ToList());
                foreach (var target in targetsToTest)
                {
                    var match = recList.FirstOrDefault(m => m.ForTag == target);
                    if (match != null) return match;
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