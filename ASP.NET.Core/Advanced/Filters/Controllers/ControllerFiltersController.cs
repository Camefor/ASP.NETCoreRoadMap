using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Filters.Controllers
{

    /// <summary>
    /// test controller
    /// </summary>
    [SampleActionFilter]
    [ApiController]
    [Route("[controller]")]
    public class ControllerFiltersController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            Console.WriteLine(
                        $"- {nameof(ControllerFiltersController)}.{nameof(Index)}");

            return Content("Check the Console.");
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine(
                $"- {nameof(ControllerFiltersController)}.{nameof(OnActionExecuting)}");

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine(
                $"- {nameof(ControllerFiltersController)}.{nameof(OnActionExecuted)}");

            base.OnActionExecuted(context);
        }

    }
}
