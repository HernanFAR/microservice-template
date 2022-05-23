using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using SharedKernel.Application.Abstractions;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.Infrastructure.Application
{
    public class RazorViewRenderService : IRazorViewRenderService
    {
        private readonly IRazorViewEngine _ViewEngine;
        private readonly ITempDataProvider _TempDataProvider;
        private readonly IServiceProvider _ServiceProvider;
        private readonly IWebHostEnvironment _Env;

        private static readonly Regex _RegexBetweenTags = new(@">(?! )\s+", RegexOptions.Compiled);
        private static readonly Regex _RegexLineBreaks = new(@"([\n\s])+?(?<= {2,})<", RegexOptions.Compiled);

        public RazorViewRenderService(IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider, IWebHostEnvironment env)
        {
            _ViewEngine = viewEngine;
            _TempDataProvider = tempDataProvider;
            _ServiceProvider = serviceProvider;
            _Env = env;
        }

        public async Task<string> RenderViewToStringAsync<TModel>(string viewNameOrPath, TModel model)
        {
            var actionContext = GetActionContext();
            var view = FindView(actionContext, viewNameOrPath);

            await using var output = new StringWriter();

            var viewContext = new ViewContext(
                actionContext,
                view,
                new ViewDataDictionary<TModel>(
                    new EmptyModelMetadataProvider(),
                    new ModelStateDictionary())
                {
                    Model = model
                },
                new TempDataDictionary(
                    actionContext.HttpContext,
                    _TempDataProvider),
                output,
                new HtmlHelperOptions());

            await view.RenderAsync(viewContext);


            return RemoveWhitespaceFromHtmlPage(output.ToString());
        }

        public static string RemoveWhitespaceFromHtmlPage(string html)
        {
            html = _RegexBetweenTags.Replace(html, ">");
            html = _RegexLineBreaks.Replace(html, "<");

            return html.Trim();
        }

        private IView FindView(ActionContext actionContext, string viewName)
        {
            if (viewName.StartsWith("~/"))
            {
                var getViewResultWithNonStandardPath = _ViewEngine.GetView(_Env.ContentRootPath, viewName, false);

                if (getViewResultWithNonStandardPath.Success)
                {
                    return getViewResultWithNonStandardPath.View;
                }
            }

            var getViewResultMainPage = _ViewEngine.GetView(null, viewName, true);

            if (getViewResultMainPage.Success)
            {
                return getViewResultMainPage.View;
            }

            var getViewResultNonMainPage = _ViewEngine.GetView(null, viewName, false);

            if (getViewResultNonMainPage.Success)
            {
                return getViewResultNonMainPage.View;
            }

            var findViewResultMainPage = _ViewEngine.FindView(actionContext, viewName, isMainPage: true);

            if (findViewResultMainPage.Success)
            {
                return findViewResultMainPage.View;
            }

            var findViewResultNonMainPage = _ViewEngine.FindView(actionContext, viewName, isMainPage: true);

            if (findViewResultNonMainPage.Success)
            {
                return findViewResultNonMainPage.View;
            }

            var searchedLocations = getViewResultMainPage.SearchedLocations
                .Concat(getViewResultNonMainPage.SearchedLocations)
                .Concat(findViewResultMainPage.SearchedLocations)
                .Concat(findViewResultNonMainPage.SearchedLocations)
                .ToList();

            var errorMessage = string.Join(
                Environment.NewLine,
                new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations)); ;

            throw new InvalidOperationException(errorMessage);
        }

        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _ServiceProvider
            };

            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }
}
