using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreRateLimit.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyTestApiController : ControllerBase
    {

        [HttpGet("test1")]
        public dynamic Test1()
        {
            return "hi im Test1 ";
        }

        [HttpGet("test2")]
        public dynamic Test2()
        {
            return "hi im Test2 ";
        }

    }
}
