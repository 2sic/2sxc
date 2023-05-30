#if NETFRAMEWORK
using System;
using ToSic.Lib.Documentation;
using ToSic.Razor.Blade;
using IEntity = ToSic.Eav.Data.IEntity;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Interfaces
{
    /// <summary>
    /// This is the old interface with the "wrong" namespace
    /// We'll probably need to keep it alive so old code doesn't break
    /// But this interface shouldn't be enhanced or documented publicly
    /// </summary>
    [PrivateApi("this was an old interface which must still work for compatibility, but shouldn't be used any more")]
    public interface IDynamicEntity
    {
#if NETFRAMEWORK
        /// <summary>
        /// Deprecated - avoid using. Use Edit.Toolbar(object...) instead
        /// </summary>
        [Obsolete("Use Edit.Toolbar(...) instead")]
        [PrivateApi]
        System.Web.IHtmlString Toolbar { get; }


        /// <summary>
        /// Note: changed to `IHtmlTag` in 16.02
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        [PrivateApi]
        IHtmlTag Render();
        //System.Web.IHtmlString
#endif

        IEntity Entity { get; }


        int EntityId { get; }

        Guid EntityGuid { get; }

        string EntityTitle { get; }

        dynamic Get(string name);

        dynamic GetDraft();

        dynamic GetPublished();

        // 2021-10-25 2dm removed this from the obsolete interface - as it was added in 2sxc 10.07 so it should never have made it to the SexyContent namespace
        //bool IsDemoItem { get; }
    }
}
#endif