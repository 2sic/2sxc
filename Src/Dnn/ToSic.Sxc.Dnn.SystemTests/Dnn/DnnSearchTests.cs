namespace ToSic.Sxc.Dnn;

public class DnnSearchTests
{
    [Theory]
    [InlineData(0, true)]
    [InlineData(0, false)]
    [InlineData(7, true)]
    [InlineData(7, false)]
    public void GetModifiedSearchDocuments_PreparesOwnerSite_BeforeBuildingModule(int ownerPortalId, bool isContent)
    {
        using var context = new DnnSearchTestContext();

        var result = context.Probe.RunSearch(ownerPortalId, isContent, failSearch: false);

        Equal(0, result.ErrorsAdded);
        Equal(ownerPortalId + 100, Single(result.RequestedZones.Distinct()));
        Equal(0, result.Documents);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GetModifiedSearchDocuments_DisposesOwnedScope_OnReturnOrFailure(bool failSearch)
    {
        using var context = new DnnSearchTestContext();

        var result = context.Probe.RunSearch(ownerPortalId: 0, isContent: true, failSearch: failSearch);

        Equal(1, result.Created);
        Equal(1, result.DisposedAfterCall);
        Equal(failSearch ? 1 : 0, result.ErrorsAdded);
    }

    [Fact]
    public void GetModifiedSearchDocuments_UsesFreshOwnedScope_WhenControllerIsReused()
    {
        using var context = new DnnSearchTestContext();

        var result = context.Probe.RunRepeatedBackgroundSearch();

        Equal([1, 2], result.ResolvedSentinels);
        Equal(2, result.DisposedAfterCall);
        Equal(0, result.ErrorsAdded);
    }
}
