using ToSic.Sxc.Images;

namespace ToSic.Sxc.Tests.LinksAndImages;


public class SrcSetParsePart
{
    public record TestSpecs(string Variants, double Size, char SizeType, int? Width, int Height);

    public static TheoryData<TestSpecs> LotsOfTests => [
        // Basic Number and optional Width only
        new("100", 100, 'w', 100, 0),
        new("100w", 100, 'w', 100, 0),
        new("2000.4", 2000, 'w', 2000, 0),
        new("2000w", 2000, 'w', 2000, 0),
        new("4w", 4, 'w', 4, 0),   // Very small number, without the 'w' it would default to x

        // Basic W number with additional width
        new("100=100", 100, 'w', 100, 0),
        new("100=700", 100, 'w', 700, 0),
        new("100w=700", 100, 'w', 700, 0),
        new("100=700.9", 100, 'w', 701, 0),

        // Basic W number with additional width and height
        new("100=100:50", 100, 'w', 100, 50),
        new("100=700:300", 100, 'w', 700, 300),
        new("100w=700:400", 100, 'w', 700, 400),
        new("100=700.9:201.7", 100, 'w', 701, 202),

        // Basic W number with no width but height
        new("100=:50", 100, 'w', 100, 50),
        new("100=:300", 100, 'w', 100, 300),
        new("100w=:400", 100, 'w', 100, 400),
        new("100=:201.7", 100, 'w', 100, 202),

        // Basic X-Factor
        new("1", 1, 'x', 0, 0),
        new("1x", 1, 'x', 0, 0),
        new("1.5", 1.5, 'x', 0, 0),
        new("1.5x", 1.5, 'x', 0, 0),
        new("2", 2, 'x', 0, 0),
        new("2x", 2, 'x', 0, 0),
        new("2.25", 2.25, 'x', 0, 0),
        new("2.25x", 2.25, 'x', 0, 0),
        new("12x", 12, 'x', 0, 0), // vary large X-factor, without x it would default to 'w'

        // X-Factor with Width and maybe height
        new("1=45", 1, 'x', 45, 0),
        new("1x=45", 1, 'x', 45, 0),
        new("1.5=77", 1.5, 'x', 77, 0),
        new("1.5x=77.9", 1.5, 'x', 78, 0),
        new("1=45:33", 1, 'x', 45, 33),
        new("1x=45:49", 1, 'x', 45, 49),
        new("1.5=77:22.1", 1.5, 'x', 77, 22),
        new("1.5x=77.9:22.9", 1.5, 'x', 78, 23),
    ];

    [Theory]
    [MemberData(nameof(LotsOfTests))]
    public void ParsePartAsPart(TestSpecs specs)
        => TestPartOnly(specs);

    [Theory]
    [MemberData(nameof(LotsOfTests))]
    public void ParsePartAsSet(TestSpecs specs)
        => TestSetOnly(specs);

    // * Factor
    public static TheoryData<TestSpecs> FactorData => [
        new("0.5*", 0.5, '*', 0, 0),
        new("1/2", 0.5, '*', 0, 0),
        new("1:2", 0.5, '*', 0, 0),
        new("3:4", 0.75, '*', 0, 0),
        new("0.5", 0.5, '*', 0, 0),
        new("0.33", 0.33, '*', 0, 0),
    ];

    [Theory]
    [MemberData(nameof(FactorData))]
    public void TestFactorsPart(TestSpecs specs) => TestPartOnly(specs);
    [Theory]
    [MemberData(nameof(FactorData))]
    public void TestFactorsSet(TestSpecs specs) => TestSetOnly(specs);


    public static TheoryData<TestSpecs> FaultySourceSets => [
        new("1q", 0, 'd', 0, 0),
        new("77vh", 0, 'd', 0, 0),
        new("77vw", 0, 'w', 0, 0), // this 'w' is picked up, because it's the last character
    ];

    [Theory]
    [MemberData(nameof(FaultySourceSets))]
    public void ParseFaultySourcePartOnly(TestSpecs specs) => TestPartOnly(specs);
    [Theory]
    [MemberData(nameof(FaultySourceSets))]
    public void ParseFaultySourceSetOnly(TestSpecs specs) => TestSetOnly(specs);

    // Some invalid data - should basically result in ignored data
    public static TheoryData<TestSpecs> FaultySourceSetsWithComma => [
        new(null, 0, 'd', 0, 0),       // test this as part only, because it would return 0 items when run as a set
        new("", 0, 'd', 0, 0),         // test this as part only, because it would return 0 items when run as a set
        new("99,9", 99, 'w', 99, 0),   // comma should never be used, but in production it will just work, because commas are split before
    ];

    [Theory]
    [MemberData(nameof(FaultySourceSetsWithComma))]
    public void ParseFaultySourcePartWithCommaOnly(TestSpecs specs) => TestPartOnly(specs);


    [Fact]
    public void ParseSet()
    {
        var variants = "100,100w,,100=100,100w=100:,d";
        var expected100 = BuildExpected(100, 'w', 100, 0);
        var expDefault = BuildExpected(0, 'd', 0, 0);
        var result = RecipeVariantsParser.ParseSet(variants);
        Equal(6, result.Length);
        CompareSrcSetPart(variants, result.First(), expected100);
        CompareSrcSetPart(variants, result.Skip(1).First(), expected100);
        CompareSrcSetPart(variants, result.Skip(2).First(), expDefault);
        CompareSrcSetPart(variants, result.Skip(3).First(), expected100);
        CompareSrcSetPart(variants, result.Skip(4).First(), expected100);
        CompareSrcSetPart(variants, result.Skip(5).First(), expDefault);
    }

    /// <summary>
    /// Real test function - standalone to quickly also run a single test if it fails
    /// </summary>
    private static void TestPartOnly(TestSpecs specs)
    {
        var expected = BuildExpected((float)specs.Size, specs.SizeType, specs.Width, specs.Height);
        var result = RecipeVariantsParser.ParsePart(specs.Variants);
        CompareSrcSetPart(specs.Variants, result, expected);
    }

    private static void TestSetOnly(TestSpecs specs)
    {
        var expected = BuildExpected((float)specs.Size, specs.SizeType, specs.Width, specs.Height);
        var asSet = RecipeVariantsParser.ParseSet(specs.Variants);
        Single(asSet);//, "Expect 1 exact hit");
        var first = asSet.First();
        CompareSrcSetPart(specs.Variants, first, expected);
    }

    private static RecipeVariant BuildExpected(float size, char sizeType, int? width, int height) =>
        new()
        {
            Size = size,
            SizeType = sizeType,
            Width = width ?? (int)size,
            Height = height
        };

    private static void CompareSrcSetPart(string variants, RecipeVariant result, RecipeVariant expected)
    {
        NotNull(result);
        True(ParseObject.DNearZero(expected.Size - result.Size), $"Sizes should match on '{variants}'");
        Equal(expected.SizeType, result.SizeType);//, $"Size Types should match on '{variants}'");
        Equal(expected.Width, result.Width);//, $"Widths should match on '{variants}'");
        Equal(expected.Height, result.Height);//, $"Heights should match on '{variants}'");
    }
}