using System.Collections.Generic;
using ToSic.Sxc.Images;

namespace ToSic.Sxc.Tests.ServicesTests
{
    public class ImageTagsTestPermutations
    {
        public static List<TestParamSet> GenerateTestParams(string name, string srcset)
        {
            var i = 1;
            return new List<TestParamSet>
            {
                new TestParamSet($"Test #{i++} {name}-both", true, true, true, srcset),
                new TestParamSet($"Test #{i++} {name}-both", true, true, false, srcset),
                new TestParamSet($"Test #{i++} {name}-on set", true, false, true, srcset),
                new TestParamSet($"Test #{i++} {name}-on set", true, false, false, srcset),
                // The On-Pic variations don't exist, as the pic doesn't have params for width/height
                //new TestParamSet($"{i++}{name}-on pic", false, true, true, srcset),
                //new TestParamSet($"{i++}{name}-on pic", false, true, false, srcset),
            };
        }

        public class TestParams
        {
            public TestParams(bool width = false, bool height = false, string srcset = null)
            {
                if (width) Width = 120;
                if (height) Height = 24;
                Srcset = srcset;
            }
            public int? Width;
            public int? Height;
            public string Srcset;

            public Recipe SrcSetRule => Srcset == null ? null : new Recipe(srcset: Srcset); // { SrcSet = Srcset };
        }

        public class TestParamSet
        {
            public TestParamSet(string name, bool useSet, bool usePic, bool putSrcOnSet, string srcset)
            {
                Set = new TestParams(useSet, useSet, putSrcOnSet ? srcset : null);
                Pic = new TestParams(usePic, usePic, putSrcOnSet ? null : srcset);
                Name = name + $" (Settings on Pic: {usePic}, On Set: {useSet}, srcset: '{srcset}' - on "
                            + (putSrcOnSet ? "set" : "pic");
            }
            public TestParams Set;
            public TestParams Pic;
            public string Name;
        }

    }
}
