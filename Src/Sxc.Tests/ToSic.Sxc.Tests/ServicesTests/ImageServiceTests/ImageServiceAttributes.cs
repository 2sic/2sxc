using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Testing.Shared;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.ServicesTests.ImageServiceTests;

[TestClass]
public class ImageServiceAttributes: TestBaseSxcDb
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Name">Test name</param>
    /// <param name="Expected">Expected result</param>
    /// <param name="OnCall">Parameter to use when calling .Picture() or .Img() </param>
    /// <param name="OnRecipe">Parameter to use when calling with recipe...</param>
    /// <param name="useRecipe"></param>
    private record TestCase(string Name, string Expected, string OnCall, string OnRecipe = default, bool useRecipe = false)
    {
        public readonly bool UseRecipe = useRecipe || OnRecipe != null;

        public override string ToString() => $"Test: '{Name}";
    }

    private static readonly List<TestCase> TestCasesClass =
    [
        new("Call Only", "class='img-class'", "img-class"),
        new("Call Only multiple", "class='img-class img-class2'", "img-class img-class2"),
        new("Call only, recipe null", "class='img-class'", "img-class", useRecipe: true),
        new("Call only, recipe empty", "class='img-class'", "img-class", useRecipe: true, OnRecipe: ""),
        new("recipe only", "class='rec-class'", null, OnRecipe: "rec-class"),
        new("Call and recipe", "class='img-class rec-class'", "img-class", useRecipe: true,
            OnRecipe: "rec-class")
    ];

    private static IEnumerable<object[]> TestDataImgClass { get; } = TestCasesClass.ToTestEnum();

    [TestMethod]
    [DynamicData(nameof(TestDataImgClass))]
    public void TestClassImg(object testObj)
    {
        var test = (TestCase)testObj;
        var recDic = new Dictionary<string, object> { { "class", test.OnRecipe } };
        var recipe = test.UseRecipe
            ? this.ImgSvc().Recipe(null, attributes: recDic)
            : null;
        // Classic API
        var pic = this.ImgSvc().Picture("dummy.jpg", imgClass: test.OnCall, recipe: recipe);
        IsTrue(pic.Img.ToString().Contains(test.Expected), $"{test}: {pic.Img}");

        // New Tweak API
        var picTweak = this.ImgSvc().Picture("dummy.jpg", tweak: t => t.ImgClass(test.OnCall), recipe: recipe);
        AreEqual(pic.Img.ToString(), picTweak.Img.ToString(), $"Should be identical for tweak {test}");
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

        // New Tweak API
        var picTweak = this.ImgSvc().Picture("dummy.jpg", tweak: t => t.PictureClass(test.OnCall));
        AreEqual(pic.ToString(), picTweak.ToString(), $"Should be identical for tweak {test}");
    }


    private static readonly List<TestCase> TestCasesStyles =
    [
        new("Call Only", "style='img-style: 50px'", "img-style: 50px"),
        new("Call Only multiple", "style='img-style: 50px; width: 10px'", "img-style: 50px; width: 10px"),
        new("Call only, recipe null", "style='img-style: 50px'", "img-style: 50px", useRecipe: true),
        new("Call only, recipe empty", "style='img-style: 50px'", "img-style: 50px", useRecipe: true, OnRecipe: ""),
        new("recipe only", "style='rec-style: 20px'", null, OnRecipe: "rec-style: 20px"),
        new("Call and recipe", "style='img-style: 50px;rec-style: 20px'", "img-style: 50px",
            useRecipe: true,
            OnRecipe: "rec-style: 20px")

    ];

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

        // New Tweak API
        var picTweak = this.ImgSvc().Picture("dummy.jpg", tweak: t => t.ImgAttributes(callDic), recipe: recipe);
        AreEqual(pic.ToString(), picTweak.ToString(), $"Should be identical for tweak {test}");
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

        // New Tweak API
        var picTweak = this.ImgSvc().Picture("dummy.jpg", tweak: t => t.PictureAttributes(callDic));
        AreEqual(pic.ToString(), picTweak.ToString(), $"Should be identical for tweak {test}");
    }


    private const string ImageAttributesStyleValue = "some: 50px";
    private void ImageAttributes(object callDic)
    {
        const string expected = "style='some: 50px";
        var pic = this.ImgSvc().Picture("dummy.jpg", imgAttributes: callDic);
        IsTrue(pic.Img.ToString().Contains(expected), $"expected: {expected}: {pic.Img}");

        // New Tweak API
        var picTweak = this.ImgSvc().Picture("dummy.jpg", tweak: t => t.ImgAttributes(callDic));
        AreEqual(pic.ToString(), picTweak.ToString(), $"Should be identical for tweak {callDic}");
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