namespace ToSic.Sxc.Run.Context
{
    /// <summary>
    /// General platform information - must be provided through Dependency Injection
    /// </summary>
    public class PlatformContext
    {
        public Platforms Id;
        public string Name => Id.ToString();
    }
}
