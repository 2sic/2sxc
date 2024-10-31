using System.Collections.Generic;
using ToSic.Sxc.Images;

namespace ToSic.Sxc.Tests.ServicesTests.ImageServiceTests;

public class ImageTagsTestPermutations
{
    public static List<TestParamSet> GenerateTestParams(string name, string variants)
    {
        var i = 1;
        return new()
        {
            new($"Test #{i++} {name}-both", true, true, true, variants),
            new($"Test #{i++} {name}-both", true, true, false, variants),
            new($"Test #{i++} {name}-on set", true, false, true, variants),
            new($"Test #{i++} {name}-on set", true, false, false, variants),
            // The On-Pic variations don't exist, as the pic doesn't have params for width/height
            //new TestParamSet($"{i++}{name}-on pic", false, true, true, srcset),
            //new TestParamSet($"{i++}{name}-on pic", false, true, false, srcset),
        };
    }

    public class TestParams
    {
        public TestParams(bool width = false, bool height = false, string variants = null)
        {
            if (width) Width = 120;
            if (height) Height = 24;
            Variants = variants;
        }
        public int? Width;
        public int? Height;
        public string Variants;

        public Recipe SrcSetRule => Variants == null ? null : new Recipe(variants: Variants);
    }

    public class TestParamSet
    {
        public TestParamSet(string name, bool useSet, bool usePic, bool putSrcOnSet, string variants)
        {
            Set = new(useSet, useSet, putSrcOnSet ? variants : null);
            Pic = new(usePic, usePic, putSrcOnSet ? null : variants);
            Name = name + $" (Settings on Pic: {usePic}, On Set: {useSet}, srcset: '{variants}' - on "
                        + (putSrcOnSet ? "set" : "pic");
        }
        public TestParams Set;
        public TestParams Pic;
        public string Name;
    }

}