using System.Threading.Tasks;
namespace ToSic.Sxc.OqtaneModule.Server
{
    public interface IRazorPartialToStringRenderer
    {
        Task<string> RenderPartialToStringAsync<TModel>(string partialName, TModel model);
    }
}