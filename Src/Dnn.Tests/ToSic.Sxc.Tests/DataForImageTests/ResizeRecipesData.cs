using System.Collections.Generic;
using Newtonsoft.Json;
using ToSic.Sxc.Images;

namespace ToSic.Sxc.Tests.DataForImageTests
{
    internal class ResizeRecipesData
    {
        public const int W100 = 990;
        public const int W75 = 700;
        public const int W75ImgOnly777 = 777;
        public const int W50 = 450;
        public const int W25 = 200;

        public static RecipeSet TestRecipeSet() =>
            new RecipeSet(new Recipe(recipes:  new[]
            {
                new Recipe(factor: "1", width: W100),
                new Recipe(factor: "3/4", width: W75, recipes: new[]
                {
                    new Recipe(tag: "img", width: W75ImgOnly777, attributes: new Dictionary<string, object>
                    {
                        { "class", "img-fluid" },
                        { "test", "value" }
                    })
                }),
                new Recipe(factor: "1:2", width: W50),
                new Recipe(factor: "0.25", width: W25)
            }));

        public static string JsonRecipe()
        {
            var adv = new
            {
                recipe = new
                {
                    recipes = new object[]
                    {
                        new { factor = "1", width = W100 },
                        new
                        {
                            factor = "3/4", width = W75, recipes = new object[] { new { tag = "img", width = W75ImgOnly777 } }
                        },
                        new { factor = "1:2", width = W50 },
                        new { factor = "0.25", width = W25 }
                    }

                }
            };

            return JsonConvert.SerializeObject(adv, Formatting.Indented);
        }

    }
}
