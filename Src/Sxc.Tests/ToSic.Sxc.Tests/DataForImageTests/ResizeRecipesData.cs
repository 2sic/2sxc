using System.Collections.Generic;
using System.Text.Json;
using ToSic.Eav.Serialization;
using ToSic.Sxc.Images;

namespace ToSic.Sxc.Tests.DataForImageTests
{
    internal class ResizeRecipesData
    {
        public const int W100 = 990;
        public const int W75 = 700;
        public const int W75ImgOnly777 = 777;
        public const int W75CssUnknown = 678;
        public const int W50 = 450;
        public const int W25 = 200;

        public const string CssNone = null;
        public const string CssUnknown = "unk";
        private static Dictionary<string, object> Attributes75MixIn = new()
        {
            { "loading", "lazy" },
            { "toReset", "parent" }
        };
        private static Dictionary<string, object> Attributes75 = new()
        {
            { "class", "img-fluid" },
            { "test", "value" },
            { "toReset", null } // null will reset the originally set attribute by the parent
        };

        public static AdvancedSettings TestRecipeSet() =>
            new(new(recipes: new[]
            {
                new Recipe(forFactor: "1", width: W100),
                new Recipe(forFactor: "3/4", width: W75, attributes: Attributes75MixIn, recipes: new[]
                {
                    new Recipe(forTag: "img", width: W75CssUnknown, forCss: CssUnknown, attributes: Attributes75),
                    new Recipe(forTag: "img", width: W75ImgOnly777, attributes: Attributes75)
                }),
                new Recipe(forFactor: "1:2", width: W50),
                new Recipe(forFactor: "0.25", width: W25)
            }));

        public static AdvancedSettings TestRecipeSetFromJson => AdvancedSettings.FromJson(JsonRecipe());

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private static object JsonAttributes75MixIn = new { loading = "lazy", toReset = "parent" };
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private static object JsonAttributes75 = new { @class = "img-fluid", test = "value", toReset = (string)null };

        private static string JsonRecipe()
        {
            var adv = new
            {
                recipe = new
                {
                    recipes = new object[]
                    {
                        new { forFactor = "1", width = W100 },
                        new
                        {
                            forFactor = "3/4", width = W75, attributes = JsonAttributes75MixIn, recipes = new object[]
                            {
                                new { forTag = "img", width = W75CssUnknown, forCss = CssUnknown, attributes = JsonAttributes75 },
                                new { forTag = "img", width = W75ImgOnly777, attributes = JsonAttributes75 }
                            }
                        },
                        new { forFactor = "1:2", width = W50 },
                        new { forFactor = "0.25", width = W25 }
                    }

                }
            };

            return JsonSerializer.Serialize(adv, JsonOptions.UnsafeJsonWithoutEncodingHtml);
        }
    }
}
