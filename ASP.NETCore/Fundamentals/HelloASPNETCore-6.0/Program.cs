//Program.cs 文件位于：
//1：已配置应用所需的服务。
//2：应用的请求处理管道定义为一系列中间件组件。


//1：
var builder = WebApplication.CreateBuilder(args);  //builder 已将配置、日志记录和许多其他服务 添加到 DI 容器中。

//这里就是配置应用所需服务的地方
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
//将 Razor Pages、带视图的 MVC 控制器 添加到 DI 容器

//builder.Services.AddDbContext<RazorPagesMovieContext>(options =>
//   options.UseSqlServer(builder.Configuration.GetConnectionString("RPMovieContext")));

//(通常使用构造函数注入从 DI 解析服务。 DI 框架在运行时提供此服务的实例。)


//2：
//请求处理管道由一系列中间件组件组成：
//每个组件在 HttpContext 上执行操作，调用管道中的下一个中间件或终止请求。
//按照惯例，通过调用 Use{Feature} 扩展方法，向管道添加中间件组件

var app = builder.Build();//    WebApplication 主机实例化
//ASP.NET Core 应用在启动时构建主机。 主机封装应用的所有资源，例如：
//HTTP 服务器实现
//中间件组件
//Logging
//依赖关系注入 (DI) 服务
//Configuration
//有三个不同的主机：
//.NET WebApplication 主机
//.NET 通用主机
//ASP.NET Core Web 主机
//建议使用 .NET 最小主机，并在所有 ASP.NET Core 模板中使用。 最小和通用主机共享许多相同的接口和类。 ASP.NET Core Web 主机仅用于支持后向兼容性。


// Configure the HTTP request pipeline.
//应用的请求处理管道定义为一系列中间件组件

//配置Http请求中间件
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.MapGet("/hi", () => "Hello!");
app.MapGet("/404", () => "(⓿_⓿)   O.O!");

app.MapDefaultControllerRoute();
app.MapRazorPages();



var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           Random.Shared.Next(-20, 55),
           summaries[Random.Shared.Next(summaries.Length)]
       ))
        .ToArray();
    return forecast;
});

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

