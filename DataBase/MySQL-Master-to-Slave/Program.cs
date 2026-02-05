using Microsoft.EntityFrameworkCore;
using MySQL_Master_to_Slave.DataBase;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IDatabaseIntentService, DatabaseIntentService>();
builder.Services.AddDbContext<MyDbContext>(); // 在MyDbContext OnConfiguring 处理配置选项

//EfCore 实现MySQL读写分离方案：
/**
 * 基于“DbContext访问上下文”的连接分发器:
 * 通过 DatabaseIntentService 标记和传递连接意图，此次数据库操作是读还是写。在扩展方法 DbContextExtensions的.AsReadOnly(_intentService)方法进行标记为读操作
 * 在DbContext的OnConfiguring方法中，根据连接意图选择使用主库连接字符串还是从库连接字符串，（连接分发）
 * 
 * */

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();