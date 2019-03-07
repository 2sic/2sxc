using System.Threading;
using ToSic.Eav.Interfaces;

namespace ToSic.Sxc.Context
{
    public static class Entity
    {
        public static object Get(this IEntity entity, string field) => entity.GetBestValue(field, new[] { Thread.CurrentThread.CurrentCulture.Name });

        public static T Get<T>(this IEntity entity, string field) => entity.GetBestValue<T>(field, new[] { Thread.CurrentThread.CurrentCulture.Name });

    }
}
