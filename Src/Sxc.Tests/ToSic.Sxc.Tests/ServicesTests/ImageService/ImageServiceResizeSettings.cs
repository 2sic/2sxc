using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services;
using System.Linq;
using ToSic.Sxc.Web.Url;

namespace ToSic.Sxc.Tests.ServicesTests
{
    [TestClass]
    public class ImageServiceResizeSettings: TestBaseSxc
    {
        [TestMethod]
        public void EmptyOnlyWidth()
        {
            var settings = Build<IImageService>().Settings(width: 100);
            Assert.AreEqual(100, settings.Width);
            AssertAllEmptyExceptSpecified(settings, nameof(settings.Width));
        }

        [TestMethod]
        public void EmptyOnlyHeight()
        {
            var settings = Build<IImageService>().Settings(height: 100);
            Assert.AreEqual(100, settings.Height);
            AssertAllEmptyExceptSpecified(settings, nameof(settings.Height));
        }

        [TestMethod]
        public void EmptyOnlyFormat()
        {
            var settings = Build<IImageService>().Settings(format: "jpg");
            Assert.AreEqual("jpg", settings.Format);
            AssertAllEmptyExceptSpecified(settings, nameof(settings.Format));
        }

        [TestMethod]
        public void EmptyOnlyResizeMode()
        {
            var settings = Build<IImageService>().Settings(resizeMode: ImageConstants.ModeCrop);
            Assert.AreEqual(ImageConstants.ModeCrop, settings.ResizeMode);
            AssertAllEmptyExceptSpecified(settings, nameof(settings.ResizeMode));
        }

        [TestMethod]
        public void EmptyOnlyScaleMode()
        {
            // todo: use constants for the final result
            var settings = Build<IImageService>().Settings(scaleMode: "up");
            Assert.AreEqual("upscaleonly", settings.ScaleMode);
            AssertAllEmptyExceptSpecified(settings, nameof(settings.ScaleMode));
        }

        [TestMethod]
        public void EmptyOnlyParameters()
        {
            var settings = Build<IImageService>().Settings(parameters: "count=17");
            Assert.AreEqual("count=17", settings.Parameters.NvcToString());
            AssertAllEmptyExceptSpecified(settings, nameof(settings.Parameters));
        }

        [TestMethod]
        public void EmptyOnlyQuality75()
        {
            var settings = Build<IImageService>().Settings(quality: 75);
            Assert.AreEqual(75, settings.Quality);
            AssertAllEmptyExceptSpecified(settings, nameof(settings.Quality));
        }

        [TestMethod]
        public void EmptyOnlyQualityDot75()
        {
            var settings = Build<IImageService>().Settings(quality: .75f);
            Assert.AreEqual(75, settings.Quality);
            AssertAllEmptyExceptSpecified(settings, nameof(settings.Quality));
        }

        // 2022-03-17 not valid any more, this property will be removed
        //[TestMethod]
        //public void EmptyOnlySrcSet()
        //{
        //    var settings = Build<IImageService>().ResizeSettings(rules: new MultiResizeRule {SrcSet = "100,200,300" });
        //    Assert.AreEqual("100,200,300", settings.SrcSet);
        //    AssertAllEmptyExceptSpecified(settings, nameof(settings.SrcSet));
        //}



        [TestMethod]
        public void EmptyWidthAndHeight()
        {
            var settings = Build<IImageService>().Settings(width: 100, height: 49);
            Assert.AreEqual(100, settings.Width);
            Assert.AreEqual(49, settings.Height);
            AssertAllEmptyExceptSpecified(settings, new[] { nameof(settings.Width), nameof(settings.Height) });
        }



        private void AssertAllEmptyExceptSpecified(IResizeSettings settings, string nameToSkip)
            => AssertAllEmptyExceptSpecified(settings, new[] { nameToSkip });

        private void AssertAllEmptyExceptSpecified(IResizeSettings settings, string[] namesToSkip)
        {
            var count = 0;
            count += MaybeTestOneProperty(settings.Height, 0, namesToSkip, nameof(settings.Height));
            count += MaybeTestOneProperty(settings.Width, 0, namesToSkip, nameof(settings.Width));
            count += MaybeTestOneProperty(settings.Format, null, namesToSkip, nameof(settings.Format));
            count += MaybeTestOneProperty(settings.ResizeMode, null, namesToSkip, nameof(settings.ResizeMode));
            count += MaybeTestOneProperty(settings.Parameters, null, namesToSkip, nameof(settings.Parameters));
            count += MaybeTestOneProperty(settings.Quality, 0, namesToSkip, nameof(settings.Quality));
            count += MaybeTestOneProperty(settings.ScaleMode, null, namesToSkip, nameof(settings.ScaleMode));
            // 2022-03-17 2dm removing this property
            //count += MaybeTestOneProperty(settings.SrcSet, null, namesToSkip, nameof(settings.SrcSet));

            // Verify the total count matches expected
            const int expectedCount = 7;
            Assert.AreEqual(expectedCount - namesToSkip.Length, count, "count should be total fields minus untested");
        }

        private static int MaybeTestOneProperty<T>(T actual, T expected, string[] namesToSkip, string notToTest)
        {
            if (namesToSkip.Any(n => n == notToTest)) return 0;
            Assert.AreEqual(expected, actual, notToTest);
            return 1;

        }
    }
}
