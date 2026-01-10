using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("BFFPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5500") 
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});




builder.Services.AddAuthentication()
    .AddJwtBearer("OcelotAuthenticationScheme", options =>
    {
        options.Authority = builder.Configuration["IdentityServerUrl"] ?? "http://localhost:5001";
        options.RequireHttpsMetadata = false; 
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "http://localhost:5001",
            
            ValidateAudience = true,
            ValidAudiences = new[] { 
                "ResourceOcelot", 
                "catalog_api", 
                "cart_api", 
                "order_api", 
                "discount_api",
                "payment_api",
                "review_api",
                "cargo_api",
                "message_api",
                "image_api"
            },
            
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero, 
            
            ValidateIssuerSigningKey = true
        };

        
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"JWT Auth Failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("JWT Token Validated Successfully");
                return Task.CompletedTask;
            }
        };
    });

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("ocelot.json")
    .Build();

builder.Services.AddOcelot(configuration);

var app = builder.Build();


app.UseCors("BFFPolicy");

await app.UseOcelot();

app.MapGet("/", () => "Ocelot Gateway - Healthy");

app.Run();
