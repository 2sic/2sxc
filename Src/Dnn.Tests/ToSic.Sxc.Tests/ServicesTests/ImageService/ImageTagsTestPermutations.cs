using System.Collections.Generic;

namespace ToSic.Sxc.Tests.ServicesTests
{
    public class ImageTagsTestPermutations
    {
        public static List<TestParamSet> GenerateTestParams(string name, string srcSet)
        {
            var i = 1;
            return new List<TestParamSet>
            {
                new TestParamSet($"{i++}{name}-both", true, true, true, srcSet),
                new TestParamSet($"{i++}{name}-both", true, true, false, srcSet),
                new TestParamSet($"{i++}{name}-on set", true, false, true, srcSet),
                new TestParamSet($"{i++}{name}-on set", true, false, false, srcSet),
                // The On-Pic variations don't exist, as the pic doesn't have params for width/height
                //new TestParamSet($"{i++}{name}-on pic", false, true, true, srcSet),
                //new TestParamSet($"{i++}{name}-on pic", false, true, false, srcSet),
            };
        }

        public class TestParams
        {
            public TestParams(bool width = false, bool height = false, string srcSet = null)
            {
                if (width) Width = 120;
                if (height) Height = 24;
                SrcSet = srcSet;
            }
            public int? Width;
            public int? Height;
            public string SrcSet;
        }

        public class TestParamSet
        {
            public TestParamSet(string name, bool useSet, bool usePic, bool srcSetOnSet, string srcSet)
            {
                Set = new TestParams(useSet, useSet, srcSetOnSet ? srcSet : null);
                Pic = new TestParams(usePic, usePic, srcSetOnSet ? null : srcSet);
                Name = name + $" (Settings on Pic: {usePic}, On Set: {useSet}, srcSet: '{srcSet}' - on "
                            + (srcSetOnSet ? "set" : "pic");
            }
            public TestParams Set;
            public TestParams Pic;
            public string Name;
        }

    }
}
