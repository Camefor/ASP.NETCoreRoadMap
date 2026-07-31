using Jose;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace jose_jwt.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public dynamic Index()
        {
            var payload = new Dictionary<string, object>()
{
    { "sub", "mr.x@contoso.com" },
    { "exp", 1300819380 }
};

            string token = Jose.JWT.Encode(payload, null, JwsAlgorithm.none);
            return token;
        }
    }
}
