#if NETFRAMEWORK
using ToSic.Razor.Markup;
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
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public interface IDynamicEntity: IEntityWrapper
    {
#if NETFRAMEWORK
        /// <summary>
        /// Deprecated - avoid using. Use Edit.Toolbar(object...) instead
        /// </summary>
        [Obsolete("Use Edit.Toolbar(...) instead")]
        [PrivateApi]
        System.Web.IHtmlString Toolbar { get; }


        /// <summary>
        /// Note: changed to `IRawHtmlString` in 16.02
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        [PrivateApi]
        IRawHtmlString Render();
#endif

        // 2023-08-13 2dm removed and used IEntityWrapper - unsure if this has side effects
        //IEntity Entity { get; }


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