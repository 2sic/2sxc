using System;
using System.Globalization;
using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToSic.Sxc.Tests.LinksAndImages.LinkImageTests
{
    [TestClass]
    public class ImgSrcSet : LinkImageTestBase
    {
        [DataRow(null)]
        [DataRow("")]
        [DataTestMethod]
        public void EmptySrcSet(string srcSet)
        {
            var urls = new[]
            {
                "test.jpg",
                "test.png",
                "/test.jpg",
                "//test.jpg",
                "http://www.2sxc.org/test.jpg",
                "weird-extension.abc"
            };

            foreach (var url in urls) TestOnLinkerAndHelper(url, url, srcSet: srcSet);
        }

        [DataRow("test.jpg?w=1000 1000w", "test.jpg", "1000")]
        [DataRow("test.jpg?w=1000 1000w", "test.jpg", "1000w")]
        [DataRow("test.jpg?w=1000 1000w,\ntest.jpg?w=2000 2000w", "test.jpg", "1000,2000")]
        [DataRow("test.jpg?w=500 500w,\ntest.jpg?w=1000 1000w,\ntest.jpg?w=2000 2000w", "test.jpg", "500w,1000w,2000w")]
        [DataTestMethod]
        public void SrcSetUrlOnlyW(string expected, string url, string srcSet) 
            => TestOnLinkerAndHelper(expected, url, srcSet: srcSet);

        [DataRow("test.jpg 1x", "test.jpg", "1")]
        [DataRow("test.jpg 1.5x", "test.jpg", "1.5x")]
        [DataTestMethod]
        public void SrcSetUrlOnlyX(string expected, string url, string srcSet) 
            => TestOnLinkerAndHelper(expected, url, srcSet: srcSet);


        [DataRow("test.jpg?w=1200 1x", "test.jpg", "1")]
        [DataRow("test.jpg?w=1800 1.5x", "test.jpg", "1.5x")]
        [DataRow("test.jpg?w=1200 1x,\ntest.jpg?w=1800 1.5x,\ntest.jpg?w=2400 2x", "test.jpg", "1x,1.5x,2")]
        [DataRow("test.jpg?w=1200 1x,\ntest.jpg?w=1800 1.5x,\ntest.jpg?w=2000 2x", "test.jpg", "1x,1.5x,2=2000")]
        [DataRow("test.jpg?w=1200 1x,\ntest.jpg?w=1800 1.5x,\ntest.jpg?w=2000&h=1000 2x", "test.jpg", "1x,1.5x,2=2000:1000")]
        [DataTestMethod]
        public void SrcSetUrlOnlyXAndWidth(string expected, string url, string srcSet)
            => TestOnLinkerAndHelper(expected, url, width: 1200, srcSet: srcSet);


        //[TestMethod]
        //public void BasicWidthAndHeight()
        //{
        //    EqualOnLinkerAndHelper("test.jpg?w=200", "test.jpg", width: 200);
        //    EqualOnLinkerAndHelper("test.jpg?h=200", "test.jpg", height: 200);
        //    EqualOnLinkerAndHelper("test.jpg?w=200&h=200", "test.jpg", width: 200, height:200);
        //}

        //[TestMethod]
        //public void BasicWidthAndAspectRatio()
        //{
        //    EqualOnLinkerAndHelper("test.jpg?w=200", "test.jpg", width: 200, aspectRatio: 0);
        //    EqualOnLinkerAndHelper("test.jpg?w=200&h=200", "test.jpg", width: 200, aspectRatio: 1);
        //    EqualOnLinkerAndHelper("test.jpg?w=200&h=400", "test.jpg", width: 200, aspectRatio: 0.5);
        //    EqualOnLinkerAndHelper("test.jpg?w=200&h=100", "test.jpg", width: 200, aspectRatio: 2);
        //    EqualOnLinkerAndHelper("test.jpg?w=200&h=80", "test.jpg", width: 200, aspectRatio: 2.5);

        //    // Note: in this case it should be 112.5 and will be rounded down by default
        //    EqualOnLinkerAndHelper("test.jpg?w=200&h=112", "test.jpg", width: 200, aspectRatio: 16f/9);
        //}


    }
}
