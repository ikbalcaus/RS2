using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using eBooks.Models;

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
            _logger.LogError(context.Exception, context.Exception.Message);

            if (context.Exception is ExceptionResult)
            {
                context.ModelState.AddModelError("error", context.Exception.Message);
                context.HttpContext.Response.StatusCode = 400;
            }
            else
            {
                context.ModelState.AddModelError("error", "Server side error");
                context.HttpContext.Response.StatusCode = 500;
            }

            var list = context.ModelState.Where(x => x.Value.Errors.Count() > 0).ToDictionary(x => x.Key, y => y.Value.Errors.Select(z => z.ErrorMessage));
            context.Result = new JsonResult(new { errors = list });
        }
    }
}
