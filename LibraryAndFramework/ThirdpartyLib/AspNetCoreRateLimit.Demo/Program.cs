using AspNetCoreRateLimit;
using Microsoft.Extensions.Configuration;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

//builder.Services.AddControllers();

//https://github.com/stefanprodan/AspNetCoreRateLimit
//AspNetCoreRateLimit 所需依赖配置：
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));
builder.Services.Configure<ClientRateLimitPolicies>(builder.Configuration.GetSection("ClientRateLimitPolicies"));

// inject counter and rules stores
builder.Services.AddInMemoryRateLimiting();
//services.AddDistributedRateLimiting<AsyncKeyLockProcessingStrategy>();
//services.AddDistributedRateLimiting<RedisProcessingStrategy>();
//services.AddRedisRateLimiting();



// Add framework services.
builder.Services.AddMvc();

// configuration (resolvers, counter key builders)
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();




//支持 分布式系统中使用
// inject counter and rules distributed cache stores
//builder.Services.AddSingleton<IClientPolicyStore, DistributedCacheClientPolicyStore>();
//builder.Services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();

var app = builder.Build();

app.UseClientRateLimiting();


// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
