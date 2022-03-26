using System.Linq;
using static ToSic.Sxc.Plumbing.ParseObject;

namespace ToSic.Sxc.Images
{
    public static class ResizeSettingsExtensions
    {
        public static Recipe Find(this ResizeSettings resizeSettings, SrcSetType srcSetType, bool useFactors, string cssFramework)
        {
            var multiSettings = resizeSettings?.Advanced;
            var mainRecipe = multiSettings?.Recipe;
            if (mainRecipe == null) return null;
            var subRecipes = mainRecipe.AllSubRecipes;

            // No sub-recipes - return main
            if (subRecipes?.Any() != true) return mainRecipe;

            // Prepare list of frameworks, targets and factors to use in the loops
            var frameworks = new[] { cssFramework, null };

            var primaryTarget = srcSetType == SrcSetType.Img ? "img" : "source";
            var targetsToTest = new[] { primaryTarget, Recipe.RuleForDefault };

            var factor = DNearZero(resizeSettings.Factor) ? 1 : resizeSettings.Factor;
            var factorsToTest = useFactors
                ? new[] { (double?)factor, null }
                : new[] { (double?)null };

            // Loop all combinations
            foreach (var cssFw in frameworks)
            {
                var cssRecipes = subRecipes.Where(r => r.CssFramework == cssFw).ToList();
                if (!cssRecipes.Any()) continue;
                foreach (var f in factorsToTest)
                {
                    var recList = cssRecipes.Where(m => f == null ? m.FactorParsed == 0 : DNearZero(m.FactorParsed - f.Value)).ToList();
                    foreach (var target in targetsToTest)
                    {
                        var match = recList.FirstOrDefault(m => m.Tag == target);
                        if (match != null) return match;
                    }
                }
            }


            return mainRecipe;
        }
    }

    public enum SrcSetType
    {
        Img,
        Source
    }

}
