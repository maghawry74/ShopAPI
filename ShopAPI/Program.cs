using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.FileProviders;
using ShopAPI.BL.CustomClasses;
using ShopAPI.Services;
using WatchDog;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureCors();
builder.Services.AddWatchDogServices();
builder.Services.ConfigureDB(builder.Configuration.GetConnectionString("ShopDb")!);
builder.Services.ConfigureRepository();
builder.Services.ConfigureMapper();
builder.Services.ConfigureManagers();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureAuthentication(builder.Configuration.GetValue<string>("Authentication:SecurityKey"));
builder.Services.ConfigureAuthorization();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler(exception =>
{
    exception.Run(async context =>
    {
        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature!.Error is StatusCodeEx Ex)
        {
            context.Response.StatusCode = Ex.StatusCode;
            await context.Response.WriteAsync(Ex.Message);

        }
        else
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync(contextFeature?.Error?.Message!);
        }
    });
});

app.UseHttpsRedirection();
app.UseCors("allowAll");
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "Public")),
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseWatchDog(opt =>
{
    opt.WatchPageUsername = "Admin";
    opt.WatchPagePassword = "password";
});
app.Run();
