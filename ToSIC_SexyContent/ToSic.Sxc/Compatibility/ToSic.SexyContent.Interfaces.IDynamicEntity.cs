using System;
using System.Web;
using IEntity = ToSic.Eav.Data.IEntity;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Interfaces
{
    /// <summary>
    /// This is the old interface with the "wrong" namespace
    /// We'll probably need to keep it alive so old code doesn't break
    /// But this interface shouldn't be enhanced or documented publicly
    /// </summary>
    public interface IDynamicEntity
    {
        /// <summary>
        /// Deprecated - avoid using. Use Edit.Toolbar(object...) instead
        /// </summary>
        [Obsolete("Use Edit.Toolbar(...) instead")]
        HtmlString Toolbar { get; }


        [Obsolete]
        IHtmlString Render();

        IEntity Entity { get; }


        int EntityId { get; }

        Guid EntityGuid { get; }

        object EntityTitle { get; }

        dynamic Get(string name);

        dynamic GetDraft();

        dynamic GetPublished();

        bool IsDemoItem { get; }

        //List<Sxc.Data.IDynamicEntity> Parents(string type = null, string field = null);
    }
}
