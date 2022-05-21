using System.Threading.Tasks;

namespace SharedKernel.Application.Abstractions
{
    public interface IRazorViewRenderService
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewNameOrPath, TModel model);
    }
}
