using Microsoft.OpenApi.Models;
using RedLockSample.Caching.Redis;
using RedLockSample.Contract;
using RedLockSample.Extensions;
using RedLockSample.Service;

namespace RedLockSample
{

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RedLockSample", Version = "v1" });
            });

            services.AddSingleton<ICacheService, CacheService>();
            services.AddTransient<IContributionService, ContributionService>();


            services.ConfigureOptions(Configuration);

            services.ConfigureDLM(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifeTime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RedLockSample v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            lifeTime.DisposeLockFactory();

        }
    }
}
