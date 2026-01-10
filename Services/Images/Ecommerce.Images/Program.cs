using Ecommerce.Images.Services;
using Ecommerce.Images.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);




builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServerUrl"];
        options.Audience = "image_api";
        options.RequireHttpsMetadata = false;
        
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();




builder.Services.Configure<GoogleCloudSettings>(
    builder.Configuration.GetSection("GoogleCloud"));


builder.Services.AddSingleton<IImageUploadService>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<GoogleCloudSettings>>();
    var logger = sp.GetRequiredService<ILogger<GoogleCloudImageUploadService>>();
    
    
    if (string.IsNullOrEmpty(settings.Value.BucketName))
    {
        throw new InvalidOperationException(
            "GoogleCloud:BucketName is not configured. Please set it in appsettings.json or environment variables.");
    }
    
    return new GoogleCloudImageUploadService(settings, logger);
});




builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowInternal", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5000",    
                "http://localhost:5001",    
                "http://localhost:5005",    
                "https://localhost:7210"    
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});




builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();




if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowInternal");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


var logger = app.Services.GetRequiredService<ILogger<Program>>();
var settings = app.Services.GetRequiredService<IOptions<GoogleCloudSettings>>().Value;
logger.LogInformation("Images Service started");
logger.LogInformation("GCS Bucket: {Bucket}", settings.BucketName);
logger.LogInformation("Max file size: {Size}MB", settings.MaxFileSizeBytes / (1024 * 1024));

app.Run();
