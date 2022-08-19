namespace ToSic.Sxc.Web.Url
{
    /// <summary>
    /// Base class for processing URL Values before keeping / converting to a string-url
    /// </summary>
    public abstract class UrlValueProcess
    {
        public abstract NameObjectSet Process(NameObjectSet set);
    }
}
