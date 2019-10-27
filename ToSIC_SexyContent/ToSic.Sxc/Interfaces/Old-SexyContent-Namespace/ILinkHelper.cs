namespace ToSic.SexyContent.Interfaces
{
    /// <summary>
    /// This is the old interface with the "wrong" namespace
    /// We'll probably need to keep it alive so old code doesn't break
    /// But this interface shouldn't be enhanced or documented publicly
    /// </summary>
    public interface ILinkHelper
    {
        string To(string requiresNamedParameters = null, int? pageId = null, string parameters = null);

        string Base();
    }
}
