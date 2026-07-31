using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace RedisPublisher.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {

        private const string RedisConnectionString = "localhost:6379";
        private static ConnectionMultiplexer connection;
        private const string Channel = "test-channel";
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            connection = ConnectionMultiplexer.Connect(RedisConnectionString);
            _logger = logger;
        }

        [HttpGet(Name = "")]
        public dynamic Index()
        {

            var json = "{\"type\":2,\"device_id\":\"PB02224009\",\"timestamp\":1662544786,\"current_state\":1,\"gps_latitude\":\"3015.32192\",\"gps_longitude\":\"11941.08924\",\"PM1_data\":4.2,\"PM2d5_data\":4.2,\"PM10_data\":6.0,\"TSP_data\":8.5,\"number_0d3\":10726,\"number_0d5\":434,\"number_1\":87,\"number_2d5\":2.3,\"number_5\":7,\"number_10\":0.3,\"wind_speed\":0.00,\"wind_scale\":0,\"wind_direction\":0,\"wind_direction_data\":0,\"ambient_humid\":50.3,\"ambient_press\":97.9,\"ambient_temp\":29.6,\"ambient_noise\":999999999,\"flowrate\":0.99,\"filter_sampling_temp\":34.5,\"sampling_start_period\":60,\"sampling_start_time\":60,\"data_storage\":60,\"heater_set_temp\":6.0,\"heater_temp\":6.0}";

            var pubsub = connection.GetSubscriber();
            pubsub.PublishAsync(Channel, json, CommandFlags.FireAndForget);
            _logger.LogInformation($"Message Successfully sent to {Channel}");
            return "hello world";
        }
    }
}
