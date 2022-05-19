using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentications.Application.Abstractions
{
    public interface IRazorViewRenderService
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewNameOrPath, TModel model);
    }
}
