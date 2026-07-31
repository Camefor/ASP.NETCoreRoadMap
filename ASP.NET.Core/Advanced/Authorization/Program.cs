using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

const string AuthScheme = "cookie";
const string AuthScheme2 = "cookie2";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(AuthScheme)
    .AddCookie(AuthScheme)
    .AddCookie(AuthScheme2); //注册认证服务

builder.Services.AddAuthorization(builder =>
{
    builder.AddPolicy("eu passport", pb =>
    {
        pb.RequireAuthenticatedUser()
            .AddAuthenticationSchemes(AuthScheme)
            .RequireClaim("passport_type", "eur");
    });
}); //注册授权服务

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();
// app.Use((ctx, next) =>
// {
//     if (ctx.Request.Path.StartsWithSegments("/login"))
//     {
//         return next();
//     }
//     
//     if (!ctx.User.Identities.Any(x => x.AuthenticationType == AuthScheme))
//     {
//         ctx.Response.StatusCode = 401;
//         // return "Unauthenticated"; //未认证
//         //这一步仅检查是否登陆 有这个用户 不进行权限校验 只进行登陆校验
//         return Task.CompletedTask;
//     }
//     
//     if (!ctx.User.HasClaim("passport_type", "eur") && !ctx.User.HasClaim("passport_type", "NOR"))
//     {
//         ctx.Response.StatusCode = 403;
//         // return "Unauthorized"; //未授权
//         return Task.CompletedTask;
//     }
//     
//     return next();
// });

// [Authorize(Policy = "eu passport")] //when its Controller API endpoint
app.MapGet("unsecure", (HttpContext ctx) => { return ctx.User.FindFirst("usr")?.Value ?? "empty"; }).RequireAuthorization("eu passport");


app.MapGet("/norway", (HttpContext ctx) =>
{
    //app.User() 通用检查
    // if (!ctx.User.Identities.Any(x => x.AuthenticationType == AuthScheme))
    // {
    //     ctx.Response.StatusCode = 401;
    //     return "Unauthenticated"; //未认证
    //     //这一步仅检查是否登陆 有这个用户 不进行权限校验 只进行登陆校验
    // }

    if (!ctx.User.HasClaim("passport_type", "NOR"))
    {
        ctx.Response.StatusCode = 403;
        return "Unauthorized"; //未授权
    }

    return "allowed";
}).RequireAuthorization("eu passport");

// [AuthScheme(AuthScheme)]
// [AuthClaim]
app.MapGet("/denmark", (HttpContext ctx) =>
{
    if (!ctx.User.HasClaim("passport_type", "eur"))
    {
        ctx.Response.StatusCode = 403;
        return "Unauthorized"; //未授权
    }

    return "allowed";
});

app.MapGet("/login", async (HttpContext ctx) =>
{
    var claims = new List<Claim>();
    claims.Add(new Claim("usr", "anton"));
    // claims.Add(new Claim("passport_type", "eur")); // request http://localhost:5210/norway will be allowed
    claims.Add(new Claim("passport_type", "NOR")); //request http://localhost:5210/denmark will be allowed
    var identity = new ClaimsIdentity(claims, AuthScheme);
    var user = new ClaimsPrincipal(identity);
    await ctx.SignInAsync(AuthScheme, user);
}).AllowAnonymous();

app.Run();