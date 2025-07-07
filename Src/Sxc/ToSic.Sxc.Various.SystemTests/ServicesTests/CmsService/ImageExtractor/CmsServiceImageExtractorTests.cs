using ToSic.Sxc.Adam;
using ToSic.Sxc.Services.Cms.Sys;

namespace ToSic.Sxc.ServicesTests.CmsService.ImageExtractor;

public class CmsServiceImageExtractorTests(IServiceProvider sp)
{
    /// <summary>
    /// Must get it like this, because the extractor is internal.
    /// </summary>
    private CmsServiceImageExtractor GetExtractor() => sp.Build<CmsServiceImageExtractor>();

    private (IFolder Folder, CmsServiceImageExtractor.ImagePropertiesExtracted Properties) GetFolderAndProperties(string imgTag)
    {
        var folder = DataForCmsServiceTests.GenerateFolderWithTestPng();
        var properties = GetExtractor().ExtractImagePropertiesTac(imgTag, folder);
        return (folder, properties);
    }

    [Fact]
    public void ExtractorGetsData()
    {
        var (_, props) = GetFolderAndProperties(DataForCmsServiceTests.ImageTag);
        NotNull(props);
        Equal(DataForCmsServiceTests.AltText, props.ImgAlt);
    }

    [Fact]
    public void FindFileInFolder()
    {
        var (folder, props) = GetFolderAndProperties(DataForCmsServiceTests.ImageTag);
        NotNull(props.File);
        Equal(folder.Files.First(), props.File);
    }


}