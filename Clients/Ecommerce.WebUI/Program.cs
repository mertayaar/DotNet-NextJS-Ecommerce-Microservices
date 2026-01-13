using Ecommerce.WebUI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthenticationServices();
builder.Services.AddLocalizationServices(builder.Configuration);
builder.Services.AddMvcServices(builder.Configuration);

builder.Services.AddHttpClientServices(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/GeneralError");
    app.UseHsts();
}
app.UseRequestLocalization();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStatusCodePagesWithReExecute("/Error/PageNotFound", "?code={0}");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/Admin", context =>
    {
        context.Response.Redirect("/Admin/Statistics");
        return Task.CompletedTask;
    });

    endpoints.MapControllerRoute(
        name: "Admin",
        pattern: "Admin/{controller=Statistics}/{action=Index}/{id?}",
        defaults: new { area = "Admin" });

    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
