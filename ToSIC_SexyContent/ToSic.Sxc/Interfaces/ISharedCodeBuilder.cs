namespace ToSic.Sxc.Interfaces
{
    public interface ISharedCodeBuilder
    {
        string SharedCodeVirtualRoot { get; set; }


        dynamic CreateInstance(string virtualPath, 
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter, 
            string name = null,
            string relativePath = null,
            bool throwOnError = true);

    }
}
