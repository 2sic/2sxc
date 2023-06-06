using ToSic.Sxc.Adam;

namespace ToSic.Sxc.Data
{
    public partial interface ITypedItem
    {
        /// <summary>
        /// Get the ADAM (Automatic Digital Asset Manager) for this field.
        /// This is a folder which contains all the files and possibly folders which are uploaded on exactly this field.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The Folder object</returns>
        /// <remarks>Added in 16.02</remarks>
        ITypedFolder Folder(string name);

    }
}
