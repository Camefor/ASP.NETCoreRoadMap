//理解 OAuth2.0 : https://www.ruanyifeng.com/blog/2014/05/oauth_2_0.html
//关于 OAuth2.0 讲解：https://www.ruanyifeng.com/blog/2019/04/oauth_design.html
//OAuth 2.0 的四种方式 : https://www.ruanyifeng.com/blog/2019/04/oauth-grant-types.html

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using ISExample.Settings;
using ISExample.Models;

namespace ISExample
{
    //https://www.yogihosting.com/identityserver-aspnet-core-identity-mongodb-database

    //在项目 \LibraryAndFramework\Learn IdentityServer with ASP.NET Core\IdentityMongo\IdentityMongo\IdentityMongo.csproj 基础上 增加Id4功能。
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbConfig)).Get<MongoDbConfig>();

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
                (
                    mongoDbSettings.ConnectionString, mongoDbSettings.Name
                );

            var identityServerSettings = builder.Configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>();
            //使用 AddIdentityServer 方法将 IdentityServer 添加到 IServiceCollection
            builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
               .AddAspNetIdentity<ApplicationUser>()
               .AddInMemoryApiScopes(identityServerSettings.ApiScopes)
               .AddInMemoryApiResources(identityServerSettings.ApiResources)
               .AddInMemoryClients(identityServerSettings.Clients)
               .AddInMemoryIdentityResources(identityServerSettings.IdentityResources)
               .AddDeveloperSigningCredential();

            //开发环境
            //.AddSigningCredential(); //生产
            /**
             * 在应用程序启动期间创建临时密钥，此密钥将对我们应用的IdentityServer设置进行签名，以便它们无法伪造。
             * 这仅用于开发情况，不应在生产中使用。生成的密钥将保留在文件系统中，以便在服务器重新启动之间保持稳定。
             * 在生产中，我们应该使用AddSigningCredential方法。
             * **/

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Configure the HTTP request pipeline.
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.Run();
        }
    }
}