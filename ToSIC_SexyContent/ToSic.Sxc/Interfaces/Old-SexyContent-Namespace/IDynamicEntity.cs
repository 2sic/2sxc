using System;
using System.Collections.Generic;
using System.Web;
using ToSic.Eav.Interfaces;

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
        [Obsolete]
        HtmlString Toolbar { get; }

        IEntity Entity { get; }


        int EntityId { get; }

        Guid EntityGuid { get; }

        object EntityTitle { get; }

        dynamic Get(string name);

        dynamic GetDraft();

        dynamic GetPublished();

        bool IsDemoItem { get; }

        List<Sxc.Interfaces.IDynamicEntity> Parents(string type = null, string field = null);
    }
}
