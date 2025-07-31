using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace workshopManagementSystem.Razor.Controllers
{
    public class BaseController : Controller
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Set session data for all controllers
            ViewData["MySession"] = HttpContext.Session.GetString("UserSession");
            base.OnActionExecuting(context);
        }

    }
}
