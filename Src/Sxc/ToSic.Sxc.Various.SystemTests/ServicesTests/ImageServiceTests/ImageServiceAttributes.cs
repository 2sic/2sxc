using ToSic.Sxc.Services;

namespace ToSic.Sxc.ServicesTests.ImageServiceTests;

[Startup(typeof(StartupSxcCoreOnly))]
public class ImageServiceAttributes(IImageService imgSvc)
    // Needs fixture to load the Primary App
    : IClassFixture<DoFixtureStartup<ScenarioBasic>>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Name">Test name</param>
    /// <param name="Expected">Expected result</param>
    /// <param name="OnCall">Parameter to use when calling .Picture() or .Img() </param>
    /// <param name="OnRecipe">Parameter to use when calling with recipe...</param>
    /// <param name="useRecipe"></param>
    public record TestCase(string Name, string Expected, string OnCall, string? OnRecipe = default, bool useRecipe = false)
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

    public static TheoryData<TestCase> TestDataImgClass { get; } = [..TestCasesClass];

    [Theory]
    [MemberData(nameof(TestDataImgClass))]
    public void TestClassImg(TestCase test)
    {
        var recDic = new Dictionary<string, object> { { "class", test.OnRecipe } };
        var recipe = test.UseRecipe
            ? imgSvc.Recipe(null, attributes: recDic)
            : null;
        // Classic API
        var pic = imgSvc.Picture("dummy.jpg", imgClass: test.OnCall, recipe: recipe);
        True(pic.Img.ToString().Contains(test.Expected), $"{test}: {pic.Img}");

        // New Tweak API
        var picTweak = imgSvc.Picture("dummy.jpg", tweak: t => t.ImgClass(test.OnCall), recipe: recipe);
        Equal(pic.Img.ToString(), picTweak.Img.ToString());//, $"Should be identical for tweak {test}");
    }

    public static TheoryData<TestCase> TestDataPicClass { get; } = [..TestCasesClass.Where(t => !t.UseRecipe)];
    [Theory]
    [MemberData(nameof(TestDataPicClass))]
    public void TestClassPic(TestCase test)
    {
        var pic = imgSvc.Picture("dummy.jpg", pictureClass: test.OnCall);
        True(pic.ToString().Contains(test.Expected), $"{test}: {pic.Picture}");
        False(pic.Img.ToString().Contains(test.Expected), $"{test}: {pic.Img}");

        // New Tweak API
        var picTweak = imgSvc.Picture("dummy.jpg", tweak: t => t.PictureClass(test.OnCall));
        Equal(pic.ToString(), picTweak.ToString());//, $"Should be identical for tweak {test}");
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

    public static TheoryData<TestCase> TestDataImgStyle { get; } = [..TestCasesStyles];

    [Theory]
    [MemberData(nameof(TestDataImgStyle))]
    public void TestImgStyleOnAttributes(TestCase test)
    {
        var callDic = new Dictionary<string, object> { { "style", test.OnCall} };
        var recipeDic = new Dictionary<string, object> { { "style", test.OnRecipe } };
        var recipe = test.UseRecipe ? imgSvc.Recipe(null, attributes: recipeDic) : null;
        var pic = imgSvc.Picture("dummy.jpg", imgAttributes: callDic, recipe: recipe);
        // True(pic.ToString().Contains(expected), name + ": " + pic);
        True(pic.Img.ToString().Contains(test.Expected), $"{test} (expected: {test.Expected}): {pic.Img}");

        // New Tweak API
        var picTweak = imgSvc.Picture("dummy.jpg", tweak: t => t.ImgAttributes(callDic), recipe: recipe);
        Equal(pic.ToString(), picTweak.ToString());//, $"Should be identical for tweak {test}");
    }

    public static TheoryData<TestCase> TestDataPicStyle { get; } = [..TestCasesStyles.Where(t => !t.UseRecipe)];
    [Theory]
    [MemberData(nameof(TestDataPicStyle))]
    public void TestStylePic(TestCase test)
    {
        var callDic = new Dictionary<string, object> { { "style", test.OnCall } };
        //var recipeDic = new Dictionary<string, object> { { "style", test.OnRecipe } };
        //var recipe = test.UseRecipe ? imgSvc.Recipe(null, attributes: recipeDic) : null;
        var pic = imgSvc.Picture("dummy.jpg", pictureAttributes: callDic);
        True(pic.ToString().Contains(test.Expected), $"{test} (expected '{test.Expected}'): {pic.Picture}");
        False(pic.Img.ToString().Contains(test.Expected), $"{test} (expected '{test.Expected}'): {pic.Img}");

        // New Tweak API
        var picTweak = imgSvc.Picture("dummy.jpg", tweak: t => t.PictureAttributes(callDic));
        Equal(pic.ToString(), picTweak.ToString());//, $"Should be identical for tweak {test}");
    }


    private const string ImageAttributesStyleValue = "some: 50px";
    private void ImageAttributes(object callDic)
    {
        const string expected = "style='some: 50px";
        var pic = imgSvc.Picture("dummy.jpg", imgAttributes: callDic);
        True(pic.Img.ToString().Contains(expected), $"expected: {expected}: {pic.Img}");

        // New Tweak API
        var picTweak = imgSvc.Picture("dummy.jpg", tweak: t => t.ImgAttributes(callDic));
        Equal(pic.ToString(), picTweak.ToString());//, $"Should be identical for tweak {callDic}");
    }

    [Fact]
    public void ImageAttributesDicStringObj() => 
        ImageAttributes(new Dictionary<string, object> { { "style", ImageAttributesStyleValue } });
    [Fact]
    public void ImageAttributesDicStringObjCasing() => 
        ImageAttributes(new Dictionary<string, object> { { "Style", ImageAttributesStyleValue } });


    [Fact]
    public void ImageAttributesDicStringString() => 
        ImageAttributes(new Dictionary<string, string> { { "style", ImageAttributesStyleValue } });
    [Fact]
    public void ImageAttributesDicStringStringCasing() => 
        ImageAttributes(new Dictionary<string, string> { { "Style", ImageAttributesStyleValue } });

    [Fact]
    public void ImageAttributesAnonymous() => 
        ImageAttributes(new { style = ImageAttributesStyleValue } );
    [Fact]
    public void ImageAttributesAnonymousCamelCase() => 
        ImageAttributes(new { Style = ImageAttributesStyleValue } );
}