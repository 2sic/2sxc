using System.Collections.Generic;
using System.Linq;
using static ToSic.Sxc.Plumbing.ParseObject;

namespace ToSic.Sxc.Images
{
    public class ResizeSettingsHelper
    {
        public static Recipe Find(ResizeSettings resizeSettings, SrcSetType srcSetType, bool useFactors)
        {
            var multiSettings = resizeSettings?.MultiResize;
            var mainRecipe = multiSettings?.Recipe;
            if (mainRecipe == null) return null;
            var subRecipes = mainRecipe.AllSubRecipes;

            Recipe result = null;
            if (subRecipes?.Any()  == true)
            {
                var factor = resizeSettings.Factor;
                if (DNearZero(factor)) factor = 1;

                var key = srcSetType == SrcSetType.Img ? "img" : "src";

                var factorToUse = useFactors ? (double?)factor : null;
                result = FindRule(subRecipes, factorToUse, key)
                             ?? FindRule(subRecipes, factorToUse, Recipe.RuleForDefault);
                
                // Nothing yet, and previously we tried with factor, then try without
                if (result == null && useFactors)
                    result = FindRule(subRecipes, null, key)
                             ?? FindRule(subRecipes, null, Recipe.RuleForDefault);
            }

            return result ?? mainRecipe;
        }

        private static Recipe FindRule(IReadOnlyCollection<Recipe> subRecipes, double? factor, string target)
        {
            if (subRecipes == null || !subRecipes.Any()) return null;
            IEnumerable<Recipe> query = subRecipes;
            query = query.Where(m => factor == null ? m.FactorParsed == 0 : DNearZero(m.FactorParsed - factor.Value));
            query = query.Where(m => m.Tag == target);
            var fm = query.FirstOrDefault();
            return fm;
        }

        //private static Recipe FindSubRule(ResizeSettings resizeSettings, IReadOnlyCollection<Recipe> subRecipes)
        //{
        //    //var subRecipes = resizeSettings?.MultiResize?.Recipe?.AllSubRecipes;
        //    if (subRecipes == null || !subRecipes.Any()) return null;
        //    var factor = resizeSettings.Factor;
        //    if (DNearZero(factor)) factor = 1;
        //    var fm = subRecipes.FirstOrDefault(m => m.Type == Recipe.RuleForFactor && DNearZero(m.FactorParsed - factor));
        //    return fm;
        //}

        //private static Recipe KeepOrUseSubRule(Recipe rule, string target)
        //{
        //    if (rule == null) return null;
        //    if (rule.AllSubRecipes == null || rule.AllSubRecipes.Count == 0) return rule;
        //    return FindRuleForTarget(rule.AllSubRecipes, target) ?? rule;
        //}

        //internal static Recipe FindRuleForTarget(IEnumerable<Recipe> recipes, string target) 
        //    => recipes?.FirstOrDefault(r => r.Type == target);
    }

    public enum SrcSetType
    {
        Img,
        Source
    }

}
