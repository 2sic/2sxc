using System.Runtime.CompilerServices;

namespace ToSic.Sxc.Data
{
    public partial class DynamicStack: ITypedObject
    {
        /// <summary>
        /// This error is used a lot, when accessing primary properties of ITypedItem, since it's simply not supported.
        /// </summary>
        private const string ErrNotSupported =
            "You are trying to access '{0}'. This is a merged object containing multiple other sources. " +
            "So there is no real/primary '{0}' property it can provide. " +
            "If you need it, get it from the part you need which was used to create this merged object. " +
            "If you see this error when using a more advanced API such as the ImageService, make sure you give it the correct original object, not the merged object. ";

        private string CreateErrMsg(string addOn = default, [CallerMemberName] string cName = default)
        {
            return string.Format(ErrNotSupported, cName);
        }


        //dynamic ITypedObject.Dyn => this;

        //ITypedItem ITypedItem.Presentation => throw new NotSupportedException(CreateErrMsg());

        //IMetadata ITypedItem.Metadata => throw new NotSupportedException(CreateErrMsg());

        //IField ITypedItem.Field(string name)
        //{
        //    throw new NotImplementedException();
        //}

        //IFolder ITypedItem.Folder(string name)
        //{
        //    throw new NotImplementedException();
        //}

        //IFile ITypedItem.File(string name)
        //{
        //    throw new NotImplementedException();
        //}

        //IEnumerable<ITypedItem> ITypedItem.Parents(string type, string noParamOrder, string field)
        //{
        //    throw new NotImplementedException();
        //}

        //IEnumerable<ITypedItem> ITypedItem.Children(string field, string noParamOrder, string type)
        //{
        //    throw new NotImplementedException();
        //}

        //ITypedItem ITypedItem.Child(string field)
        //{
        //    throw new NotImplementedException();
        //}

        //bool ITypedItem.IsDemoItem => throw new NotSupportedException(CreateErrMsg());

        //IHtmlTag ITypedItem.Html(string name, string noParamOrder, object container,
        //    bool? toolbar, object imageSettings, bool debug)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
