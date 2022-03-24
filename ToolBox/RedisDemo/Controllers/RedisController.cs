using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace RedisDemo.Controllers {
    [Route("api/redis")]
    [ApiController]
    public class RedisController : ControllerBase {

        private readonly IDatabase _redis;

        public RedisController(RedisHelper client) {
            _redis = client.GetDatabase();
        }

        [HttpGet("setval")]
        public void Set(string val) {
            _redis.StringSet("Name", val);
        }

        [HttpGet("get")]
        public string Get(string key) {
            string name = _redis.StringGet(key);
            return name;
        }


    }
}
