using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AgricolaDH_GApp.Controllers
{


        public class ViewRenderService
        {

            private readonly RazorViewEngine _razorViewEngine;
            private readonly ITempDataProvider _tempDataProvider;
            private readonly HttpContextAccessor _contextAccessor;

            public ViewRenderService(RazorViewEngine razorViewEngine,
                                     ITempDataProvider tempDataProvider,
                                     HttpContextAccessor contextAccessor)
            {

                _razorViewEngine = razorViewEngine;
                _tempDataProvider = tempDataProvider;
                _contextAccessor = contextAccessor;

            }

            public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)

            {

                var actionContext = new ActionContext(_contextAccessor.HttpContext, _contextAccessor.HttpContext.GetRouteData(), new ActionDescriptor());

                await using var sw = new StringWriter();
                var viewResult = FindView(actionContext, viewName);

                if (viewResult == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())

                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.RenderAsync(viewContext);
                return sw.ToString();
            }



            private IView FindView(ActionContext actionContext, string viewName)
            {
                var getViewResult = _razorViewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: false);
                if (getViewResult.Success)
                {
                    return getViewResult.View;
                }

                var findViewResult = _razorViewEngine.FindView(actionContext, viewName, isMainPage: true);
                if (findViewResult.Success)
                {
                    return findViewResult.View;
                }

                var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
                var errorMessage = string.Join(
                    Environment.NewLine,
                    new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations));

                throw new InvalidOperationException(errorMessage);

            }

        }
    
}
