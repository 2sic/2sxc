using System.IO;
using ToSic.Eav.Helpers;
using ToSic.Eav.Internal.Environment;
using ToSic.Lib.DI;
using ToSic.Sxc.Apps.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Razor.Internal;

namespace ToSic.Sxc.Razor;

internal class RazorService(LazySvc<IRazorRenderer> razorRendererLazy) : IRazorService
{
    /// <summary>
    /// render razorPage
    /// </summary>
    /// <param name="partialName">virtual path to razorPage from 2sxc app root folder</param>
    /// <param name="model">data</param>
    /// <returns>string</returns>
    public string Render(string partialName, object model = null)
    {
        var razorPageVirtualPath = GetVirtualPath(partialName);
        var razorPageModel = model ?? new {};

        var task = razorRendererLazy.Value.RenderToStringAsync(razorPageVirtualPath, razorPageModel, null);
        task.Wait();
        return task.Result;
    }

    private string GetVirtualPath(string partialName)
    {
        //return $"~/2sxc/{CodeRoot.App.Site.Id}/{CodeRoot.App.Folder}/{partialName}";
        return Path.Combine(CodeRoot.App.PathSwitch(false, PathTypes.PhysRelative), partialName).ForwardSlash();
    }

    #region Connect to DynamicCodeRoot

    public void ConnectToRoot(ICodeApiService codeRoot)
    {
        CodeRoot = codeRoot;
    }

    public ICodeApiService CodeRoot;

    #endregion
}