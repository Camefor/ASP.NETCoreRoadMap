using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

const string AuthScheme = "cookie";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(AuthScheme)
    .AddCookie(AuthScheme); //注册认证服务

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthentication();

app.MapGet("unsecure", (HttpContext ctx) => { return ctx.User.FindFirst("usr")?.Value ?? "empty"; });
app.MapGet("sweden", (HttpContext ctx) =>
{
    if (!ctx.User.Identities.Any(x => x.AuthenticationType == AuthScheme))
    {
        ctx.Response.StatusCode = 401;
        return "Unauthenticated"; //未认证
        //这一步仅检查是否登陆 有这个用户 不进行权限校验 只进行登陆校验
    }

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
    claims.Add(new Claim("passport_type", "eur"));
    var identity = new ClaimsIdentity(claims, AuthScheme);
    var user = new ClaimsPrincipal(identity);
    await ctx.SignInAsync(AuthScheme, user);
});

app.Run();