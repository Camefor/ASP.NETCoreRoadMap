//理解 OAuth2.0 : https://www.ruanyifeng.com/blog/2014/05/oauth_2_0.html
//关于 OAuth2.0 讲解：https://www.ruanyifeng.com/blog/2019/04/oauth_design.html
//OAuth 2.0 的四种方式 : https://www.ruanyifeng.com/blog/2019/04/oauth-grant-types.html

namespace ISExample
{
    //https://www.yogihosting.com/identityserver-aspnet-core-identity-mongodb-database/#apiscopes
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}