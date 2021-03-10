using System;
using System.IO;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Dnn.Code;

// ReSharper disable UnusedMember.Global

namespace ToSic.Sxc.Dnn.Web
{
    /// <summary>
    /// This interface extends the IAppAndDataHelpers with the DNN Context.
    /// It's important, because if 2sxc also runs on other CMS platforms, then the Dnn Context won't be available, so it's in a separate interface.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public interface IDynamicWebApi : IDnnDynamicCode
    {
        /// <summary>
        /// Save a file from a stream (usually an upload from the browser) into an adam-field of an item.
        /// Read more about this in the the [WebAPI docs for SaveInAdam](xref:WebApi.Custom.DotNet.SaveInAdam)
        /// </summary>
        /// <param name="dontRelyOnParameterOrder">ensure that all parameters use names, so the api can change in future</param>
        /// <param name="stream">the stream</param>
        /// <param name="fileName">file name to save to</param>
        /// <param name="contentType">content-type of the target item (important for security checks)</param>
        /// <param name="guid"></param>
        /// <param name="field"></param>
        /// <param name="subFolder"></param>
        /// <returns></returns>
        IFile SaveInAdam(string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            Stream stream = null,
            string fileName = null,
            string contentType = null,
            Guid? guid = null,
            string field = null,
            string subFolder = "");

    }
}
