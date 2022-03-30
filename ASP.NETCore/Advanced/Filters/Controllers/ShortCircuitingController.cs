using Microsoft.AspNetCore.Mvc;

namespace Filters.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ResponseHeader("Filter-Header", "Filter Value")]
    public class ShortCircuitingController : Controller
    {
        [HttpGet]
        [ShortCircuitingResourceFilter]
        public IActionResult Index() =>
        Content($"- {nameof(ShortCircuitingController)}.{nameof(Index)}");
    }
}
