using Ecommerce.BFF.Services;
using Ecommerce.BFF.Settings;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    var clientUrl = builder.Configuration["ServiceApiSettings:Client:BaseUrl"]!;
    var bffUrl = builder.Configuration["ServiceApiSettings:Bff:BaseUrl"]!;

    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(clientUrl, bffUrl)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); 
    });
});

builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));


builder.Services.AddSingleton<RedisService>(sp =>
{
    var redisSettings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
    Console.WriteLine($"[BFF] Attempting to connect to Redis at {redisSettings.Host}:{redisSettings.Port}, DB: {redisSettings.Db}...");
    try 
    {
        var redis = new RedisService(redisSettings.Host, redisSettings.Port, redisSettings.Db);
        redis.Connect();
        Console.WriteLine("[BFF] Successfully connected to Redis.");
        return redis;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[BFF] CRITICAL ERROR: Failed to connect to Redis: {ex.Message}");
        throw;
    }
});


builder.Services.AddControllers();


builder.Services.AddHttpClient();


builder.Services.AddSingleton<ITokenStore, RedisTokenStore>();

var app = builder.Build();


app.UseCors("AllowFrontend");

app.UseRouting();

app.MapControllers();

app.Run();
