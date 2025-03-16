using ToSic.Lib.DI;
using ToSic.Sxc.Adam;

namespace ToSic.Sxc.ServicesTests.CmsService.CmsServiceImageExtractor;

public class CmsServiceImageExtractorTests(IServiceProvider sp)
{
    /// <summary>
    /// Must get it like this, because the extractor is internal.
    /// </summary>
#if NETCOREAPP
    [field: System.Diagnostics.CodeAnalysis.AllowNull, System.Diagnostics.CodeAnalysis.MaybeNull]
#endif
    private Services.CmsService.Internal.CmsServiceImageExtractor Extractor => field ??= sp.Build<Services.CmsService.Internal.CmsServiceImageExtractor>();

    private (IFolder Folder, Services.CmsService.Internal.CmsServiceImageExtractor.ImagePropertiesExtracted Properties) GetFolderAndProperties(string imgTag)
    {
        var folder = CmsServiceTestData.GenerateFolderWithTestPng();
        var properties = Extractor.ExtractImagePropertiesTac(imgTag, folder);
        return (folder, properties);
    }

    [Fact]
    public void ExtractorGetsData()
    {
        var (_, props) = GetFolderAndProperties(CmsServiceTestData.ImageTag);
        NotNull(props);
        Equal(CmsServiceTestData.AltText, props.ImgAlt);
    }

    [Fact]
    public void FindFileInFolder()
    {
        var (folder, props) = GetFolderAndProperties(CmsServiceTestData.ImageTag);
        NotNull(props.File);
        Equal(folder.Files.First(), props.File);
    }


}