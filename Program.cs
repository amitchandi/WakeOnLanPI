using WoLPi.Components;

namespace WoLPi;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddScoped<MainService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.MapGet("/api/status", async (HttpContext context, MainService service) =>
        {
            await context.Response.WriteAsJsonAsync(await service.GetStatusAsync());
        });

        app.MapGet("/api/wake", async (MainService service) =>
        {
            await service.SendWake();
            return "Magic Packet Sent";
        });

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
