using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SimplCommerce.Infrastructure.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                // The result will go directly to client in json format. 
                // It'd rather be used with Api requests than MVC requests.
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }

    }
}