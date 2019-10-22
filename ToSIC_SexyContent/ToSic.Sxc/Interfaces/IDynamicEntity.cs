using System;
using System.Collections.Generic;
using System.Web;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent.Interfaces
{
    public interface IDynamicEntity
    {
        /// <summary>
        /// Deprecated - avoid using. Use Edit.Toolbar(object...) instead
        /// </summary>
        HtmlString Toolbar { get; }

        int EntityId { get; }

        Guid EntityGuid { get; }

        object EntityTitle { get; }

        dynamic Get(string name);

        dynamic GetDraft();

        dynamic GetPublished();

        bool IsDemoItem { get; }

        List<IDynamicEntity> Parents(string type = null, string field = null);
    }
}
