using System;
using System.Web;

namespace ToSic.SexyContent.Interfaces
{
    public interface IDynamicEntity
    {
        HtmlString Toolbar { get; }

        int EntityId { get; }

        Guid EntityGuid { get; }

        object EntityTitle { get; }

        dynamic GetDraft();

        dynamic GetPublished();


    }
}
