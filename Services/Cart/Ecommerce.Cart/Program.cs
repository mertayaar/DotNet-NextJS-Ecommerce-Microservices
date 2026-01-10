using Ecommerce.Cart.Application.Interfaces;
using Ecommerce.Cart.Application.Services;
using Ecommerce.Cart.Infrastructure.ExternalServices;
using Ecommerce.Cart.Infrastructure.Interfaces;
using Ecommerce.Cart.Infrastructure.Repositories;
using Ecommerce.Cart.Settings;
using Ecommerce.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);


var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.Authority = builder.Configuration["IdentityServerUrl"];
    opt.Audience = "cart_api";
    opt.RequireHttpsMetadata = false;
});


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ILoginService, LoginService>();


builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));
builder.Services.AddSingleton<RedisService>(sp =>
{
    var redisSettings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
    var redis = new RedisService(redisSettings.Host, redisSettings.Port);
    redis.Connect();
    return redis;
});


builder.Services.AddSingleton<ICartRepository, RedisCartRepository>();
builder.Services.AddHttpClient<IDiscountServiceClient, DiscountServiceClient>();


builder.Services.AddScoped<ICartApplicationService, CartApplicationService>();


builder.Services.AddControllers(opt =>
{
    opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

builder.Services.AddOpenApi();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseApiExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
