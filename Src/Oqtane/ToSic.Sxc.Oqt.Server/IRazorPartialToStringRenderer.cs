using System.Threading.Tasks;

namespace ToSic.Sxc.Oqt.Server
{
    public interface IRazorPartialToStringRenderer
    {
        Task<string> RenderPartialToStringAsync<TModel>(string partialName, TModel model);
    }
}