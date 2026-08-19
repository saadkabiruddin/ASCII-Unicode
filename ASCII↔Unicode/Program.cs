using ASCII_Unicode.Services;
using Scalar.AspNetCore;

namespace ASCII_Unicode;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        // Register conversion mappings loader and file watcher
        builder.Services.AddSingleton<ConversionMappingsLoader>();
        builder.Services.AddScoped<BijoyToUnicodeConverter>();
        builder.Services.AddHostedService<FileWatcherService>();

        var app = builder.Build();

        app.MapOpenApi();

        app.MapScalarApiReference(options =>
        {
            options.WithOpenApiRoutePattern("/openapi/v1.json");
        });

        // Comment this if IIS site is HTTP only
        // app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}