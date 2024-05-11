using Microsoft.AspNetCore.Mvc.Filters;

namespace Stock_Management_System.Services
{
    public class PreventBackButtonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            filterContext.HttpContext.Response.Headers.Add("Pragma", "no-cache");
            filterContext.HttpContext.Response.Headers.Add("Expires", "0");
        }
    }
}
