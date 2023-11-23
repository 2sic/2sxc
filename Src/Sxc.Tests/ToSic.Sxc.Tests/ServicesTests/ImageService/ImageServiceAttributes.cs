using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Tests.ServicesTests.ImageService;
using ToSic.Testing.Shared;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.ServicesTests
{
    [TestClass]
    public class ImageServiceAttributes: TestBaseSxcDb
    {
        private class TestCase
        {
            public string Name, Expected, OnCall, OnRecipe;
            public bool UseRecipe;
            public TestCase(string name, string expected, string onCall, string onRecipe = default, bool useRecipe = false)
            {
                Name = name;
                Expected = expected;
                OnCall = onCall;
                OnRecipe = onRecipe;
                UseRecipe = useRecipe || onRecipe != null;
            }

            public override string ToString() => $"Test: '{Name}";
        }

        private static List<TestCase> TestCasesClass = new()
        {
            new TestCase("Call Only", "class='img-class'", "img-class"),
            new TestCase("Call Only multiple", "class='img-class img-class2'", "img-class img-class2"),
            new TestCase("Call only, recipe null", "class='img-class'", "img-class", useRecipe: true),
            new TestCase("Call only, recipe empty", "class='img-class'", "img-class", useRecipe: true, onRecipe: ""),
            new TestCase("recipe only", "class='rec-class'", null, onRecipe: "rec-class"),
            new TestCase("Call and recipe", "class='img-class rec-class'", "img-class", useRecipe: true,
                onRecipe: "rec-class"),
        };

        private static IEnumerable<object[]> TestDataImgClass { get; } = TestCasesClass.ToTestEnum();

        [TestMethod]
        [DynamicData(nameof(TestDataImgClass))]
        public void TestClassImg(object testObj)
        {
            var test = (TestCase)testObj;
            var recDic = new Dictionary<string, object> { { "class", test.OnRecipe } };
            var recipe = test.UseRecipe ? this.ImgSvc().Recipe(null, attributes: recDic) : null;
            var pic = this.ImgSvc().Picture("dummy.jpg", imgClass: test.OnCall, recipe: recipe);
            IsTrue(pic.Img.ToString().Contains(test.Expected), $"{test}: {pic.Img}");
        }

        private static IEnumerable<object[]> TestDataPicClass { get; } = TestCasesClass.Where(t => !t.UseRecipe).ToTestEnum();
        [TestMethod]
        [DynamicData(nameof(TestDataPicClass))]
        public void TestClassPic(object testObj)
        {
            var test = (TestCase)testObj;
            var pic = this.ImgSvc().Picture("dummy.jpg", pictureClass: test.OnCall);
            IsTrue(pic.ToString().Contains(test.Expected), $"{test}: {pic.Picture}");
            IsFalse(pic.Img.ToString().Contains(test.Expected), $"{test}: {pic.Img}");
        }


        private static List<TestCase> TestCasesStyles = new()
        {
            new TestCase("Call Only", "style='img-style: 50px'", "img-style: 50px"),
            new TestCase("Call Only multiple", "style='img-style: 50px; width: 10px'", "img-style: 50px; width: 10px"),
            new TestCase("Call only, recipe null", "style='img-style: 50px'", "img-style: 50px", useRecipe: true),
            new TestCase("Call only, recipe empty", "style='img-style: 50px'", "img-style: 50px", useRecipe: true, onRecipe: ""),
            new TestCase("recipe only", "style='rec-style: 20px'", null, onRecipe: "rec-style: 20px"),
            new TestCase("Call and recipe", "style='img-style: 50px;rec-style: 20px'", "img-style: 50px",
                useRecipe: true,
                onRecipe: "rec-style: 20px"),
        };

        private static IEnumerable<object[]> TestDataImgStyle { get; } = TestCasesStyles.ToTestEnum();

        [TestMethod]
        [DynamicData(nameof(TestDataImgStyle))]
        public void TestImgStyleOnAttributes(object testObj)
        {
            var test = (TestCase)testObj;
            var callDic = new Dictionary<string, object> { { "style", test.OnCall} };
            var recipeDic = new Dictionary<string, object> { { "style", test.OnRecipe } };
            var recipe = test.UseRecipe ? this.ImgSvc().Recipe(null, attributes: recipeDic) : null;
            var pic = this.ImgSvc().Picture("dummy.jpg", imgAttributes: callDic, recipe: recipe);
            // IsTrue(pic.ToString().Contains(expected), name + ": " + pic);
            IsTrue(pic.Img.ToString().Contains(test.Expected), $"{test} (expected: {test.Expected}): {pic.Img}");
        }

        private static IEnumerable<object[]> TestDataPicStyle { get; } = TestCasesStyles.Where(t => !t.UseRecipe).ToTestEnum();
        [TestMethod]
        [DynamicData(nameof(TestDataPicStyle))]
        public void TestStylePic(object testObj)
        {
            var test = (TestCase)testObj;
            var callDic = new Dictionary<string, object> { { "style", test.OnCall } };
            //var recipeDic = new Dictionary<string, object> { { "style", test.OnRecipe } };
            //var recipe = test.UseRecipe ? this.ImgSvc().Recipe(null, attributes: recipeDic) : null;
            var pic = this.ImgSvc().Picture("dummy.jpg", pictureAttributes: callDic);
            IsTrue(pic.ToString().Contains(test.Expected), $"{test} (expected '{test.Expected}'): {pic.Picture}");
            IsFalse(pic.Img.ToString().Contains(test.Expected), $"{test} (expected '{test.Expected}'): {pic.Img}");
        }


        private const string ImageAttributesStyleValue = "some: 50px";
        private void ImageAttributes(object callDic)
        {
            var expected = "style='some: 50px";
            var pic = this.ImgSvc().Picture("dummy.jpg", imgAttributes: callDic);
            IsTrue(pic.Img.ToString().Contains(expected), $"expected: {expected}: {pic.Img}");
        }

        [TestMethod]
        public void ImageAttributesDicStringObj() => 
            ImageAttributes(new Dictionary<string, object> { { "style", ImageAttributesStyleValue } });
        [TestMethod]
        public void ImageAttributesDicStringObjCasing() => 
            ImageAttributes(new Dictionary<string, object> { { "Style", ImageAttributesStyleValue } });


        [TestMethod]
        public void ImageAttributesDicStringString() => 
            ImageAttributes(new Dictionary<string, string> { { "style", ImageAttributesStyleValue } });
        [TestMethod]
        public void ImageAttributesDicStringStringCasing() => 
            ImageAttributes(new Dictionary<string, string> { { "Style", ImageAttributesStyleValue } });

        [TestMethod]
        public void ImageAttributesAnonymous() => 
            ImageAttributes(new { style = ImageAttributesStyleValue } );
        [TestMethod]
        public void ImageAttributesAnonymousCamelCase() => 
            ImageAttributes(new { Style = ImageAttributesStyleValue } );
    }
}
