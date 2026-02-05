using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySQL_Master_to_Slave.DataBase;

namespace MySQL_Master_to_Slave.Controllers
{
    //sample demo

    [ApiController]
    [Route("[controller]")]
    public class LogController : ControllerBase
    {
        private readonly MyDbContext _dbContext;
        private readonly IDatabaseIntentService _intentService;
        public LogController(MyDbContext context, IDatabaseIntentService intentService)
        {
            _dbContext = context;
            _intentService = intentService;
        }

        [HttpGet]
        public IEnumerable<Log> Get()
        {
            return [.. _dbContext.AsReadOnly(_intentService).Logs.AsNoTracking().Take(100)];
        }
     
        [HttpGet("master")]
        public IEnumerable<Log> GetFromMaster()
        {
            return [.. _dbContext.Logs.AsNoTracking().Take(100)];
        }

        [HttpPost]
        public IActionResult Post([FromBody] Log log)
        {
            _dbContext.Logs.Add(log);
            _dbContext.SaveChanges();
            return Ok(log);
        }
    }
}
