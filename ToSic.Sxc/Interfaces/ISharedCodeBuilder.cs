namespace ToSic.Sxc.Interfaces
{
    public interface ISharedCodeBuilder
    {
        string SharedCodeVirtualRoot { get; set; }


        dynamic SharedCode(string path, 
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter, 
            string name = null,
            string relativePath = null,
            bool throwOnError = true);

    }
}
