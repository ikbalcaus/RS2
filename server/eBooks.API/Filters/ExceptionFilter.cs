using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using eBooks.Models.Exceptions;

namespace eBooks.API.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is ExceptionBadRequest ex)
            {
                foreach (var key in ex.Errors.Keys)
                {
                    foreach (var message in ex.Errors[key])
                    {
                        context.ModelState.AddModelError(key, message);
                    }
                }
                context.HttpContext.Response.StatusCode = 400;
            }
            else if (context.Exception is ExceptionForbidden)
            {
                context.ModelState.AddModelError("error", context.Exception.Message);
                context.HttpContext.Response.StatusCode = 403;
            }
            else if (context.Exception is ExceptionNotFound)
            {
                context.ModelState.AddModelError("error", context.Exception.Message);
                context.HttpContext.Response.StatusCode = 404;
            }
            else
            {
                _logger.LogError(context.Exception, context.Exception.Message);
                context.ModelState.AddModelError("error", "Server side error");
                context.HttpContext.Response.StatusCode = 500;
            }
            var list = context.ModelState.Where(x => x.Value.Errors.Count() > 0).ToDictionary(x => x.Key, y => y.Value.Errors.Select(z => z.ErrorMessage));
            context.Result = new JsonResult(new { errors = list });
        }
    }
}
